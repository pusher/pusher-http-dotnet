using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace PusherServer.RestfulClient
{
    /// <summary>
    /// Factory that creates authenticated requests to send to the Pusher API
    /// </summary>
    public class AuthenticatedRequestFactory : IAuthenticatedRequestFactory
    {
        private readonly string _appKey;
        private readonly string _appId;
        private readonly string _appSecret;

        /// <summary>
        /// Constructs a new Autheticated Request Factory
        /// </summary>
        /// <param name="appKey">Your app Key for the Pusher API</param>
        /// <param name="appId">Your app Id for the Pusher API</param>
        /// <param name="appSecret">Your app Secret for the Pusher API</param>
        public AuthenticatedRequestFactory(string appKey, string appId, string appSecret)
        {
            _appKey = appKey;
            _appId = appId;
            _appSecret = appSecret;
        }

        /// <summary>
        /// Pusher library version information.
        /// </summary>
        public static Version VERSION => typeof(AuthenticatedRequestFactory).GetTypeInfo().Assembly.GetName().Version;

        /// <summary>
        /// The Pusher library name.
        /// </summary>
        public static String LIBRARY_NAME
        {
            get
            {
                Attribute attr = typeof(Pusher).GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));

                AssemblyProductAttribute adAttr = (AssemblyProductAttribute)attr;

                return adAttr.Product;
            }
        }

        /// <inheritdoc/>
        public IPusherRestRequest Build(PusherMethod requestType, string resource, object requestParameters = null, object requestBody = null)
        {
            string queryString = GetQueryString(requestParameters, requestBody);

            string path = $"/apps/{_appId}/{resource.TrimStart('/')}";

            string authToSign = $"{Enum.GetName(requestType.GetType(), requestType)}\n{path}\n{queryString}";

            string authSignature = CryptoHelper.GetHmac256(_appSecret, authToSign);

            string requestUrl = $"{path}?{queryString}&auth_signature={authSignature}";

            IPusherRestRequest request = new PusherRestRequest(requestUrl);
            request.Method = requestType;
            request.Body = requestBody;

            return request;
        }

        private string GetQueryString(object requestParameters, object requestBody)
        {
            StringBuilder stringBuilder = GetStringBuilderfromSourceObject(requestParameters);

            int timeNow = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            stringBuilder.Append($"auth_key={_appKey}&auth_timestamp={timeNow}&auth_version=1.0");

            if (requestBody != null)
            {
                var bodyDataJson = JsonConvert.SerializeObject(requestBody);
                var bodyMd5 = CryptoHelper.GetMd5Hash(bodyDataJson);
                stringBuilder.Append($"&body_md5={bodyMd5}");
            }
            
            return stringBuilder.ToString();
        }

        private static StringBuilder GetStringBuilderfromSourceObject(object sourceObject)
        {
            StringBuilder stringBuider = new StringBuilder();

            if (sourceObject != null)
            {
                Type objType = sourceObject.GetType();
                IList<PropertyInfo> propertyInfos = new List<PropertyInfo>(objType.GetProperties());

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    stringBuider.Append($"{propertyInfo.Name}={propertyInfo.GetValue(sourceObject, null)}&");
                }
            }

            return stringBuider;
        }
    }
}