using Fleck;
using RESTful_Sample.Music_Playlists;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V;
using WS3V.JSON;
using WS3V.Support;

namespace RESTful_Sample
{
    public class Websocket : IDisposable
    {
        // used to maintain a list of clients
        public ConcurrentDictionary<Guid, WS3V_Client> WS3V_Clients = new ConcurrentDictionary<Guid, WS3V_Client>();

        // websocket server
        public WebSocketServer server;

        // start the websocket demo
        public void start(string connect)
        {
            // create the fleck websocket server
            // see https://github.com/statianzo/Fleck for more information
            FleckLog.Level = LogLevel.Debug;
            server = new WebSocketServer(connect);

            // now we take the websocket server hooks and connect our demo processing code to them
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // on open callback we create the client
                    WS3V_Client client = new WS3V_Client(new WS3V_Protocol(ws3v_protocol =>
                    {
                        Playlist playlist = new Playlist("", "");

                        // we need to pass the socket send and close methods inside the protocol
                        ws3v_protocol.SocketSend = socket.Send;
                        ws3v_protocol.Dispose = socket.Close;

                        // lets provide some more info about this server
                        ws3v_protocol.server = "Sample RESTful API Demo 0.9.6";

                        // provide the hook with the clientID, convert GUID to string
                        ws3v_protocol.clientID = socket.ConnectionInfo.Id.ToString("N");

                        // lets setup the heartbeat
                        ws3v_protocol.heartbeat.heartbeat_min_seconds = 30;
                        ws3v_protocol.heartbeat.heartbeat_max_seconds = 60;
                        ws3v_protocol.heartbeat.allow_heartbeats_when_busy = false;

                        // here is where we define the server-side settings
                        // in this demo we are going to simulate an api_key requirement
                        ws3v_protocol.credentials = new string[] { "api_key" };
                        // obviously this is not how this works server-side, but you knew that and understand this is a demo
                        // replace this with your logic to validate an api key
                        ws3v_protocol.Authenticate = (x) =>
                        {
                            if (x[0] == "98eac98feeaf8e25410ce135076d688a")
                            {
                                // here we are going to simulate building a playlist for a particular client
                                // again this is NOT how you should do this, but for a demo this is perfect
                                playlist = new Playlist("The Best of Phil Collins", "Collins is one of only three recording artists (along with Paul McCartney and Michael Jackson) who have sold over 100 million albums worldwide");
                                playlist.addSong(new Song("Phil Collins", "No Jacket Required", "Sussudio", "http://g-ecx.images-amazon.com/images/G/01/ciu/e5/41/4059c060ada08c928d90e110.L._AA300_.jpg"));
                                playlist.addSong(new Song("Phil Collins", "...Hits", "Another Day In Paradise", "http://ecx.images-amazon.com/images/I/51WRpYh8nNL._SL500_AA280_.jpg"));
                                playlist.addSong(new Song("Phil Collins", "Tarzan [Soundtrack]", "Youll Be In My Heart", "http://ecx.images-amazon.com/images/I/516JGYA351L._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));
                                playlist.addSong(new Song("Phil Collins", "Face Value", "In The Air Tonight", "http://ecx.images-amazon.com/images/I/414D91CKFFL._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));
                                playlist.addSong(new Song("Phil Collins", "Love Songs: A Compilation Old & New", "True Colours", "http://ecx.images-amazon.com/images/I/51Y46K9GYEL._SL500_AA300__PJautoripBadge,BottomRight,4,-40_OU11__.jpg"));

                                // return true to say the user is valid
                                return true;
                            }

                            // user is not authenticated, return false
                            return false;
                        };

                        // lets wire up the rpc commands
                        ws3v_protocol.RPC = (x) =>
                        {
                            if (x.uri == "/user/playlist/bestofphil")
                            {
                                if (x.method == "GET")
                                {
                                    // send the playlist and set the expiration header 5 min from now
                                    return new RPC_Outgoing(playlist, DateTime.UtcNow.AddMinutes(5));
                                }
                                else if (x.method == "DELETE")
                                {
                                    // delete the song
                                    playlist.removeSong(x.parameters);
                                    return new RPC_Outgoing("done");
                                }
                                else if (x.method == "POST")
                                {
                                    // add the song
                                    string[] song = JSONDecoders.DecodeJSONArray(x.parameters);
                                    playlist.addSong(new Song(song[2], song[3], song[1], song[0]));
                                    return new RPC_Outgoing("done");
                                }
                            }

                            throw new RPC_Exception(400, "Bad Request", "http://example.com/api/error#400");
                        };

                    }));

                    // add the client to the concurrent dictionary
                    WS3V_Clients.TryAdd(socket.ConnectionInfo.Id, client);
                };

                socket.OnClose = () =>
                {
                    // on close callback means we need to pull the client from the
                    // concurrent dictionary and call the dispose method on it
                    WS3V_Client c; WS3V_Clients.TryRemove(socket.ConnectionInfo.Id, out c);
                    if (c != null) c.Dispose();
                };

                socket.OnMessage = message =>
                {
                    // when a message is recieved over the websocket, try to match the client by id
                    // if we find the client then send the message to the protocol process method
                    WS3V_Client c; WS3V_Clients.TryGetValue(socket.ConnectionInfo.Id, out c);
                    if (c != null) c.Process(message);
                };
            });
        }

        public void Dispose()
        {
            WS3V_Clients.Clear();
            server.Dispose();
        }
    }
}