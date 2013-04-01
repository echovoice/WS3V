using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Demo
{
    public class WebRole : RoleEntryPoint
    {
        Chat_Room_Sample.Websocket websocket;
        RESTful_Sample.Websocket websocket2;

        public override bool OnStart()
        {
            websocket = new Chat_Room_Sample.Websocket();
            websocket.start("ws://localhost:8181");

            websocket2 = new RESTful_Sample.Websocket();
            websocket2.start("ws://localhost:8182");

            return base.OnStart();
        }

        public override void OnStop()
        {
            websocket.Dispose();
            websocket2.Dispose();
        }
    }
}
