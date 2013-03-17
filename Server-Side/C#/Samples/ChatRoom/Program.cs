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

            // this is how channel control is handled outside the socket definition
            // use this object to populate channels for channel listing

            PubSub_Listing pubsub = new PubSub_Listing();
            for (int i = 0; i < rooms.Count; i++)
			{
                // create the pubsub channel
                PubSub_Channel c = new PubSub_Channel("/public/chatrooms/A" + i, rooms[i]);
                
                // save 100 of the oldest chats per chatroom
                c.Set_Max(100);

                // add the channel to the pubsub control
                pubsub.CreateChannel(c);
			}

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

                        // lets wire up the pubsub listing, this is required for all pubsub opperations
                        // even if listing is not enabled, not passing in the pubsub listing object
                        // will disable pubsub completely and block clients from subscribing
                        ws3v_protocol.pubsub = pubsub;

                        // we need to pass in the cocurrent dictionary for PubSub to work
                        ws3v_protocol.WS3V_Clients = WS3V_Clients;

                        // this demo wont use this, but this is how to pass dynamic channels into
                        // the server, channels created based on the subscription request
                        ws3v_protocol.Subscribe = channel_name_or_uri =>
                        {
                            // inside here based on the channel name and filter a subscription
                            // could be created and added to the pubsub object

                            // in this demo we will use this to increment the chat room count

                            PubSub_Channel c = pubsub.GetChannel(channel_name_or_uri);
                            
                            // make sure it isnt null
                            if (c != null)
                            {
                                // extract the room based on the channel meta data
                                Room r = new Room(c.channel_meta);

                                // increment the particpants
                                r.participants++;

                                // set the channel meta again
                                c.channel_meta = r.ToString();
                            }
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
                    if (c != null)
                    {
                        // we need to decrement the room particapant counts
                        // and lets send good bye messages to subscribed chat rooms

                        // make sure client was subscribed in the first place
                        if (c.subscriptions != null && c.subscriptions.Count > 0)
                        {
                            for (int i = 0; i < c.subscriptions.Count; i++)
                            {
                                // extract the room based on the channel meta data
                                Room r = new Room(c.subscriptions[i].channel_meta);

                                // increment the particpants
                                r.participants--;

                                // set the channel meta again
                                c.subscriptions[i].channel_meta = r.ToString();

                                // send good bye message
                                c.publish_channel(c.subscriptions[i].channel_name_or_uri, "{\"type\":3,\"message\":\"\",\"client\":\"" + c.clientID + "\"}", false);
                            }
                        }

                        c.Dispose();
                    }
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

            Console.WriteLine("\r\ntype anything into the console to blast a message to all chat rooms");
            string input = Console.ReadLine();
            while (input != "exit")
            {
                for (int i = 0; i < pubsub.channels.Count; i++)
                {
                    List<WS3V_Client> subscribers = WS3V_Clients.Select(t => t.Value).Where(c => c.subscriptions != null && c.subscriptions.Any(p => p.channel_name_or_uri == pubsub.channels[i].channel_name_or_uri)).ToList();
                    
                    for (int j = 0; j < subscribers.Count; j++)
                        subscribers[j].SocketSend(input);

                }
                input = Console.ReadLine();
            }
        }
    }
}
