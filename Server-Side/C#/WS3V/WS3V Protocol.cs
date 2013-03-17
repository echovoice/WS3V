using System;
using System.Collections.Concurrent;
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
        public ConcurrentDictionary<Guid, WS3V_Client> WS3V_Clients { get; set; }

        public Func<RPC_Incoming, RPC_Outgoing> RPC { get; set; }
        public Action<string> Subscribe { get; set; }
        public Action<string> Pub { get; set; }
        public Action<string> Sub { get; set; }
        public Action<string> SocketSend { get; set; }
        public Action Dispose { get; set; }

        public string server { get; set; }

        public string clientID { get; set; }

        public Heartbeat heartbeat { get; set; }
        public Filetransfer filetransfer { get; set; }
        public PubSub_Listing pubsub { get; set; }

        public int authentication_attempts { get; set; }
        public int authentication_timeout { get; set; }
        public bool recovery { get; set; }
        public int recovery_timeout { get; set; }
        public bool channel_listing { get; set; }
        public string headers  { get; set; }

        public string[] credentials { get; set; }
        public Func<string[], bool> Authenticate { get; set; }

        private Action<IWS3V_Protocol> config;

        public WS3V_Protocol(Action<IWS3V_Protocol> _config)
        {
            WS3V_Clients = null;

            RPC = x => { return new RPC_Outgoing(); };
            Subscribe = x => { };
            Pub = x => { };
            Sub = x => { };
            SocketSend = x => { };

            server = string.Empty;
            clientID = string.Empty;
            heartbeat = new Heartbeat();
            filetransfer = new Filetransfer();
            pubsub = null;

            authentication_attempts = 3;
            authentication_timeout = 5;
            recovery = false;
            recovery_timeout = 0;
            channel_listing = false;
            headers = null;

            credentials = null;
            Authenticate = x => { return true; };

            config = _config;
            config(this);
        }
    }
}
