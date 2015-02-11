using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace PusherServer
{
    internal class WebHook: IWebHook
    {
        private string secret;
        private string signature;
        private string contentType;
        private string body;

        internal WebHook(string secret, string signature, string contentType, string body)
        {
            if(string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("A secret must be provided", "secret");
            }

            this.secret = secret;
            this.signature = signature;
            this.contentType = contentType;
            this.body = body;

        }

        public bool IsValid
        {
            get
            {
                return ( string.IsNullOrEmpty(this.secret) == false &&
                    this.IsSignatureValid &&
                    this.IsContentTypeValid &&
                    this.IsBodyValid );
            }
        }

        internal bool IsSignatureValid
        {
            get
            {
                return string.IsNullOrEmpty(this.signature) == false &&
                    SignatureMatchesExpected;
            }
        }

        internal bool IsBodyValid
        {
            get
            {
                return string.IsNullOrEmpty(this.body) == false &&
                    this.Data != null;
            }
        }

        public Dictionary<string, string>[] Events
        {
            get
            {
                return this.Data.events;
            }
        }

        private WebHookData Data
        {
            get {
                WebHookData data = null;

                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    data = serializer.Deserialize<WebHookData>(this.body);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }

                return data;
            }
        }

        // TODO: parse to DateTime
        public string Time
        {
            get { return this.Data.time_ms; }
        }

        public bool SignatureMatchesExpected
        {
            get
            {
                return ( string.IsNullOrEmpty(this.body) == false &&
                         this.signature == CryptoHelper.GetHmac256(this.secret, this.body) );
            }
        }

        public bool IsContentTypeValid
        {
            get
            {
                return this.contentType == "application/json";
            }
        }
    }

}
