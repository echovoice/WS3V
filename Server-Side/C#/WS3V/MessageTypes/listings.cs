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
    /// http://ws3v.org/spec.json#listings
    /// </summary>
    
    public class listings
    {
        private const int id = 9;

        public PubSub_Core core { get; set; }
        
        public listings()
        {
            core = new PubSub_Core();
        }

        public listings(PubSub_Core core)
        {
            this.core = core;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsStringArray(core.GetChannels()));

            if (core.HasMeta())
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsStringArray(core.GetMeta()));
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
