using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.Support
{
    public class RPC_Exception : Exception
    {
        public int code { get; set; }
        public string message { get; set; }
        public string url { get; set; }

        public RPC_Exception()
        {
            code = 0;
            message = string.Empty;
            url = string.Empty;
        }

        public RPC_Exception(int code)
        {
            this.code = code;
            message = string.Empty;
            url = string.Empty;
        }

        public RPC_Exception(int code, string message)
        {
            this.code = code;
            this.message = message;
            url = string.Empty;
        }

        public RPC_Exception(int code, string message, string url)
        {
            this.code = code;
            this.message = message;
            this.url = url;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(code);

            if (message != null)
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsString(message));

                if (url != null)
                {
                    sb.Append(',');
                    sb.Append(JSONEncoders.EncodeJsString(url));
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
