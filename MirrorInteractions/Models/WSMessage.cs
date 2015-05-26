using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions {
    /// <summary>
    /// Enum collection of the available interaction types.
    /// </summary>
    public enum InteractionType {
        Voice,
        Gesture,
        FaceRecognition
    }

    /// <summary>
    /// Class containing a message for the Websocket.
    /// </summary>
    public class WSMessage {
        private InteractionType _actionType;

        public InteractionType actionType {
            get {
                return this._actionType;
            }
            set {
                this.type = value.ToString();
                this._actionType = value;
            }
        }
        public String app { get; set; }
        private String type { get; set; }
        public String action { get; set; }
        public String person { get; set; }
        public Appointment appointment { get; set; }
    }

    /// <summary>
    /// Class containing an appointment for the agenda app.
    /// </summary>
    public class Appointment {
        public String title { get; set; }
        public String desc { get; set; }
        public String datetime { get; set; }
    }
}