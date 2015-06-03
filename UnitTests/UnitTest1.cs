// ***********************************************************************
// Assembly         : UnitTests
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="UnitTest1.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System;

/// <summary>
/// The UnitTests namespace.
/// </summary>
namespace UnitTests {
    /// <summary>
    /// Class UnitTest1.
    /// </summary>
    [TestClass]
    public class UnitTest1 {
        /// <summary>
        /// The nul l_ appointment
        /// </summary>
        public const String NULL_APPOINTMENT = "\"appointment\":null";
        /// <summary>
        /// The voice
        /// </summary>
        public const String VOICE = "\"type\":\"voice\"";
        /// <summary>
        /// The gesture
        /// </summary>
        public const String GESTURE = "\"type\":\"gesture\"";
        /// <summary>
        /// The fac e_ recognition
        /// </summary>
        public const String FACE_RECOGNITION = "\"type\":\"facerecognition\"";

        /// <summary>
        /// The application
        /// </summary>
        public const String APP = "test_app";
        /// <summary>
        /// The action
        /// </summary>
        public const String ACTION = "test_action";
        /// <summary>
        /// The person
        /// </summary>
        public const String PERSON = "test_person";

        /// <summary>
        /// Actions the type tests.
        /// </summary>
        [TestMethod]
        public void ActionTypeTests() {
            WSMessage message = new WSMessage(APP, InteractionType.Voice, ACTION, PERSON);

            Assert.IsTrue(this.IsInteractionType(message.getInteractionType()), "You should not be able to provide a non-interactiveType.");
            Assert.IsTrue(message.getInteractionType().Equals(InteractionType.Voice), "The interactionType should be Voice, but was " + message.type + ".");
        }

        /// <summary>
        /// Actions the tests.
        /// </summary>
        [TestMethod]
        public void ActionTests() {
            WSMessage message = new WSMessage(APP, InteractionType.Voice, ACTION, PERSON);

            Assert.AreNotEqual(message.action, "onzin", "The action on " + message.app + " can not be \"onzin\".");
        }

        /// <summary>
        /// Jsons the tests.
        /// </summary>
        [TestMethod]
        public void JSONTests() {
            WSMessage message = new WSMessage(APP, InteractionType.FaceRecognition, ACTION, PERSON);
            String json = NetworkCommunicator.ConvertToJson(message);

            Assert.AreEqual(json.Contains(NULL_APPOINTMENT), true, "The appointment was not empty.");
            Assert.AreEqual(json.Contains(FACE_RECOGNITION), true, "The checked message was not a face req. (" + json + ")");

            message = new WSMessage(APP, InteractionType.Voice, ACTION, PERSON);
            json = NetworkCommunicator.ConvertToJson(message);

            Assert.IsTrue(message.getInteractionType().Equals(InteractionType.Voice), "The message was not sent with voice.");
            Assert.IsTrue(json.Contains(VOICE), "The message was not a voice command.");
        }

        [TestMethod]
        public void GestureTest() {
            WSMessage message = new WSMessage(InteractionType.Gesture, "right");
            String json = NetworkCommunicator.ConvertToJson(message);

            Assert.IsTrue(json.Contains(GESTURE), "The message was not a gesture: " + json);
        }

        /// <summary>
        /// Checks if the interaction is valid.
        /// </summary>
        /// <param name="interactionType">The interactionType to check for.</param>
        /// <returns>True if the interactionType is valid, else false.</returns>
        private bool IsInteractionType(InteractionType interactionType) {
            switch (interactionType) {
                case InteractionType.FaceRecognition:
                case InteractionType.Gesture:
                case InteractionType.Voice:
                    return true;

                default:
                    return false;
            }
        }
    }
}
