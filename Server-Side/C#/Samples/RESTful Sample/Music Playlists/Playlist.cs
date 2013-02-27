using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace RESTful_Sample.Music_Playlists
{
    public class Playlist
    {
        public List<Song> songs { get; set; }
        public string name {get; set; }
        public string description {get; set; }

        public Playlist(string name, string description)
        {
            songs = new List<Song>();

            this.name = name;
            this.description = description;
        }

        public void addSong(Song song)
        {
            songs.Add(song);
        }

        public void removeSong(string title)
        {
            songs.RemoveAll(s => s.title == title);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(JSONEncoders.EncodeJsString(name));
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString(description));
            for (int i = 0; i < songs.Count; i++)
            {
                sb.Append(',');
                sb.Append(songs[i].ToString());
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
