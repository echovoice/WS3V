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
    /// http://ws3v.org/spec.json#send
    /// </summary>

    public class send
    {
        private const int id = 5;

        public string message_id { get; set; }
        public string method { get; set; }
        public string uri { get; set; }
        public string parameters { get; set; }

        public send()
        {
            message_id = string.Empty;
            method = string.Empty;
            uri = string.Empty;
            parameters = null;
        }

        public send(string[] message)
        {
            message_id = message[1];
            method = message[2];
            uri = message[3];

            if(message.Length == 5)
                parameters = message[4];
        }
    }
}
