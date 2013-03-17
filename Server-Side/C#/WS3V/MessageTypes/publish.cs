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
    /// http://ws3v.org/spec.json#publish
    /// </summary>

    public class publish
    {
        private const int id = 15;

        public string channel_name_or_uri { get; set; }
        public string message { get; set; }
        public bool echo { get; set; }

        public publish()
        {
            echo = false;
            channel_name_or_uri = null;
            message = null;
        }

        public publish(string[] message)
        {
            bool _echo = false;

            if (message.Length > 1)
                channel_name_or_uri = message[1];

            if (message.Length > 2)
                this.message = message[2];

            if (message.Length > 3)
                bool.TryParse(message[3], out _echo);

            echo = _echo;
        }
    }
}
