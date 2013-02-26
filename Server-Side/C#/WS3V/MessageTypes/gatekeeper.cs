using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.MessageTypes
{
    /// <summary>
    /// Built from WS3V Specifications v1
    /// http://ws3v.org/spec.json#gatekeeper
    /// </summary>
    
    public class gatekeeper
    {
        private const int id = 1;

        public string[] credentials { get; set; }
        public int seconds { get; set; }
        public int retries { get; set; }
        public int error_code { get; set; }
        public string error_message { get; set; }
        public string error_url { get; set; }

        public gatekeeper()
        {
            credentials = new string[0];
            seconds = 0;
            retries = 3;
            error_code = 0;
            error_message = string.Empty;
            error_url = string.Empty;
        }

        public gatekeeper(string credentials, int seconds, int retries, int error_code = 0, string error_message = "", string error_url = "")
        {
            this.credentials = new string[] { credentials };
            this.seconds = seconds;
            this.retries = retries;
            this.error_code = error_code;
            this.error_message = error_message;
            this.error_url = error_url;
        }

        public gatekeeper(string[] credentials, int seconds, int retries, int error_code = 0, string error_message = "", string error_url = "")
        {
            this.credentials = credentials;
            this.seconds = seconds;
            this.retries = retries;
            this.error_code = error_code;
            this.error_message = error_message;
            this.error_url = error_url;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsStringArray(credentials));
            sb.Append(',');
            sb.Append(seconds);
            sb.Append(',');
            sb.Append(retries);
            sb.Append(',');
            sb.Append(error_code);

            if (!string.IsNullOrWhiteSpace(error_message))
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsString(error_message));

                if (!string.IsNullOrWhiteSpace(error_url))
                {
                    sb.Append(',');
                    sb.Append(JSONEncoders.EncodeJsString(error_url));
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
