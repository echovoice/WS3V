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
    /// http://ws3v.org/spec.json#receive
    /// </summary>
    
    public class receive
    {
        private const int id = 6;

        public string message_id { get; set; }
        public string response { get; set; }
        public string headers { get; set; }

        public receive()
        {
            message_id = string.Empty;
            response = string.Empty;
            headers = null;
        }

        public receive(string message_id, string response)
        {
            this.message_id = message_id;
            this.response = response;
            headers = null;
        }

        public receive(string message_id, string response, string headers)
        {
            this.message_id = message_id;
            this.response = response;
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
            sb.Append(response);

            if (!string.IsNullOrWhiteSpace(headers))
            {
                sb.Append(',');
                sb.Append(headers);
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
