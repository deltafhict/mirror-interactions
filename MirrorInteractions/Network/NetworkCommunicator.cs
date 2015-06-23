using MirrorInteractions.Models;
// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="NetworkCommunicator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace MirrorInteractions.Network {
    /// <summary>
    /// Class NetworkCommunicator.
    /// </summary>
    public class NetworkCommunicator {
        /// <summary>
        /// The server adress
        /// </summary>
        private readonly static String serverAdress = "ws://127.0.0.1:1337";
        private static NetworkCommunicator instance;
        private WebSocket webSocket;

        private NetworkCommunicator()
        {
            webSocket = new WebSocket(serverAdress);
            webSocket.Connect();
            webSocket.OnMessage += webSocket_OnMessage;
        }

        public static NetworkCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkCommunicator();
                }
                return instance;
            }
        }

        void webSocket_OnMessage(object sender, MessageEventArgs e)
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the message to the websocket.
        /// </summary>
        /// <param name="wsMessage">The message to send.</param>
        public void SendToServer(WSMessage wsMessage) {
            String json = NetworkUtils.ConvertToJson(wsMessage);
            using (webSocket)
            {
                webSocket.Send(json);
            }
        }
    }
}