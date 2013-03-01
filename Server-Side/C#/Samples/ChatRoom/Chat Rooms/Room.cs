using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace Chat_Room_Sample.Chat_Rooms
{
    public class Room
    {
        public string name { get; set; }
        public string description { get; set; }
        public int partiapants { get; set; }

        public Room()
        {
            name = string.Empty;
            description = string.Empty;
            partiapants = 0;
        }

        public Room(string name, string description)
        {
            this.name = name;
            this.description = description;
            partiapants = 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(JSONEncoders.EncodeJsString(name));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(description));
            sb.Append(',');
            sb.Append(partiapants);
            sb.Append(']');
            return sb.ToString();
        }
    }
}
