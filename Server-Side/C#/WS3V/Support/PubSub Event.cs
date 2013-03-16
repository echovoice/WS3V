using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.Support
{
    public class PubSub_Event
    {
        public string message { get; set; }
        public int timestamp { get; set; }

        public PubSub_Event(string message)
        {
            this.message = message;
            timestamp = _3vEpoch.getEVTime();
        }

        public PubSub_Event(string message, int timestamp)
        {
            this.message = message;
            this.timestamp = timestamp;
        }
    }
}
