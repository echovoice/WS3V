using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V;

namespace Chat_Room_Sample
{
    class Program
    {
        static void Main()
        {
            // start the demo class
            Websocket websocket = new Websocket();
            websocket.start("ws://localhost:8181");

            Console.WriteLine("\r\n\r\nServer-side is now running, please open the Client-side sample file:");
            Console.WriteLine("\r\n\t\\Client-side\\JS\\Samples\\Chat Room Sample\\client.html");

            Console.WriteLine("\r\ntype anything into the console to blast a message to all chat rooms");
            string input = Console.ReadLine();
            while (input != "exit")
            {
                for (int i = 0; i < websocket.pubsub.channels.Count; i++)
                {
                    List<WS3V_Client> subscribers = websocket.WS3V_Clients.Select(t => t.Value).Where(c => c.subscriptions != null && c.subscriptions.Any(p => p.channel_name_or_uri == websocket.pubsub.channels[i].channel_name_or_uri)).ToList();

                    for (int j = 0; j < subscribers.Count; j++)
                        subscribers[j].publish_channel(websocket.pubsub.channels[i].channel_name_or_uri, "{\"type\":1,\"message\":\"" + input + "\",\"client\":\"ADMIN\"}", false);

                }
                input = Console.ReadLine();
            }
        }
    }
}