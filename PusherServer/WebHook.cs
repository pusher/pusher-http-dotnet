using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace PusherServer
{
    internal class WebHook: IWebHook
    {
        private WebHookData _webHookData;
        private List<string> _validationErrors;

        internal WebHook(string secret, string signature, string body)
        {
            if(string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("A secret must be provided", "secret");
            }

            this._validationErrors = new List<string>();

            this._webHookData = ValidateWebHook(secret, signature, body);
        }

        private WebHookData ValidateWebHook(string secret, string signature, string body)
        {
            WebHookData parsedWebHookData = null;

            var signatureNullOrEmpth = string.IsNullOrEmpty(signature);
            if(signatureNullOrEmpth == true) {
                this._validationErrors.Add("The supplied signature to check was null or empty. A signature to check must be provided.");
            }

            if (string.IsNullOrEmpty(body) == true)
            {
                this._validationErrors.Add("The supplied body to check was null or empty. A body to check must be provided.");
            }
            else
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    parsedWebHookData = serializer.Deserialize<WebHookData>(body);
                }
                catch (Exception e)
                {
                    var validationError = string.Format("Exception occurred parsing the body as JSON: {0}{1}", Environment.NewLine, e.StackTrace);
                    this._validationErrors.Add(validationError);
                }
            }

            if (parsedWebHookData != null)
            {
                var expectedSignature = CryptoHelper.GetHmac256(secret, body);
                var signatureIsValid = (signature == expectedSignature);
                if (signatureIsValid == false)
                {
                    this._validationErrors.Add(
                        string.Format("The signature did not validate. Expected {0}. Got {1}", signature, expectedSignature)
                    );
                }
            }
            return parsedWebHookData;
        }

        public bool IsValid
        {
            get
            {
                return (this.ValidationErrors.Length == 0);
            }
        }

        public Dictionary<string, string>[] Events
        {
            get
            {
                return this._webHookData.events;
            }
        }

        public DateTime Time
        {
            get
            {
                return this._webHookData.Time;
            }
        }

        public string[] ValidationErrors
        {
            get
            {
                return this._validationErrors.ToArray<string>();
            }
        }

    }

}
