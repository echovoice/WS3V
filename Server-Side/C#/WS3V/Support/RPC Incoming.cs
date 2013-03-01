using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS3V.Support
{
    public class RPC_Incoming
    {
        public string method { get; set; }
        public string uri { get; set; }
        public string parameters { get; set; }

        public RPC_Incoming()
        {
            method = string.Empty;
            uri = string.Empty;
            parameters = string.Empty;
        }

        public RPC_Incoming(string[] message)
        {
            if (message.Length > 2)
                method = message[2];
            else
                method = string.Empty;

            if (message.Length > 3)
                uri = message[3];
            else
                uri = string.Empty;

            if (message.Length > 4)
                parameters = message[4];
            else
                parameters = string.Empty;
        }
    }
}
