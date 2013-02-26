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
    /// http://ws3v.org/spec.json#signature
    /// </summary>

    public class signature
    {
        private const int id = 2;

        public string[] credentials { get; set; }

        public signature()
        {
            credentials = new string[0];
        }

        public signature (string[] message)
        {
            credentials = JSONDecoders.DecodeJsStringArray(message[1]);
        }
    }
}
