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
    /// http://ws3v.org/spec.json#howdy
    /// </summary>
    
    public class howdy
    {
        private const int id = 3;
        public const int protocol_version = 1;

        public string session_id { get; set; }
        public string server_information { get; set; }

        public _heartbeat heartbeat { get; set; }
        public _filetransfer filetransfer { get; set; }

        public bool recovery { get; set; }
        public int recovery_interval { get; set; }
        public bool channel_listing { get; set; }
        public string headers { get; set; }

        
        public howdy()
        {
            session_id = Guid.NewGuid().ToString();
            server_information = string.Empty;
            heartbeat = new _heartbeat();
            filetransfer = new _filetransfer();
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
            heartbeat = new _heartbeat();
            filetransfer = new _filetransfer();
            recovery = false;
            recovery_interval = 0;
            channel_listing = false;
            headers = null;
        }

        public howdy(string session_id, string server_information, int heartbeat_min_seconds, int heartbeat_max_seconds, bool allow_heartbeats_when_busy, int file_transfer_duplex_mode, int recovery_interval)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                this.session_id = Guid.NewGuid().ToString();
            else
                this.session_id = session_id;

            this.server_information = server_information;
            heartbeat = new _heartbeat(heartbeat_min_seconds, heartbeat_max_seconds, allow_heartbeats_when_busy);
            filetransfer = new _filetransfer(file_transfer_duplex_mode);
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

    public class _heartbeat
    {
        public int heartbeat_min_seconds { get; set; }
        public int heartbeat_max_seconds { get; set; }
        public bool allow_heartbeats_when_busy { get; set; }

        public _heartbeat()
        {
            heartbeat_min_seconds = -1;
            heartbeat_max_seconds = -1;
            allow_heartbeats_when_busy = false;
        }

        public _heartbeat(int heartbeat_min_seconds, int heartbeat_max_seconds, bool allow_heartbeats_when_busy)
        {
            this.heartbeat_min_seconds = heartbeat_min_seconds;
            this.heartbeat_max_seconds = heartbeat_max_seconds;
            this.allow_heartbeats_when_busy = allow_heartbeats_when_busy;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(heartbeat_min_seconds);
            sb.Append(',');
            sb.Append(heartbeat_max_seconds);
            sb.Append(',');
            sb.Append((allow_heartbeats_when_busy) ? "true" : "false");
            sb.Append(']');

            return sb.ToString();
        }
    }

    public class _filetransfer
    {
        public int file_transfer_duplex_mode { get; set; }
        public bool allow_pausing { get; set; }
        public bool force_chunk_integrity { get; set; }
        public bool force_file_integrity { get; set; }
        public string max_chunk_size { get; set; }
        public string max_file_size { get; set; }

        public long max_chunk_size_real
        {
            get
            {
                return shorthand(max_chunk_size);
            }
        }

        public long max_file_size_real
        {
            get
            {
                return shorthand(max_file_size);
            }
        }

        public _filetransfer()
        {
            file_transfer_duplex_mode = 0;
            allow_pausing = false;
            force_chunk_integrity = false;
            force_file_integrity = false;
            max_chunk_size = "0";
            max_file_size = "0";
        }

        public _filetransfer(int file_transfer_duplex_mode)
        {
            this.file_transfer_duplex_mode = file_transfer_duplex_mode;
        }

        public long shorthand(string input)
        {
            long value = 0;
            char ending = input[input.Length - 1];
            switch (ending)
            {
                case 'K':
                    long.TryParse(input.Substring(0, input.Length - 1), out value);
                    value = value * 1024;
                    break;

                case 'M':
                    long.TryParse(input.Substring(0, input.Length - 1), out value);
                    value = value * 1024 * 1024;
                    break;

                case 'G':
                    long.TryParse(input.Substring(0, input.Length - 1), out value);
                    value = value * 1024 * 1024 * 1024;
                    break;

                case 'T':
                    long.TryParse(input.Substring(0, input.Length - 1), out value);
                    value = value * 1024 * 1024 * 1024 * 1024;
                    break;

                default:
                    long.TryParse(input, out value);
                    break;
            }

            return value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('[');
            sb.Append(file_transfer_duplex_mode);
            sb.Append(',');
            sb.Append((allow_pausing) ? "true" : "false");
            sb.Append(',');
            sb.Append((force_chunk_integrity) ? "true" : "false");
            sb.Append(',');
            sb.Append((force_file_integrity) ? "true" : "false");
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(max_chunk_size));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(max_file_size));
            sb.Append(']');

            return sb.ToString();
        }
    }
}
