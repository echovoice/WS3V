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
    /// http://ws3v.org/spec.json#terminated
    /// </summary>
    
    public class terminated
    {
        private const int id = 26;

        public int error_code { get; set; }
        public string error_message { get; set; }
        public string error_url { get; set; }

        public terminated()
        {
            error_code = 0;
            error_message = null;
            error_url = null;
        }

        public terminated(int error_code)
        {
            this.error_code = error_code;
            error_message = null;
            error_url = null;
        }

        public terminated(int error_code, string error_message)
        {
            this.error_code = error_code;
            this.error_message = error_message;
            error_url = null;
        }

        public terminated(int error_code, string error_message, string error_url)
        {
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
            sb.Append(error_code);

            if (error_message != null)
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsString(error_message));

                if (error_url != null)
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
