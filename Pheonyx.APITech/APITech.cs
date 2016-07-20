using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Pheonyx.APITech.Database;
using Pheonyx.APITech.Utils;

namespace Pheonyx.APITech
{
    public sealed class ApiTech
    {
        private readonly EQuery.QueryLock _accessManager = new EQuery.QueryLock();
        private readonly Dictionary<string, JToken> _configFiles = new Dictionary<string, JToken>();
        private readonly WebApiClient _web;

        #region Constructor

        /// <summary>
        ///     Initialise une nouvelle instance de l'API d'Epitech avec le client web configuré par défaut (Temps max d'une
        ///     requête: 30s / Agent web: ".NET Epitech API").
        /// </summary>
        /// <param name="ignoreStatusCode">Tableau de <see cref="HttpStatusCode" /> à ignorer</param>
        public ApiTech(HttpStatusCode[] ignoreStatusCode = null)
        {
            _web = new WebApiClient(new TimeSpan(0, 0, 0, 30), ".NET Epitech API", ignoreStatusCode);
            _accessManager.QueryInstance = Database;
            _accessManager.Lock();
        }

        /// <summary>
        ///     Initialise une nouvelle instance de l'API d'Epitech avec l'agent web configuré par défaut (".NET Epitech API").
        /// </summary>
        /// <param name="timeout">Temps de réponse maximum d'une requête.</param>
        /// <param name="ignoreStatusCode">Tableau de <see cref="HttpStatusCode" /> à ignorer.</param>
        public ApiTech(TimeSpan timeout, HttpStatusCode[] ignoreStatusCode = null)
        {
            _web = new WebApiClient(timeout, ".NET Epitech API", ignoreStatusCode);
            _accessManager.QueryInstance = Database;
            _accessManager.Lock();
        }

        /// <summary>
        ///     Initialise une nouvelle instance de l'API d'Epitech avec le temps d'une requête configuré par défaut (30s).
        /// </summary>
        /// <param name="apiWebAgent">Agent web de l'API.</param>
        /// <param name="ignoreStatusCode">Tableau de <see cref="HttpStatusCode" /> à ignorer.</param>
        public ApiTech(string apiWebAgent, HttpStatusCode[] ignoreStatusCode = null)
        {
            _web = new WebApiClient(new TimeSpan(0, 0, 0, 30), apiWebAgent, ignoreStatusCode);
            _accessManager.QueryInstance = Database;
            _accessManager.Lock();
        }

        /// <summary>
        ///     Initialise une nouvelle instance de l'API d'Epitech.
        /// </summary>
        /// <param name="timeout">Temps de réponse maximum d'une requête.</param>
        /// <param name="apiWebAgent">Agent web de l'API.</param>
        /// <param name="ignoreStatusCode">Tableau de <see cref="HttpStatusCode" /> à ignorer.</param>
        public ApiTech(TimeSpan timeout, string apiWebAgent, HttpStatusCode[] ignoreStatusCode = null)
        {
            _web = new WebApiClient(timeout, apiWebAgent, ignoreStatusCode);
            _accessManager.QueryInstance = Database;
            _accessManager.Lock();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        ///     Obtient une valeur indiquant si <see cref="EpitechApi" /> est connectée à l'intranet d'Epitech.
        /// </summary>
        public bool IsConnected => _web.IsConnected;

        /// <summary>
        ///     Obtient une valeur indiquant si <see cref="EpitechApi" /> est configurée.
        /// </summary>
        public bool IsConfigured => _configFiles.Count > 0;

        /// <summary>
        ///     Obtient la base de donnée de <see cref="EpitechApi" />.
        /// </summary>
        public EQuery Database { get; private set; } = new EObject();

        #endregion Properties

        #region User methods

        /// <summary>
        ///     Connecte l'API avec l'intranet d'Epitech
        /// </summary>
        /// <param name="manager">Type de connexion (Classic: login UNIX + Password / Office365: mail office + Password).</param>
        /// <param name="uri">Url de connexion sur l'intranet.</param>
        /// <param name="login">Login / Mail de connexion.</param>
        /// <param name="password">Mot de passe de connexion.</param>
        /// <returns><see langword="true" /> si la connexion a réussie; Sinon <see langword="false" />.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ProtocolViolationException"></exception>
        /// <exception cref="WebException"></exception>
        public bool ConnectTo(ConnectionManager manager, string uri, string login, string password)
        {
            _web.ConnectToApi(manager, uri, login, password);
            return IsConnected;
        }

        /// <summary>
        ///     Configure l'architecture de l'API.
        /// </summary>
        /// <param name="jsonConfiguration">Liste contenant la (les) configuration(s) de l'API en JSON (voir Github)</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public void ConfigureApi(List<string> jsonConfiguration)
        {
            foreach (var jsonFile in jsonConfiguration)
                AddJsonFile(jsonFile);
        }

        /// <summary>
        ///     Nettoie la base de donnée et la configuration de l'API.
        /// </summary>
        public void ClearApi()
        {
            _configFiles.Clear();
            Database = new EObject();
            _accessManager.QueryInstance = Database;
        }

        /// <summary>
        ///     Charge les données voulue dans la base de données en fonctions des paramètres.
        /// </summary>
        /// <param name="dVar">Dictionnaire contant les variables de configuration (Variable => Valeur ou Liste de Value).</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ProtocolViolationException"></exception>
        /// <exception cref="WebException"></exception>
        public void LoadData(Dictionary<string, object> dVar)
        {
            if (!IsConnected)
                throw new Exception("API not connected. Use ConnectTo() before.");
            _accessManager.Unlock();
            foreach (var db in _configFiles)
                ChargeApi(db.Value, dVar);
            _accessManager.Lock();
        }

        #endregion

        #region Configurator & Loader methods

        private void AddJsonFile(string jsonFile)
        {
            JToken jRoot;
            JValue jName;
            JArray jModules;

            try
            {
                jRoot = JToken.Parse(jsonFile);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to parse JSON file: {e.Message}.");
            }

            if ((jName = jRoot.SelectToken("API-local-config.API-dbName") as JValue) == null)
                throw new InvalidDataException(
                    "Incorrect configuration file: Database name missed (API-local-config.API-dbName).");
            if ((jModules = jRoot.SelectToken("API-local-config.API-modules") as JArray) == null)
                throw new InvalidDataException(
                    "Incorrect configuration file: Modules array missed (API-local-config.API-modules).");

            if (jModules.Empty())
                throw new InvalidDataException(
                    "Incorrect configuration file: Modules array was empty (API-local-config.API-modules).");

            _configFiles[jName.ToString()] = jRoot;
        }

        private void ChargeApi(JToken jDatabase, Dictionary<string, object> dVar)
        {
            JArray jModules;

            if ((jModules = jDatabase.SelectToken("API-local-config.API-modules") as JArray) == null || jModules.Empty())
                throw new InvalidOperationException(
                    "Internal failure: Can't access to API-local-config.API-modules (or is empty).");

            foreach (var jModule in jModules)
                ChargeModule(jModule, jDatabase, dVar);
        }

        private void ChargeModule(JToken jModule, JToken jDatabase, Dictionary<string, object> dVar)
        {
            JToken jDownload;
            JValue jModuleName;
            JValue jModuleUrl;
            string sModuleUrl;

            if ((jModuleName = jModule["Name"] as JValue) == null)
                throw new InvalidDataException($"Incorrect configuration file: Module name missed ({jModule.Path}.Name)");
            if ((jModuleUrl = jModule["Url"] as JValue) == null)
                throw new InvalidDataException($"Incorrect configuration file: Module name missed ({jModule.Path}.Url)");
            if ((sModuleUrl = Tools.SetVar(jModuleUrl, dVar)) == null)
                throw new InvalidDataException($"Incorrect configuration file: Invalid url {jModuleUrl}");

            try
            {
                jDownload = _web.DownloadJson(sModuleUrl);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $"Incorrect configuration file: Can't download from {sModuleUrl} ({e.Message})");
            }

            foreach (var jObj in jDatabase[jModuleName.ToString()].Children())
                ChargeEQuery(jObj, jDownload, Database);
        }

        private EQuery ChargeEQuery(JToken jObj, JToken jDatabase, EQuery eParent)
        {
            switch (jObj.Type)
            {
                case JTokenType.Property:

                    var oParent = eParent as EObject;
                    var jProperty = jObj as JProperty;
                    if (oParent == null)
                        return new ENull(ReasonType.InvalidJsonType,
                            $"Internal EQuery error: jObj is a JSON property, but his parent is not EObject ({jObj.Path})");
                    if (jProperty == null)
                        return new ENull(ReasonType.JsonNetFailure,
                            $"JSON.NET failure: {jObj.Path} is a JSON property, but not JPropery ({jObj.Path})");

                    var propertyQuery = ChargeEQuery(jProperty.Value, jDatabase, eParent);
                    if (oParent.ContainsKey(jProperty.Name) && oParent[jProperty.Name] is EObject &&
                        propertyQuery is EObject)
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
                return new ENull(ReasonType.ConfigurationFile,
                    $"Invalid configuration file: Invalid array configuration at {jObj.Path}");

            var eArray = new EArray();
            var jArrayDb = Tools.AccessTo(jObj[0].Value<string>(), jDatabase);
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
            EQuery eValue;
            if ((jValue = Tools.AccessTo(jObj, jDatabase)) == null)
                return new ENull(ReasonType.AccessFailure, $"Internal access error: Can't parse {jObj}");
            if (jValue is JArray)
            {
                var eArray = new EArray();
                foreach (var jItem in jValue as JArray)
                    eArray.Add(new EValue(jItem.ToString()));
                return eArray;
            }
            if (!(jValue is JValue))
                eValue = new ENull(ReasonType.InvalidValue, $"Incorrect Value (null or {jObj.Type} is invalid type)");
            else
                eValue = new EValue((jValue as JValue).Value);
            return eValue;
        }

        #endregion Configurator & Loader methods
    }
}