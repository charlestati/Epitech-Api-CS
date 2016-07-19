using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public enum ConnectionManager { Office365, Classic }

    public sealed class WebApiClient
    {
        private enum NetworkMethod { GET, POST }

        private Boolean connected = false;
        private String webApiAgent;
        private TimeSpan webTimeOut;
        private CookieContainer cookies;
        private HttpStatusCode[] ignoreStatusCode = { };

        #region Constructor
        public WebApiClient(TimeSpan webTimeOut, String webApiAgent, HttpStatusCode[] ignoreStatusCode)
        {
            this.webTimeOut = webTimeOut;
            this.webApiAgent = webApiAgent;
            if (ignoreStatusCode != null)
                this.ignoreStatusCode = ignoreStatusCode;
        }
        #endregion

        #region Networks
        private String GetStringData(Dictionary<String, Object> data)
        {
            string dataString = "";

            if (data == null)
                return ("");
            foreach (var kv in data)
                if (kv.Key != null && kv.Key != "")
                    dataString += "&" + kv.Key + "=" + kv.Value;
            return (dataString.TrimStart('&'));
        }
        private HttpWebResponse LoadUri(Uri uri, NetworkMethod method, Dictionary<String, Object> data)
        {
            Byte[] dataByte = Encoding.UTF8.GetBytes(GetStringData(data));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            HttpWebResponse response = null;

            request.Method = (method == NetworkMethod.GET ? "GET" : "POST");
            request.ContentType = "application/x-www-form-urlencoded";

            SetCookies(request);
            try
            {
                if (data != null)
                {
                    Task<Stream> taskStream = Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null);
                    taskStream.Wait(250);
                    using (Stream requestStream = taskStream.Result)
                    {
                        requestStream.Write(dataByte, 0, dataByte.Length);
                    }
                }

                Task<WebResponse> taskResponse = Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
                taskResponse.Wait(webTimeOut);
                response = taskResponse.Result as HttpWebResponse;
            }
            catch (Exception e)
            {
                if (e.InnerException is WebException && ignoreStatusCode.Contains(((e.InnerException as WebException).Response as HttpWebResponse).StatusCode))
                    response = (e.InnerException as WebException).Response as HttpWebResponse;
                else if (e is AggregateException)
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                else
                    ExceptionDispatchInfo.Capture(e).Throw();
            }
            GetCookies(response);
            return (response);
        }
        #endregion

        #region Cookies Manager
        private void SetCookies(WebRequest request)
        {
            if (request is HttpWebRequest)
                (request as HttpWebRequest).CookieContainer = cookies;
        }
        private void GetCookies(WebResponse response)
        {
            if (response is HttpWebResponse)
                foreach (Cookie cookie in (response as HttpWebResponse).Cookies)
                    cookies.Add(response.ResponseUri, cookie);
        }
        #endregion

        #region Connection Manager
        private bool ConnectWithOffice(Uri uri, String user, String password)
        {
            throw new NotImplementedException();
        }
        private bool ConnectWithUnix(Uri uri, String user, String password)
        {
            HttpWebResponse intraResponse = (HttpWebResponse)LoadUri(uri, NetworkMethod.POST, new Dictionary<String, Object>()
            {
                { "login", user },
                { "password", password }
            });
            return (intraResponse.StatusCode == HttpStatusCode.OK);
        }
        #endregion

        public Boolean IsConnected
        {
            get
            {
                return connected;
            }
        }
        public JToken DownloadJson(String uri)
        {
            HttpWebResponse jsonResponse = LoadUri(new Uri(uri), NetworkMethod.GET, null);
            string jsonContent = "";
            using (StreamReader sResponse = new StreamReader(jsonResponse.GetResponseStream(), Encoding.UTF8))
            {
                jsonContent = sResponse.ReadToEnd();
            }
            return (JToken.Parse(jsonContent));
        }
        public bool ConnectToAPI(ConnectionManager manager, String uri, String user, String password)
        {
            uri.ArgumentNotNull(nameof(uri));
            user.ArgumentNotNull(nameof(user));
            password.ArgumentNotNull(nameof(password));

            cookies = new CookieContainer();
            switch (manager)
            {
                case ConnectionManager.Office365:
                    connected = ConnectWithOffice(new Uri(uri), user, password);
                    break;
                case ConnectionManager.Classic:
                    connected = ConnectWithUnix(new Uri(uri), user, password);
                    break;
                default:
                    connected = false;
                    break;
            }
            return connected;
        }
    }
}
