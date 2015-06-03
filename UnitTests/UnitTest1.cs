using Microsoft.VisualStudio.TestTools.UnitTesting;
using MirrorInteractions;
using MirrorInteractions.Network;
using System;

namespace UnitTests {
    [TestClass]
    public class UnitTest1 {
        public const String NULL_APPOINTMENT = "\"appointment\":null";
        public const String VOICE = "\"type\":\"voice\"";
        public const String GESTURE = "\"type\":\"gesture\"";
        public const String FACE_RECOGNITION = "\"type\":\"facerecognition\"";

        public const String APP = "test_app";
        public const String ACTION = "test_action";
        public const String PERSON = "test_person";

        [TestMethod]
        public void ActionTypeTests() {
            WSMessage message = new WSMessage(APP, InteractionType.Voice, ACTION, PERSON);

            Assert.IsTrue(this.IsInteractionType(message.getInteractionType()), "You should not be able to provide a non-interactiveType.");
            Assert.IsTrue(message.getInteractionType().Equals(InteractionType.Voice), "The interactionType should be Voice, but was " + message.type + ".");
        }

        [TestMethod]
        public void ActionTests() {
            WSMessage message = new WSMessage(APP, InteractionType.Voice, ACTION, PERSON);

            Assert.AreNotEqual(message.action, "onzin", "The action on " + message.app + " can not be \"onzin\".");
        }

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