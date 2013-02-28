using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.Interfaces;
using WS3V.Support;

namespace WS3V
{
    /// <summary>
    /// Built from WS3V Specifications v1
    /// http://ws3v.org/spec.json
    /// </summary>
    
    public class WS3V_Protocol : IWS3V_Protocol
    {
        public Func<string, string> RPC { get; set; }
        public Action<string> Pub { get; set; }
        public Action<string> Sub { get; set; }
        public Action<string> SocketSend { get; set; }

        public string server { get; set; }

        public Heartbeat heartbeat { get; set; }
        public Filetransfer filetransfer { get; set; }

        public int authentication_attempts { get; set; }
        public int authentication_timeout { get; set; }
        public bool recovery { get; set; }
        public int recovery_timeout { get; set; }
        public bool channel_listing { get; set; }
        public string headers  { get; set; }

        private Action<IWS3V_Protocol> config;

        public WS3V_Protocol(Action<IWS3V_Protocol> _config)
        {
            RPC = x => { return x; };
            Pub = x => { };
            Sub = x => { };
            SocketSend = x => { };

            server = string.Empty;
            heartbeat = new Heartbeat();
            filetransfer = new Filetransfer();

            authentication_attempts = 3;
            authentication_timeout = 5;
            recovery = false;
            recovery_timeout = 0;
            channel_listing = false;
            headers = null;

            config = _config;
            config(this);
        }
    }
}
