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
    /// http://ws3v.org/spec.json#unsubscribe
    /// </summary>

    public class unsubscribe
    {
        private const int id = 14;

        public string channel_name_or_uri { get; set; }

        public unsubscribe(string[] message)
        {
            if (message.Length > 1)
                channel_name_or_uri = message[1];
        }
    }
}
