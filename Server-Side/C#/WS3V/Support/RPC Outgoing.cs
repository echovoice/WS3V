using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.Support
{
    public class RPC_Outgoing
    {
        public string response { get; set; }
        public DateTime expires { get; set; }

        public RPC_Outgoing()
        {
            response = string.Empty;
            expires = DateTime.MinValue;
        }

        public RPC_Outgoing(string response, DateTime expires)
        {
            this.response = JSONEncoders.EncodeJsString(response);
            this.expires = expires;
        }

        public RPC_Outgoing(object response, DateTime expires)
        {
            this.response = response.ToString();
            this.expires = expires;
        }

        public RPC_Outgoing(string response)
        {
            this.response = JSONEncoders.EncodeJsString(response);
        }

        public RPC_Outgoing(object response)
        {
            this.response = response.ToString();
        }
    }
}
