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
    /// http://ws3v.org/spec.json#subscribe
    /// </summary>

    public class subscribe
    {
        private const int id = 10;

        public string[] channel_name_or_uri { get; set; }
        public string[] filter { get; set; }

        public subscribe(string[] message)
        {
            if (message.Length > 1)
                channel_name_or_uri = JSONDecoders.DecodeJSONArray(message[1]);

            if (message.Length > 2)
                filter = JSONDecoders.DecodeJSONArray(message[2]);

            else
                filter = null;
        }
    }
}
