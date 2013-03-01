using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;
using WS3V.Support;

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
        public RPC_Exception e { get; set; }
        public string headers { get; set; }

        public error()
        {
            message_id = string.Empty;
            e = new RPC_Exception();
            headers = null;
        }

        public error(string message_id, RPC_Exception e)
        {
            this.message_id = message_id;
            this.e = e;
            headers = null;
        }

        public error(string message_id, RPC_Exception e, string headers)
        {
            this.message_id = message_id;
            this.e = e;
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
            sb.Append(e.ToString());
            sb.Append(',');
            sb.Append(headers);
            sb.Append(']');

            return sb.ToString();
        }
    }
}
