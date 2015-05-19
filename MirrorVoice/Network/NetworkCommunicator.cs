﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace MirrorInteractions.Network
{
    public class NetworkCommunicator
    {
        private readonly static String serverAdress = "ws://127.0.0.1:1337";
        public static void SendToServer(WSMessage wsMessage)
        {
            String json = new JavaScriptSerializer().Serialize(wsMessage);
            using (var ws = new WebSocket(serverAdress))
            {
                ws.Connect();
                ws.Send(json);
            }
        }
    }
}
