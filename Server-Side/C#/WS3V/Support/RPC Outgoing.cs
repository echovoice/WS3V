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

        public RPC_Outgoing()
        {
            response = string.Empty;
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
