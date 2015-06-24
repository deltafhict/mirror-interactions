// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : daan
// Created          : 06-24-2015
//
// Last Modified By : daan
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="NetworkUtils.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used for support methods for the network calls.</summary>
// ***********************************************************************
using MirrorInteractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

/// <summary>
/// The Network namespace, all network related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Network
{
    /// <summary>
    /// Class used for support methods for the network calls.
    /// </summary>
    public class NetworkUtils
    {
        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        public static String ConvertToJson(WSMessage message)
        {
            return new JavaScriptSerializer().Serialize(message);
        }
    }
}
