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
    /// http://ws3v.org/spec.json#channels
    /// </summary>

    public class channels
    {
        private const int id = 8;

        public bool meta { get; set; }
        public string filter { get; set; }

        public channels()
        {
            meta = false;
            filter = null;
        }

        public channels(string[] message)
        {
            bool meta = false;
            if (message.Length > 1)
                bool.TryParse(message[1], out meta);

            this.meta = meta;

            if (message.Length > 2)
                filter = message[2];
        }
    }
}
