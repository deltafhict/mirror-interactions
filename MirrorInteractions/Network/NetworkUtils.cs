using MirrorInteractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MirrorInteractions.Network
{
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
