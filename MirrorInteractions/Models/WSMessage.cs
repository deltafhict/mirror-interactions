// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="WSMessage.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MirrorInteractions.Models
{
    /// <summary>
    /// Enum collection of the available interaction types.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// The voice
        /// </summary>
        Voice,
        /// <summary>
        /// The gesture
        /// </summary>
        Gesture,
        /// <summary>
        /// The face recognition
        /// </summary>
        FaceRecognition
    }

    /// <summary>
    /// Class containing a message for the Websocket.
    /// </summary>
    public class WSMessage
    {
        /// <summary>
        /// The application
        /// </summary>
        public readonly String app;
        /// <summary>
        /// The type
        /// </summary>
        public readonly String type;
        /// <summary>
        /// The interaction type
        /// </summary>
        private InteractionType interactionType;
        /// <summary>
        /// The action
        /// </summary>
        public readonly String action;
        /// <summary>
        /// The person
        /// </summary>
        public readonly String person;
        /// <summary>
        /// The appointment
        /// </summary>
        public readonly Appointment appointment;

        /// <summary>
        /// Gets the type of the interaction.
        /// </summary>
        /// <returns>MirrorInteractions.InteractionType.</returns>
        public InteractionType getInteractionType()
        {
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
        public WSMessage(String app, InteractionType type, String action, String person, Appointment appointment)
        {
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
        public WSMessage(String app, InteractionType type, String action, String person)
        {
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
        public WSMessage(String app, String action)
        {
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
    public class Appointment
    {
        /// <summary>
        /// The n o_ desc
        /// </summary>
        public const String NO_DESC = "No description";
        /// <summary>
        /// The n o_ title
        /// </summary>
        public const String NO_TITLE = "No title";

        /// <summary>
        /// The title
        /// </summary>
        public readonly String title = NO_TITLE;
        /// <summary>
        /// The desc
        /// </summary>
        public readonly String desc = NO_DESC;
        /// <summary>
        /// The datetime
        /// </summary>
        public readonly String datetime;

        /// <summary>
        /// Creates an appointment with a title, description and time.
        /// </summary>
        /// <param name="title">The title of the appointment.</param>
        /// <param name="desc">The description of the appointment.</param>
        /// <param name="datetime">The time of the appointment.</param>
        public Appointment(String title, String desc, DateTime datetime)
        {
            this.title = title;
            this.desc = desc;
            this.datetime = datetime.ToString();
        }

        /// <summary>
        /// Creates an appointment with a title, empty description and time.
        /// </summary>
        /// <param name="title">The title of the appointment.</param>
        /// <param name="datetime">The time of the appointment.</param>
        public Appointment(String title, DateTime datetime)
        {
            this.title = title;
            this.datetime = datetime.ToString();
        }

        /// <summary>
        /// Creates an appointment with an empty title, empty description and time.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        public Appointment(DateTime datetime)
        {
            this.datetime = datetime.ToString();
        }
    }
}
