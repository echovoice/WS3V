using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;
using WS3V.Support;

namespace WS3V.MessageTypes
{
    /// <summary>
    /// Built from WS3V Specifications v1
    /// http://ws3v.org/spec.json#howdy
    /// </summary>
    
    public class howdy
    {
        private const int id = 3;
        public const int protocol_version = 1;

        public string session_id { get; set; }
        public string server_information { get; set; }

        public Heartbeat heartbeat { get; set; }
        public Filetransfer filetransfer { get; set; }

        public bool recovery { get; set; }
        public int recovery_interval { get; set; }
        public bool channel_listing { get; set; }
        public string headers { get; set; }

        
        public howdy()
        {
            session_id = Guid.NewGuid().ToString();
            server_information = string.Empty;
            heartbeat = new Heartbeat();
            filetransfer = new Filetransfer();
            recovery = false;
            recovery_interval = 0;
            channel_listing = false;
            headers = null;
        }

        public howdy(string session_id)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                this.session_id = Guid.NewGuid().ToString();
            else
                this.session_id = session_id;

            server_information = string.Empty;
            heartbeat = new Heartbeat();
            filetransfer = new Filetransfer();
            recovery = false;
            recovery_interval = 0;
            channel_listing = false;
            headers = null;
        }

        public howdy(string session_id, string server_information, int recovery_interval, Heartbeat heartbeat, Filetransfer filetransfer)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                this.session_id = Guid.NewGuid().ToString();
            else
                this.session_id = session_id;

            this.server_information = server_information;
            this.heartbeat = heartbeat;
            this.filetransfer = filetransfer;
            this.recovery_interval = recovery_interval;

            recovery = false;
            channel_listing = false;
            headers = null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(id);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(session_id));
            sb.Append(',');
            sb.Append(protocol_version);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(server_information));

            sb.Append(',');
            sb.Append(heartbeat.ToString());

            sb.Append(',');
            sb.Append(filetransfer.ToString());

            sb.Append(',');
            sb.Append((recovery) ? "true" : "false");
            sb.Append(',');
            sb.Append((channel_listing) ? "true" : "false");
            sb.Append(',');
            sb.Append(recovery_interval);

            if (headers != null)
            {
                sb.Append(',');
                sb.Append(headers);
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
