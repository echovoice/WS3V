using Fleck;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTful_Sample
{
    class Program
    {
        static ConcurrentDictionary<Guid, Client> clients = new ConcurrentDictionary<Guid, Client>();

        static void Main()
        {
            FleckLog.Level = LogLevel.Debug;
            List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
            WebSocketServer server = new WebSocketServer("ws://localhost:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    clients.TryAdd(socket.ConnectionInfo.Id, new Client(socket));
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    Client c;
                    clients.TryRemove(socket.ConnectionInfo.Id, out c);
                    if(c!=null)
                        c.Dispose();
                };
                socket.OnMessage = message =>
                {
                    Client c;
                    clients.TryGetValue(socket.ConnectionInfo.Id, out c);
                    if (c != null)
                        c.process(message);
                };
            });

            Console.WriteLine("\r\n\r\nServer-side is now running, please open the Client-side sample file:");
            Console.WriteLine("\r\n\t\\Client-side\\JS\\Samples\\RESTful Sample\\client.html");

            string input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (IWebSocketConnection socket in allSockets.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }

        }
    }
}
