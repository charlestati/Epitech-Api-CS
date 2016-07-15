using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI.Database;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI
{
    public sealed class EpitechApi
    {
        private readonly WebApiClient _web;
        private readonly Dictionary<String, JToken> _configFiles = new Dictionary<String, JToken>();
        private readonly EQuery _eDatabase = new EObject();
        private readonly EQuery.QueryLock _accessManager = new EQuery.QueryLock();
        private String _errorMessage = "";

        #region Constructor
        public EpitechApi(HttpStatusCode[] ignoreStatusCode = null) : this(new TimeSpan(0, 0, 1, 30), ".NET Epitech API", ignoreStatusCode) { }
        public EpitechApi(TimeSpan timeout, HttpStatusCode[] ignoreStatusCode = null) : this(timeout, ".NET Epitech API", ignoreStatusCode) { }
        public EpitechApi(String apiWebAgent, HttpStatusCode[] ignoreStatusCode = null) : this(new TimeSpan(0, 0, 1, 30), apiWebAgent, ignoreStatusCode) { }
        public EpitechApi(TimeSpan timeout, String apiWebAgent, HttpStatusCode[] ignoreStatusCode = null)
        {
            _web = new WebApiClient(timeout, apiWebAgent, ignoreStatusCode);
            _accessManager.QueryInstance = _eDatabase;
            _accessManager.Lock();
        }
        #endregion

        public Boolean IsConnected => _web.IsConnected;
        public Boolean IsConfigured => _configFiles.Count > 0;
        public EQuery Database => _eDatabase;
        public String ErrorMessage => _errorMessage;

        private Boolean AddJsonFile(String jsonFile)
        {
            JToken jRoot;
            JValue jName;
            JArray jModules;

            _errorMessage = String.Empty;
            try
            {
                jRoot = JToken.Parse(jsonFile);
            }
            catch (Exception e)
            {
                _errorMessage = $"Failed to parse JSON file: {e.Message}.";
                return false;
            }

            if ((jName = jRoot.SelectToken("API-local-config.API-dbName") as JValue) == null)
            {
                _errorMessage = "Incorrect configuration file: Database name missed (API-local-config.API-dbName).";
                return false;
            }
            if ((jModules = jRoot.SelectToken("API-local-config.API-modules") as JArray) == null)
            {
                _errorMessage = "Incorrect configuration file: Modules array missed (API-local-config.API-modules).";
                return false;
            }

            if (jModules.Empty())
            {
                _errorMessage = "Incorrect configuration file: Modules array was empty (API-local-config.API-modules).";
                return false;
            }

            _configFiles[jName.ToString()] = jRoot;
            return true;
        }

        private void ChargeApi(JToken jDatabase, Dictionary<String, Object> dVar)
        {
            JArray jModules;

            if ((jModules = jDatabase.SelectToken("API-local-config.API-modules") as JArray) == null || jModules.Empty())
                throw new Exception($"Internal failure: Can't access to API-local-config.API-modules (or is empty).");

            foreach (var jModule in jModules)
                ChargeModule(jModule, jDatabase, dVar);
        }
        private void ChargeModule(JToken jModule, JToken jDatabase, Dictionary<String, Object> dVar)
        {
            JToken jDownload;
            JValue jModuleName;
            JValue jModuleUrl;
            string sModuleUrl;

            if ((jModuleName = jModule["Name"] as JValue) == null)
                throw new Exception($"Incorrect configuration file: Module name missed ({jModule.Path}.Name)");
            if ((jModuleUrl = jModule["Url"] as JValue) == null)
                throw new Exception($"Incorrect configuration file: Module name missed ({jModule.Path}.Url)");
            if ((sModuleUrl = Tools.SetVar(jModuleUrl, dVar)) == null)
                throw new Exception($"Incorrect configuration file: Invalid url {jModuleUrl}");

            try
            {
                jDownload = _web.DownloadJson(sModuleUrl);
            }
            catch (Exception e)
            {                
                throw new Exception($"Incorrect configuration file: Can't download from {sModuleUrl} ({e.Message})");
            }

            foreach (var jObj in jDatabase[jModuleName.ToString()].Children())
                ChargeEQuery(jObj, jDownload, _eDatabase);
        }

        private EQuery ChargeEQuery(JToken jObj, JToken jDatabase, EQuery eParent)
        {
            switch (jObj.Type)
            {
                case JTokenType.Property:

                    var oParent = eParent as EObject;
                    var jProperty = jObj as JProperty;
                    if (oParent == null)
                        return new ENull(ReasonType.InvalidJsonType, $"Internal EQuery error: jObj is a JSON property, but his parent is not EObject ({jObj.Path})");
                    if (jProperty == null)
                        return new ENull(ReasonType.JsonNetFailure, $"JSON.NET failure: {jObj.Path} is a JSON property, but not JPropery ({jObj.Path})"); 

                    var propertyQuery = ChargeEQuery(jProperty.Value, jDatabase, eParent);
                    if (oParent.ContainsKey(jProperty.Name) && oParent[jProperty.Name] is EObject && propertyQuery is EObject)
                        ((EObject) oParent[jProperty.Name]).Add(propertyQuery as EObject);
                    else
                        oParent[jProperty.Name] = propertyQuery;
                    return oParent;
                case JTokenType.Object:
                    return ChargeEObject(jObj as JObject, jDatabase);
                case JTokenType.Array:
                    return ChargeEArray(jObj as JArray, jDatabase);
                default:
                    return ChargeEValue(jObj as JValue, jDatabase);
            }
        }
        private EQuery ChargeEObject(JObject jObj, JToken jDatabase)
        {
            var eObj = new EObject();
            foreach (var jChild in jObj.Children())
                if (jChild != null)
                    ChargeEQuery(jChild, jDatabase, eObj);
            return eObj;
        }
        private EQuery ChargeEArray(JArray jObj, JToken jDatabase)
        {
            if (jObj.Count == 0 || jObj.Count > 2 ||
                jObj[0].Type != JTokenType.String ||
                jObj[1].Type != JTokenType.Object)
                return new ENull(ReasonType.ConfigurationFile, $"Invalid configuration file: Invalid array configuration at {jObj.Path}");

            var eArray = new EArray();
            var jArrayDb = Tools.AccessTo(jObj[0].Value<String>(), jDatabase);
            if (jArrayDb == null || jArrayDb.Type != JTokenType.Array)
                return new ENull(ReasonType.AccessFailure, $"{jObj[0]} is not a JSON Array");

            foreach (var jChild in jArrayDb.Children())
            {
                if (jChild == null)
                    continue;
                var eObj = new EObject();
                eArray.Add(ChargeEQuery(jObj[1], jChild, eObj));
            }
            return eArray;
        }
        private EQuery ChargeEValue(JValue jObj, JToken jDatabase)
        {
            JToken jValue;
            if ((jValue = Tools.AccessTo(jObj, jDatabase)) == null)
                return new ENull(ReasonType.AccessFailure, $"Internal access error: Can't parse {jObj}");
            if (jValue is JArray)
            {
                var eArray = new EArray();
                foreach (var jItem in jValue as JArray)
                    eArray.Add(new EValue(jItem.ToString()));
                return eArray;
            }
            EQuery eValue;
            if (!(jValue is JValue))
                eValue = new ENull(ReasonType.InvalidValue, $"Incorrect Value (null or {jObj.Type} is invalid type)");
            else
                eValue = new EValue((jValue as JValue).Value);
            return eValue;
        }

        public Boolean ConnectTo(ConnectionManager manager, String uri, String login, String password)
        {
            _web.ConnectToApi(manager, uri, login, password);
            return IsConnected;
        }
        public Boolean ConfigureApi(List<String> jsonConfiguration)
        {
            foreach (var jsonFile in jsonConfiguration)
                if (!AddJsonFile(jsonFile))
                {
                    _configFiles.Clear();
                    return false;
                }
            return true;
        }
        public void ClearConfig()
        {
            _configFiles.Clear();
        }
        public Boolean LoadApi(Dictionary<String, Object> dVar)
        {
            if (!IsConnected)
            {
                _errorMessage = "API not connected. Use ConnectTo() before.";
                return false;
            }
            _accessManager.Unlock();
            foreach (var db in _configFiles)
                ChargeApi(db.Value, dVar);
            _accessManager.Lock();
            return true;
        }
    }
}
