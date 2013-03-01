using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS3V.Support
{
    public class PubSub_Core
    {
        public List<PubSub_Channel> channels { get; set; }

        public PubSub_Core()
        {
            channels = new List<PubSub_Channel>();
        }

        public void CreateChannel(string channel_name_or_uri)
        {
            channels.Add(new PubSub_Channel(channel_name_or_uri));
        }

        public void CreateChannel(string channel_name_or_uri, string channel_meta)
        {
            channels.Add(new PubSub_Channel(channel_name_or_uri, channel_meta));
        }

        public void RemoveChannel(string channel_name_or_uri)
        {
            channels.RemoveAll(c => c.channel_name_or_uri == channel_name_or_uri);
        }

        public string[] GetChannels()
        {
            return channels.Select(c => c.channel_name_or_uri).ToArray();
        }

        public bool HasMeta()
        {
            return channels.Any(c => c.channel_meta != null);
        }

        public string[] GetMeta()
        {
            return channels.Select(c => c.channel_meta).ToArray();
        }
    }
}
