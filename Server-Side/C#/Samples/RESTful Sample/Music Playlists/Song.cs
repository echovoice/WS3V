using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace RESTful_Sample.Music_Playlists
{
    public class Song
    {
        public string artist { get; set; }
        public string album { get; set; }
        public string title { get; set; }
        public string art { get; set; }

        public Song(string artist, string album, string title, string art)
        {
            this.artist = artist;
            this.album = album;
            this.title = title;
            this.art = art;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(JSONEncoders.EncodeJsString(artist));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(album));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(title));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(art));
            sb.Append(']');
            return sb.ToString();
        }
    }
}
