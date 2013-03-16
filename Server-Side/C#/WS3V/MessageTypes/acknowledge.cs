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
    /// http://ws3v.org/spec.json#acknowledge
    /// </summary>
    
    public class acknowledge
    {
        private const int id = 11;

        public PubSub_Channel channel { get; set; }
        public bool allow_publishing { get; set; }

        public acknowledge(string channel_name_or_uri)
        {
            channel = new PubSub_Channel(channel_name_or_uri);
        }

        public acknowledge(PubSub_Channel channel)
        {
            this.channel = channel;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(channel.channel_name_or_uri));

            if (allow_publishing || channel.historical)
            {
                sb.Append(',');
                sb.Append((allow_publishing) ? "true" : "false");

                if (channel.historical)
                {
                    sb.Append(',');
                    sb.Append(channel.number_of_events);
                    sb.Append(',');
                    sb.Append(channel.oldest_event);
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
