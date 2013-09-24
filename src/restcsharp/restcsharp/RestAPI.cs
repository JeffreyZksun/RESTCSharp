using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace restcsharp
{
    public class RestAPI
    {
        public RestAPI()
        {
            // Set the default value
            APIEndPoint = "http://127.0.0.1:3000";
            BaseURL = "/api/1.0";
            AuthoId = "";
        }
        public String APIEndPoint { get; set; } // http://127.0.0.1:3000
        public String BaseURL {get; set;} // /api/1.0
        public String AuthoId { get; set; } // 21XW2DS254

        public bool SendHttpRequest(string uri, string method, JObject bodyObject, out JObject responseObject)
        {
            responseObject = null;
            JContainer response;
            bool ret = _SendHttpRequest(uri, method, bodyObject, out response);
            if (ret && response != null)
            {
                responseObject = response as JObject;
            }

            return ret;
        }

        public bool SendHttpRequest(string uri, string method, JObject bodyObject, out JArray responseObject)
        {
            responseObject = null;
            JContainer response;
            bool ret = _SendHttpRequest(uri, method, bodyObject, out response);
            if (ret && response != null)
            {
                responseObject = response as JArray;
            }

            return ret;
        }

        /**
         * @param uri
         * @param method Case-insensitive
         * @param bodyObject 
         * @param responseObject output, the value would be JObject or JArray        
         */
        private bool _SendHttpRequest(string uri, string method, JObject bodyObject, out JContainer responseObject)
        {
            responseObject = null;
            var url = APIEndPoint + BaseURL + @"/" + uri;

            // Set remote url and http method.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            // Set the content type header and http body.
            if(bodyObject != null){
                var body = JsonConvert.SerializeObject(bodyObject);
                if (body.Length != 0)
                {
                    request.ContentType = "application/json";
                    using (Stream requestStream = request.GetRequestStream())
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    {
                        writer.Write(body);
                    }
                }
            }

            // Set the authorization id.
            string sAuth = "auth_id=" + AuthoId;
            request.Headers.Add("Authorization", sAuth);

            string responseData = "";
            try
            {
                // Send request
                System.Net.WebResponse response = (HttpWebResponse)request.GetResponse();
                responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                if (response != null)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    response.Close();   // Releases the resources of the response.
                }
            }
            catch (Exception)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(responseData))
            {
                var res = JsonConvert.DeserializeObject(responseData);
                responseObject = res as JContainer;
            }

            return true;
        }
    }
}
