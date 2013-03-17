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
    /// http://ws3v.org/spec.json#deny
    /// </summary>
    
    public class deny
    {
        private const int id = 13;

        public string channel_name_or_uri { get; set; }
        public PubSub_Exception p { get; set; }

        public deny()
        {
            channel_name_or_uri = string.Empty;
            p = new PubSub_Exception();
        }

        public deny(string channel_name_or_uri, PubSub_Exception p)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            this.p = p;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(channel_name_or_uri));
            sb.Append(',');
            sb.Append(p.ToString());
            sb.Append(']');

            return sb.ToString();
        }
    }
}
