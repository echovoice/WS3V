using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WS3V.Interfaces;
using WS3V.JSON;
using WS3V.MessageTypes;
using WS3V.Support;

namespace WS3V
{
    public class WS3V_Client : IDisposable
    {
        public bool authenticated { get; set; }
        public List<PubSub_Channel> subscriptions { get; set; }

        private bool isterminated = false;
        private WS3V_Protocol protocol;

        public WS3V_Client(WS3V_Protocol protocol)
        {
            // connect protocol to client class
            this.protocol = protocol;

            // setup heartbeat if enabled
            if (protocol.heartbeat.enabled)
            {
                // wire up the cardiac arrest option
                protocol.heartbeat.cardiac_arrest = () =>
                {
                    send_terminated(408, "Request Timeout", "http://example.com/api/error#408");
                };
            }

            // are we using credentials?
            if (protocol.credentials != null)
            {
                authenticated = false;

                // send the client the gatekeeper message
                send_gatekeeper();

                // we need to setup a thread to kill the connection on authentication timeout
                if (protocol.authentication_timeout > 0)
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(protocol.authentication_timeout * 1000);

                        if (!authenticated && !isterminated)
                            send_terminated(403, "Forbidden", "http://example.com/api/error#403");

                    }).Start();
                }
            }
            else
            {
                // no authentication, authorize and send howdy
                authenticated = true;
                send_howdy();
            }
        }

        // send string over socket
        public void SocketSend(string input)
        {
            if(!string.IsNullOrWhiteSpace(input))
                protocol.SocketSend(input);
        }

        // send object over socket
        public void SocketSend(object input)
        {
            if (input != null)
            {
                string output = input.ToString();

                if (!string.IsNullOrWhiteSpace(output))
                    protocol.SocketSend(output);
            }
        }

        public void Process(string input)
        {
            // check if the client has a heartbeat
            if (protocol.heartbeat.enabled)
            {
                // pump the heart
                protocol.heartbeat.beat();

                // check if the incomming is a lub from the client
                if (input == "lub")
                {
                    // reply and return
                    protocol.SocketSend("dub");
                    return;
                }
            }

            // all messages incoming follow the JSONArray format
            string[] message = JSONDecoders.DecodeJSONArray(input);
            int message_type = 0;

            // lets first validate the incoming message
            if (int.TryParse(message[0], out message_type) && message_type > 0 && message_type <= 26)
            {
                // run the valid message through a switch
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

                    // channel listings command
                    case 8:
                        process_channels(message);
                        return;

                }
            }

            // the message is not valid, send bad request
            send_terminated(400, "Bad Request", "http://example.com/api/error#400");
        }

        // lets tell the client what channels are availible
        public void process_channels(string[] message)
        {
            if (protocol.channel_listing && protocol.pubsub != null)
            {
                protocol.SocketSend(new listings(protocol.pubsub).ToString());
            }
        }

        // this is the main processor for incoming rpc commands
        // it basically loops out to the wire up on the outside to process the rpc commands externally
        // and also catches error and returns those correctly to the client
        public void process_send(string[] message)
        {
            try
            {
                protocol.SocketSend(new receive(message[1], protocol.RPC(new RPC_Incoming(message)).response, protocol.headers).ToString());
            }

            // catch our own errors triggered externally
            catch (RPC_Exception e)
            {
                protocol.SocketSend(new error(message[1], e).ToString());
            }

            // catch those unknown errors, send the response to the client
            // if you need to be more stealthy, don't inclucde e.message
            catch (Exception e)
            {
                protocol.SocketSend(new error(message[1], new RPC_Exception(500, e.Message)).ToString());
            }
        }

        // here we need to check the authentication, so lets validate the signature object
        public void check_signature(string[] message)
        {
            // check the authenticate hook, true means they are authenticated
            if (protocol.Authenticate(JSONDecoders.DecodeJSONArray(message[1])))
            {
                authenticated = true;
                send_howdy();
            }

            // they didnt authenticate, check if we should allow them to try again
            else if (protocol.authentication_attempts > 0)
                send_gatekeeper();

            // the user failed authentication and exceeded the maximum number of attempts
            else
                send_terminated(403, "Forbidden", "http://example.com/api/error#403");
        }
       
        
        public void send_gatekeeper()
        {
            gatekeeper g = new gatekeeper(protocol.credentials, protocol.authentication_timeout, protocol.authentication_attempts--, 401, "Unauthorized", "http://example.com/api/error#401");
            protocol.SocketSend(g.ToString());
        }

        public void send_howdy()
        {
            howdy h = new howdy(protocol.clientID, protocol.server, protocol.recovery_timeout, protocol.heartbeat, protocol.filetransfer);

            if (protocol.channel_listing)
                h.channel_listing = true;

            if (protocol.recovery)
                h.recovery = true;

            if (protocol.headers != null)
                h.headers = protocol.headers;

            protocol.SocketSend(h.ToString());
        }

        public void send_terminated(int code, string message, string url = "")
        {
            isterminated = true;

            terminated t = new terminated(code, message, url);
            protocol.SocketSend(t.ToString());
            protocol.Dispose();
        }

        public void Dispose()
        {
            //if(!isterminated)
            //    send_terminated(200, "Good bye");
        }
    }
}
