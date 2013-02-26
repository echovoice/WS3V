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
    /// http://ws3v.org/spec.json#recover
    /// </summary>

    public class recover
    {
        private const int id = 4;

        public string session_id { get; set; }

        public recover()
        {
            session_id = string.Empty;
        }

        public recover(string[] message)
        {
            session_id = message[1];
        }
    }
}
