﻿using System;
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
        public int participants { get; set; }

        public Room()
        {
            name = string.Empty;
            description = string.Empty;
            participants = 0;
        }

        public Room(string name, string description)
        {
            this.name = name;
            this.description = description;
            participants = 0;
        }

        public Room(string _data)
        {
            string[] data = JSONDecoders.DecodeJSONArray(_data);

            name = data[0];
            description = data[1];

            int _participants = 0;

            int.TryParse(data[2], out _participants);

            participants = _participants;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(JSONEncoders.EncodeJsString(name));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(description));
            sb.Append(',');
            sb.Append(participants);
            sb.Append(']');
            return sb.ToString();
        }
    }
}
