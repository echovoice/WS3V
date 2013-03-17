using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.Support;

namespace WS3V.Interfaces
{
    public interface IWS3V_Protocol
    {
        ConcurrentDictionary<Guid, WS3V_Client> WS3V_Clients { get; set; }

        Func<RPC_Incoming, RPC_Outgoing> RPC { get; set; }
        Action<string> Subscribe { get; set; }
        Action<string> Pub { get; set; }
        Action<string> Sub { get; set; }
        Action<string> SocketSend { get; set; }
        Action Dispose { get; set; }

        string server { get; set; }

        string clientID { get; set; }

        Heartbeat heartbeat { get; set; }
        Filetransfer filetransfer { get; set; }
        PubSub_Listing pubsub { get; set; }

        int authentication_attempts { get; set; }
        int authentication_timeout { get; set; }
        bool recovery { get; set; }
        int recovery_timeout { get; set; }
        bool channel_listing { get; set; }
        string headers { get; set; }

        string[] credentials { get; set; }
        Func<string[], bool> Authenticate { get; set; }
    }
}
