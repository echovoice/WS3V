using Fleck;
using RESTful_Sample.Music_Playlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WS3V.JSON;
using WS3V.MessageTypes;

namespace RESTful_Sample
{
    /// <summary>
    /// Designed to be used with Fleck Server, swap socket object if using another.
    /// </summary>
    public class Client : IDisposable
    {
        public IWebSocketConnection socket { get; set; }
        public bool authenticated { get; set; }

        private bool running = false;
        private bool beat = false;
        private int heartinterval_max, authentication_timeout, max_auths;
        private Thread heartbeat;

        // obviously this is not how this works server-side, but you knew that
        private const string api_key = "[\"98eac98feeaf8e25410ce135076d688a\"]";

        // again this is NOT how APIs work, but this is how demos work, lets create some dummy static playlist data
        private Playlist playlist = new Playlist("The Best of Phil Collins", "Collins is one of only three recording artists (along with Paul McCartney and Michael Jackson) who have sold over 100 million albums worldwide");

        public Client(IWebSocketConnection socket, int authentication_timeout = 10, int heartinterval_max = 60, int max_auths = 3)
        {
            // add some of the best songs ever written...
            playlist.addSong(new Song("Phil Collins", "No Jacket Required", "Sussudio", "http://g-ecx.images-amazon.com/images/G/01/ciu/e5/41/4059c060ada08c928d90e110.L._AA300_.jpg"));
            playlist.addSong(new Song("Phil Collins", "...Hits", "Another Day In Paradise", "http://ecx.images-amazon.com/images/I/51WRpYh8nNL._SL500_AA280_.jpg"));
            playlist.addSong(new Song("Phil Collins", "Tarzan [Soundtrack]", "You'll Be In My Heart", "http://ecx.images-amazon.com/images/I/516JGYA351L._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));
            playlist.addSong(new Song("Phil Collins", "Face Value", "In The Air Tonight", "http://ecx.images-amazon.com/images/I/414D91CKFFL._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));
            playlist.addSong(new Song("Phil Collins", "Love Songs: A Compilation Old & New", "True Colours", "http://ecx.images-amazon.com/images/I/51Y46K9GYEL._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));

            this.socket = socket;
            this.authentication_timeout = authentication_timeout;
            this.heartinterval_max = heartinterval_max;
            this.max_auths = max_auths;
            authenticated = false;
            running = true;

            // start the heartbeat thread
            heartbeat = new Thread(new ThreadStart(LubDub));
            heartbeat.Start();

            send_gatekeeper();

        }

        // Heartbeat thread loop, also used for gatekeeper timeout catching
        private void LubDub()
        {
            while (running)
            {
                // start with sleep cycle
                int z = (!authenticated) ? authentication_timeout : heartinterval_max;
                while (running && z-- > 0)
                    Thread.Sleep(1000);

                if (!running)
                    return;

                if (!authenticated)
                {
                    send_terminated(403, "Forbidden", "http://example.com/api/error#403");
                    return;
                }

                if (!beat)
                    send_terminated(408, "Request Timeout", "http://example.com/api/error#408");

                beat = false;
            }
        }

        public void process(string input)
        {
            beat = true;

            if (input == "lub")
            {
                socket.Send("dub");
                return;
            }

            string[] message = JSONDecoders.DecodeJSONArray(input);
            int message_type = 0;

            if (int.TryParse(message[0], out message_type) && message_type > 0 && message_type <= 26)
            {
                switch(message_type)
                {
                    // signature response
                    case 2:
                        check_signature(message);
                        return;

                    // rpc command
                    case 5:
                        process_send(message);
                        return;

                }
            }

            send_terminated(400, "Bad Request", "http://example.com/api/error#400");
        }

        public void process_send(string[] message)
        {
            // in this demo we only have 3 potential commands

            if (message.Length == 4 && message[2] == "GET" && message[3] == "/user/playlist/bestofphil")
            {
                // send the playlist
                send_receive(message[1], playlist.ToString());
            }
            else
                send_terminated(400, "Bad Request", "http://example.com/api/error#400");
        }

        public void check_signature(string[] message)
        {
            if (message.Length == 2 && !string.IsNullOrWhiteSpace(message[1]) && message[1] == api_key)
            {
                authenticated = true;
                send_howdy();
            }
            else if (max_auths > 0)
                send_gatekeeper();
            else
                send_terminated(403, "Forbidden", "http://example.com/api/error#403");
        }

        public void send_receive(string id, string response, string headers = null)
        {
            // http://ws3v.org/spec.json#receive
            receive r = new receive(id, response, headers);
            socket.Send(r.ToString());
        }
        
        public void send_howdy()
        {
            // http://ws3v.org/spec.json#howdy
            howdy h = new howdy(socket.ConnectionInfo.Id.ToString("N"), "Sample RESTful API Demo 0.9.6", 0, heartinterval_max, false, 0, 0);
            socket.Send(h.ToString());
        }
        
        public void send_gatekeeper()
        {
            // http://ws3v.org/spec.json#gatekeeper
            gatekeeper g = new gatekeeper("api_key", authentication_timeout, max_auths--, 401, "Unauthorized", "http://example.com/api/error#401");
            socket.Send(g.ToString());
        }

        public void send_terminated(int code, string message, string url)
        {
            // http://ws3v.org/spec.json#terminated

            terminated t = new terminated(code, message, url);
            socket.Send(t.ToString());
            socket.Close();
        }

        public void Dispose()
        {
            running = false;

            // wait for heartbeat thread to die
            while (heartbeat.IsAlive)
                Thread.Sleep(100);

            heartbeat.Join(0);
        }
    }
}
