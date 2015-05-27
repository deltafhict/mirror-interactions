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

        /// <summary>
        /// Sends the message to the websocket.
        /// </summary>
        /// <param name="wsMessage">The message to send.</param>
        public static void SendToServer(WSMessage wsMessage) {
            String json = ConvertToJson(wsMessage);

            using (var ws = new WebSocket(serverAdress)) {
                ws.Connect();
                ws.Send(json);
            }
        }

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        public static String ConvertToJson(WSMessage message) {
            return new JavaScriptSerializer().Serialize(message);
        }
    }
}