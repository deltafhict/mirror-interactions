// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : daan
// Created          : 06-24-2015
//
// Last Modified By : daan
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="NetworkCommunicator.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used to communicate through websockets with the server.</summary>
// ***********************************************************************
using System;
using System.Web.Script.Serialization;
using WebSocketSharp;

/// <summary>
/// The Network namespace, all network related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Network {
    /// <summary>
    /// Class used to communicate through websockets with the server.
    /// </summary>
    public class NetworkCommunicator {
        /// <summary>
        /// The server adress
        /// </summary>
        private readonly static String serverAdress = "ws://127.0.0.1:1337";
        /// <summary>
        /// The instance
        /// </summary>
        private static NetworkCommunicator instance;
        /// <summary>
        /// The web socket
        /// </summary>
        private WebSocket webSocket;

        /// <summary>
        /// Prevents a default instance of the <see cref="NetworkCommunicator" /> class from being created.
        /// </summary>
        private NetworkCommunicator()
        {
            webSocket = new WebSocket(serverAdress);
            webSocket.OnMessage += webSocket_OnMessage;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
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

        /// <summary>
        /// Handles the OnMessage event of the webSocket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
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
                webSocket.Connect();
                webSocket.Send(json);
            }
        }
    }
}