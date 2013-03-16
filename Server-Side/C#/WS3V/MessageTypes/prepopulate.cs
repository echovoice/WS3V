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
    /// http://ws3v.org/spec.json#prepopulate
    /// </summary>

    public class prepopulate
    {
        private const int id = 12;

        public string channel_name_or_uri { get; set; }
        public int count { get; set; }
        public int timestamp { get; set; }

        public prepopulate(string[] message)
        {
            int _count = 0;
            int _timestamp = 0;

            if (message.Length > 1)
                channel_name_or_uri = message[1];

            if (message.Length > 2)
                int.TryParse(message[2], out _count);

            if (message.Length > 3)
                int.TryParse(message[3], out _timestamp);

            count = _count;
            timestamp = _timestamp;
        }
    }
}
