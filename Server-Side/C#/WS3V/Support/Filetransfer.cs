using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.Support
{
    /// <summary>
    /// based on ws3v documentation example: http://ws3v.org/spec.json#howdy
    /// by default file transfer is disabled
    /// </summary>
    
    public class Filetransfer
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

        public Filetransfer()
        {
            file_transfer_duplex_mode = 0;
            allow_pausing = false;
            force_chunk_integrity = false;
            force_file_integrity = false;
            max_chunk_size = "0";
            max_file_size = "0";
        }

        public Filetransfer(int file_transfer_duplex_mode)
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
            if (max_chunk_size != null)
            {
                sb.Append(',');
                sb.Append(JSONEncoders.EncodeJsString(max_chunk_size));
                if (max_file_size != null)
                {
                    sb.Append(',');
                    sb.Append(JSONEncoders.EncodeJsString(max_file_size));
                }
            }
            sb.Append(']');

            return sb.ToString();
        }
    }
}
