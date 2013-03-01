using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS3V.Support
{
    public class PubSub_Channel
    {
        public string channel_name_or_uri { get; set; }
        public string channel_meta { get; set; }

        public PubSub_Channel(string channel_name_or_uri)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            channel_meta = null;
        }

        public PubSub_Channel(string channel_name_or_uri, string channel_meta)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            this.channel_meta = channel_meta;
        }
    }
}
