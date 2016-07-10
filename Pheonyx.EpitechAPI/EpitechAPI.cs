using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public sealed partial class EpitechAPI
    {
        private WebApiClient web = null;
        private Dictionary<String, JToken> configFiles = new Dictionary<String, JToken>();
        private EQuery eDatabase = new EObject();
        private EQuery.QueryLock accessManager = new EQuery.QueryLock();

        #region Constructor
        public EpitechAPI(HttpStatusCode[] ignoreStatusCode = null) : this(new TimeSpan(0, 0, 1, 30), ".NET Epitech API", ignoreStatusCode) { }
        public EpitechAPI(TimeSpan timeout, HttpStatusCode[] ignoreStatusCode = null) : this(timeout, ".NET Epitech API", ignoreStatusCode) { }
        public EpitechAPI(String apiWebAgent, HttpStatusCode[] ignoreStatusCode = null) : this(new TimeSpan(0, 0, 1, 30), apiWebAgent, ignoreStatusCode) { }
        public EpitechAPI(TimeSpan timeout, String apiWebAgent, HttpStatusCode[] ignoreStatusCode = null)
        {
            web = new WebApiClient(timeout, apiWebAgent, ignoreStatusCode);
            accessManager.QueryInstance = eDatabase;
            accessManager.Lock();
        }
        #endregion

        public Boolean IsConnected
        {
            get
            {
                return web.IsConnected;
            }
        }
        public Boolean IsConfigured
        {
            get
            {
                return configFiles.Count > 0;
            }
        }
        public EQuery Database
        {
            get
            {
                return eDatabase;
            }
        }

        private void AddConfig(String jsonFile)
        {
            //TODO: Error manager
            JToken jRoot = JToken.Parse(jsonFile);
            string dbName = (jRoot.SelectToken("API-local-config.API-dbName") as JValue).Value<String>();
            if ((jRoot.SelectToken("API-local-config.API-modules") as JArray).Count > 0)
                configFiles[dbName] = jRoot;
        }
        private void ChargeAPI(JToken jDatabase, Dictionary<String, Object> dVar)
        {
            JArray modules = jDatabase.SelectToken("API-local-config.API-modules") as JArray;
            foreach (JToken jModule in modules)
                ChargeModule(jModule, jDatabase, dVar);
        }
        private void ChargeModule(JToken jModule, JToken jDatabase, Dictionary<String, Object> dVar)
        {
            string moduleName = jModule["Name"].Value<String>();
            string moduleUrl = Utils.Json.setVar(jModule["Url"], dVar);

            JToken jDownload = web.DownloadJson(moduleUrl);
            foreach (JToken jObj in jDatabase[moduleName].Children())
                ChargeEQuery(jObj, jDownload, eDatabase);
        }

        private EQuery ChargeEQuery(JToken jObj, JToken jDatabase, EQuery parent)
        {
            switch (jObj.Type)
            {
                case JTokenType.Property:
                    string propertyName = (jObj as JProperty).Name;
                    EQuery propertyQuery = ChargeEQuery((jObj as JProperty).Value, jDatabase, parent);
                    if ((parent as EObject).ContainsKey(propertyName) && parent[propertyName] is EObject && propertyQuery is EObject)
                        (parent[propertyName] as EObject).Add(propertyQuery as EObject);
                    else
                        parent[propertyName] = propertyQuery;
                    return parent;
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
            EObject eObj = new EObject();
            foreach (JToken jChild in jObj.Children())
                ChargeEQuery(jChild, jDatabase, eObj);
            return eObj;
        }
        private EQuery ChargeEArray(JArray jObj, JToken jDatabase)
        {
            if (jObj.Count == 0 || jObj.Count > 2 ||
                jObj[0].Type != JTokenType.String || 
                jObj[1].Type != JTokenType.Object)
                return new EValue(null); //Todo: ENull: Invalid Array configuration

            EArray eArray = new EArray();
            JToken jArrayDB = Utils.Json.accessTo(jObj[0].Value<String>(), jDatabase);
            if (jArrayDB == null || jArrayDB.Type != JTokenType.Array)
                return new EValue(null); //Todo: ENull: Is not Array

            foreach (JToken jChild in jArrayDB.Children())
            {
                EObject eObj = new EObject();
                eArray.Add(ChargeEQuery(jObj[1], jChild, eObj));
            }
            return eArray;
        }
        private EQuery ChargeEValue(JValue jObj, JToken jDatabase)
        {
            JToken jValue = Utils.Json.accessTo(jObj, jDatabase);
            if (jValue is JArray)
            {
                EArray eArray = new EArray();
                foreach (JToken jItem in jValue as JArray)
                    eArray.Add(new EValue(jItem.ToString()));
                return eArray;
            }
            EValue eValue;
            if (jValue == null || !(jValue is JValue))
                eValue = new EValue(null); //Todo: ENull: Incorrect Value (null or invalid)
            else
                eValue = new EValue((jValue as JValue).Value);
            return (eValue);
        }

        public Boolean ConnectTo(ConnectionManager manager, String uri, String login, String password)
        {
            web.ConnectToAPI(manager, uri, login, password);
            return IsConnected;
        }
        public void ConfigAPI(List<String> configFilePath)
        {
            foreach (var jsonFile in configFilePath)
                AddConfig(jsonFile);
        }
        public void LoadAPI(Dictionary<String, Object> dVar)
        {
            accessManager.Unlock();
            if (!IsConnected)
                return; //Todo: Error manager
            foreach (var db in configFiles)
                ChargeAPI(db.Value, dVar);
            accessManager.Lock();
        }
    }
}
