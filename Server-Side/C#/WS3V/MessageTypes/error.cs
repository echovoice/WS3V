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
    /// http://ws3v.org/spec.json#error
    /// </summary>
    
    public class error
    {
        private const int id = 7;

        public string message_id { get; set; }
        public int error_code { get; set; }
        public string error_message { get; set; }
        public string error_url { get; set; }
        public string headers { get; set; }

        public error()
        {
            message_id = string.Empty;
            error_code = 0;
            error_message = null;
            error_url = null;
            headers = null;
        }

        public error(string message_id, int error_code)
        {
            this.message_id = message_id;
            this.error_code = error_code;
            error_message = null;
            error_url = null;
            headers = null;
        }

        public error(string message_id, int error_code, string headers)
        {
            this.message_id = message_id;
            this.error_code = error_code;
            error_message = string.Empty;
            error_url = string.Empty;
            this.headers = headers;
        }

        public error(string message_id, int error_code, string error_message, string error_url)
        {
            this.message_id = message_id;
            this.error_code = error_code;
            this.error_message = error_message;
            this.error_url = error_url;
            headers = null;
        }

        public error(string message_id, int error_code, string error_message, string error_url, string headers)
        {
            this.message_id = message_id;
            this.error_code = error_code;
            this.error_message = error_message;
            this.error_url = error_url;
            this.headers = headers;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(message_id));
            sb.Append(',');
            sb.Append(error_code);

            if (error_message != null)
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsString(error_message));

                if (error_url != null)
                {
                    sb.Append(',');
                    sb.Append(JSONEncoders.EncodeJsString(error_url));

                    if (headers != null)
                    {
                        sb.Append(',');
                        sb.Append(headers);
                    }
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
