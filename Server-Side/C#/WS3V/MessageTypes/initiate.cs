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
    /// http://ws3v.org/spec.json#initiate
    /// </summary>

    public class initiate
    {
        private const int id = 17;

        public string file_transfer_id { get; set; }
        public string filename { get; set; }
        public string content_media_type { get; set; }
        public string size { get; set; }
        public string encoding_scheme { get; set; }
        public string hash { get; set; }
        public string hash_type { get; set; }

        public initiate()
        {
            session_id = string.Empty;
        }

        public recover(string[] message)
        {
            session_id = message[1];
        }
    }
}
