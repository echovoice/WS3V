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
    /// http://ws3v.org/spec.json#event
    /// </summary>
    
    public class _event
    {
        private const int id = 16;

        public string channel_name_or_uri { get; set; }
        public PubSub_Event p { get; set; }

        public _event(string channel_name_or_uri, PubSub_Event p)
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
            sb.Append((p.message[0] == '{') ? p.message : JSONEncoders.EncodeJsString(p.message));
            sb.Append(',');
            sb.Append(p.timestamp);
            sb.Append(']');

            return sb.ToString();
        }
    }
}
