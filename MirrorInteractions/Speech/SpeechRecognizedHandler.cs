using Microsoft.Speech.Recognition;
using MirrorInteractions;
using MirrorInteractions.Face;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions.Speech {
    class SpeechRecognizedHandler {
        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender1">object sending the event.</param>
        /// <param name="e1">event arguments.</param>
        public void SpeechRecognized(object sender1, SpeechRecognizedEventArgs e1) {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e1.Result.Confidence >= ConfidenceThreshold) {
                Console.WriteLine("Speech recognized: " + e1.Result.Text.ToLower());
                String app = e1.Result.Semantics.Value.ToString().ToLower();
                String resultText = e1.Result.Text.ToLower();

                switch (app) {
                    case "agenda":
                        String action = "";
                        if (resultText.Contains("open")) {
                            action = "open";
                        } else if (resultText.Contains("close")) {
                            action = "close";
                        }
                        WSMessage messageToSend = new WSMessage {
                            action = action,
                            app = app,
                            actionType = InteractionType.Voice,
                            person = RecognizedPerson.recognizedPerson
                        };
                        NetworkCommunicator.SendToServer(messageToSend);
                        break;

                    case "initialize face":
                        String personName = "Daan";
                        FaceRecognizedHandler.SetLearnNewFaces(personName);
                        break;
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e) {
            Console.WriteLine("Speech rejected");
        }
    }
}