using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using WS3V;
using WS3V.JSON;
using WS3V.Support;
using RESTful_Sample.Music_Playlists;

namespace RESTful_Sample
{
    class Program
    {
        static void Main()
        {
            // start the demo class
            Websocket websocket = new Websocket();
            websocket.start("ws://localhost:8181");

            Console.WriteLine("\r\n\r\nServer-side is now running, please open the Client-side sample file:");
            Console.WriteLine("\r\n\t\\Client-side\\JS\\Samples\\RESTful Sample\\client.html");

            string input = Console.ReadLine();
        }
    }
}
