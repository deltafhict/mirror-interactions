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
        public readonly String app;
        public readonly String type;
        private InteractionType interactionType;
        public readonly String action;
        public readonly String person;
        public readonly Appointment appointment;

        public InteractionType getInteractionType() {
            return this.interactionType;
        }

        /// <summary>
        /// Creates a WSMessage with an appointment.
        /// </summary>
        /// <param name="app">The app the message was created for.</param>
        /// <param name="type">The interaction that created the message.</param>
        /// <param name="action">The action of the message.</param>
        /// <param name="person">The person that sent the message.</param>
        /// <param name="appointment">The appointment to add to  the message.</param>
        public WSMessage(String app, InteractionType type, String action, String person, Appointment appointment) {
            this.app = app;
            this.type = type.ToString().ToLower();
            this.interactionType = type;
            this.action = action;
            this.person = person;
            this.appointment = appointment;
        }

        /// <summary>
        /// Creates a WSMessage without an appointment.
        /// </summary>
        /// <param name="app">The app the message was created for.</param>
        /// <param name="type">The interaction that created the message.</param>
        /// <param name="action">The action of the message.</param>
        /// <param name="person">The person that sent the message.</param>
        public WSMessage(String app, InteractionType type, String action, String person) {
            this.app = app;
            this.type = type.ToString().ToLower();
            this.interactionType = type;
            this.action = action;
            this.person = person;
        }

        /// <summary>
        /// Creates a dummy-message.
        /// </summary>
        /// <param name="app">The app the message was created for.</param>
        /// <param name="action">The action of the message.</param>
        public WSMessage(String app, String action) {
            this.app = app;
            this.action = action;
        }

        /// <summary>
        /// Creates a message send as an gesture.
        /// </summary>
        /// <param name="type">The interaction that created the message.</param>
        /// <param name="action">The action of the message.</param>
        public WSMessage(InteractionType type, String action) {
            this.type = type.ToString().ToLower();
            this.action = action;
        }
    }

    /// <summary>
    /// Class containing an appointment for the agenda app.
    /// </summary>
    public class Appointment {
        public const String NO_DESC = "No description";
        public const String NO_TITLE = "No title";

        public readonly String title = NO_TITLE;
        public readonly String desc = NO_DESC;
        public readonly String datetime;

        /// <summary>
        /// Creates an appointment with a title, description and time.
        /// </summary>
        /// <param name="title">The title of the appointment.</param>
        /// <param name="desc">The description of the appointment.</param>
        /// <param name="datetime">The time of the appointment.</param>
        public Appointment(String title, String desc, DateTime datetime) {
            this.title = title;
            this.desc = desc;
            this.datetime = datetime.ToString();
        }

        /// <summary>
        /// Creates an appointment with a title, empty description and time.
        /// </summary>
        /// <param name="title">The title of the appointment.</param>
        /// <param name="datetime">The time of the appointment.</param>
        public Appointment(String title, DateTime datetime) {
            this.title = title;
            this.datetime = datetime.ToString();
        }

        /// <summary>
        /// Creates an appointment with an empty title, empty description and time.
        /// </summary>
        /// <param name="datetime"></param>
        public Appointment(DateTime datetime) {
            this.datetime = datetime.ToString();
        }
    }
}