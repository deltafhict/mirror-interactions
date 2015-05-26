using Microsoft.VisualStudio.TestTools.UnitTesting;
using MirrorInteractions;
using System;

namespace UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void ActionTypeTests() {
            WSMessage message = new WSMessage {
                action = "action",
                app = "app",
                actionType = InteractionType.Voice,
                person = "Bas"
            };

            Assert.AreNotEqual(message.actionType.ToString(), "Onzin", "You should not be able to provide a non-interactiveType.");
            Assert.AreEqual(message.actionType, InteractionType.Voice, "The interactionType should be Voice, but was " + message.actionType.ToString() + ".");

        }

        [TestMethod]
        public void ActionTests() {
            WSMessage message = new WSMessage {
                action = "open",
                app = "calendar",
                actionType = InteractionType.Voice,
                person = "Bas"
            };

            Assert.AreNotEqual(message.action, "onzin", "The action on " + message.app + " can not be \"onzin\".");
        }
    }
}