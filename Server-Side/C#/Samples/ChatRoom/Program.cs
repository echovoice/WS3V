using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using WS3V;
using WS3V.JSON;
using WS3V.Support;
using Chat_Room_Sample.Chat_Rooms;

namespace Chat_Room_Sample
{
    class Program
    {
        static void Main()
        {
            // used to maintain a list of clients
            ConcurrentDictionary<Guid, WS3V_Client> WS3V_Clients = new ConcurrentDictionary<Guid, WS3V_Client>();

            // in this demo we setup the chatrooms
            List<Room> rooms = new List<Room>();
            rooms.Add(new Room("The Best Chat Room", "Where only the best people chat."));
            rooms.Add(new Room("<3 Phil Collins", "In the air tonight, each and every night."));
            rooms.Add(new Room("80's Music Fans ONLY!", "If you love 80's music come chat."));

            // create the fleck websocket server
            // see https://github.com/statianzo/Fleck for more information
            FleckLog.Level = LogLevel.Debug;
            WebSocketServer server = new WebSocketServer("ws://localhost:8181");

            // now we take the websocket server hooks and connect our demo processing code to them
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // on open callback we create the client
                    WS3V_Client client = new WS3V_Client(new WS3V_Protocol(ws3v_protocol =>
                    {
                        // we need to pass the socket send and close methods inside the protocol
                        ws3v_protocol.SocketSend = socket.Send;
                        ws3v_protocol.Dispose = socket.Close;

                        // lets provide some more info about this server
                        ws3v_protocol.server = "Sample Chat Room Demo 0.9.6";

                        // provide the hook with the clientID, convert GUID to string
                        ws3v_protocol.clientID = socket.ConnectionInfo.Id.ToString("N");

                        // lets setup the heartbeat
                        ws3v_protocol.heartbeat.heartbeat_min_seconds = 30;
                        ws3v_protocol.heartbeat.heartbeat_max_seconds = 60;
                        ws3v_protocol.heartbeat.allow_heartbeats_when_busy = false;

                        // enable channel listing
                        ws3v_protocol.channel_listing = true;

                        // lets wire up the rpc commands
                        /*ws3v_protocol.RPC = (x) =>
                        {
                            if (x.uri == "/user/playlist/bestofphil")
                            {
                                if (x.method == "GET")
                                {
                                    // send the playlist
                                    return new RPC_Outgoing(playlist);
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
                        };*/

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

            Console.WriteLine("\r\n\r\nServer-side is now running, please open the Client-side sample file:");
            Console.WriteLine("\r\n\t\\Client-side\\JS\\Samples\\Chat Room Sample\\client.html");

            string input = Console.ReadLine();
        }
    }
}
