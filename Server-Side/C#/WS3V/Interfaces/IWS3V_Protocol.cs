using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.Support;

namespace WS3V.Interfaces
{
    public interface IWS3V_Protocol
    {
        public Func<string, string> RPC;
        public Action<string> Pub;
        public Action<string> Sub;
        public Action<string> SocketSend;

        public string server;

        public Heartbeat heartbeat;
        public Filetransfer filetransfer;

        public int authentication_attempts;
        public int authentication_timeout;
        public bool recovery;
        public int recovery_timeout;
        public bool channel_listing;
        public string headers;
    }
}
