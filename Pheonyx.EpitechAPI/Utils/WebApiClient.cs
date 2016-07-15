﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Pheonyx.EpitechAPI.Utils
{
    public enum ConnectionManager { Office365, Classic }

    public sealed class WebApiClient
    {
        private enum NetworkMethod { Get, Post }

        private Boolean _connected;
        private String _webApiAgent;
        private CookieContainer _cookies;
        private readonly TimeSpan _webTimeOut;
        private readonly HttpStatusCode[] _ignoreStatusCode = { };

        public Boolean IsConnected => _connected;

        #region Constructor
        public WebApiClient(TimeSpan webTimeOut, String webApiAgent, HttpStatusCode[] ignoreStatusCode)
        {
            _webTimeOut = webTimeOut;
            _webApiAgent = webApiAgent;
            if (ignoreStatusCode != null)
                _ignoreStatusCode = ignoreStatusCode;
        }
        #endregion

        #region Networks
        private String GetStringData(Dictionary<String, Object> data)
        {
            if (data == null) return String.Empty;
            var dataString = data.Where(kv => !string.IsNullOrEmpty(kv.Key)).Aggregate("", (current, kv) => current + ("&" + kv.Key + "=" + kv.Value));
            return dataString.TrimStart('&');
        }
        private HttpWebResponse LoadUri(Uri uri, NetworkMethod method, Dictionary<String, Object> data)
        {
            var dataByte = Encoding.UTF8.GetBytes(GetStringData(data));
            var request = WebRequest.CreateHttp(uri);
            HttpWebResponse response = null;

            request.Method = method == NetworkMethod.Get ? "GET" : "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            SetCookies(request);
            try
            {
                if (data != null)
                {
                    var taskStream = Task.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
                    taskStream.Wait(250);
                    using (var requestStream = taskStream.Result)
                    {
                        requestStream.Write(dataByte, 0, dataByte.Length);
                    }
                }

                var taskResponse = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
                taskResponse.Wait(_webTimeOut);
                response = taskResponse.Result as HttpWebResponse;
            }
            catch (Exception e)
            {
                if ((e.InnerException as WebException)?.Response is HttpWebResponse &&
                    _ignoreStatusCode.Contains(((HttpWebResponse) ((WebException) e.InnerException).Response).StatusCode))
                    response = ((WebException) e.InnerException).Response as HttpWebResponse;
                else if (e is AggregateException)
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                else
                    ExceptionDispatchInfo.Capture(e).Throw();
            }
            GetCookies(response);
            return response;
        }
        #endregion

        #region Cookies Manager
        private void SetCookies(WebRequest request)
        {
            if (request is HttpWebRequest)
                ((HttpWebRequest) request).CookieContainer = _cookies;
        }
        private void GetCookies(WebResponse response)
        {
            if (response is HttpWebResponse)
                foreach (Cookie cookie in ((HttpWebResponse) response).Cookies)
                    _cookies.Add(response.ResponseUri, cookie);
        }
        #endregion

        #region Connection Manager
        private bool ConnectWithOffice(Uri uri, String user, String password)
        {
            throw new NotImplementedException();
        }
        private bool ConnectWithUnix(Uri uri, String user, String password)
        {
            var intraResponse = LoadUri(uri, NetworkMethod.Post, new Dictionary<String, Object>
            {
                { "login", user },
                { "password", password }
            });
            return intraResponse.StatusCode == HttpStatusCode.OK;
        }
        #endregion

        public JToken DownloadJson(String uri)
        {
            var jsonResponse = LoadUri(new Uri(uri), NetworkMethod.Get, null);
            var jsonContent = "";
            using (var sResponse = new StreamReader(jsonResponse.GetResponseStream(), Encoding.UTF8))
            {
                jsonContent = sResponse.ReadToEnd();
            }
            return JToken.Parse(jsonContent);
        }
        public Boolean ConnectToApi(ConnectionManager manager, String uri, String user, String password)
        {
            uri.ArgumentNotNull(nameof(uri));
            user.ArgumentNotNull(nameof(user));
            password.ArgumentNotNull(nameof(password));

            _cookies = new CookieContainer();
            switch (manager)
            {
                case ConnectionManager.Office365:
                    _connected = ConnectWithOffice(new Uri(uri), user, password);
                    break;
                case ConnectionManager.Classic:
                    _connected = ConnectWithUnix(new Uri(uri), user, password);
                    break;
                default:
                    _connected = false;
                    break;
            }
            return _connected;
        }
    }
}
