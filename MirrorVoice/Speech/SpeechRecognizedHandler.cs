using Microsoft.Speech.Recognition;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions.Speech
{
    class SpeechRecognizedHandler
    {
        private enum Types { voice, gesture, faceRecognition }
        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        public void SpeechRecognized(object sender1, SpeechRecognizedEventArgs e1)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e1.Result.Confidence >= ConfidenceThreshold)
            {
                Console.WriteLine("Speech recognized: " + e1.Result.Text.ToLower());
                switch (e1.Result.Semantics.Value.ToString())
                {
                    case "AGENDA":
                        String action = "";
                        String resultText = e1.Result.Text.ToLower();
                        if (resultText.Contains("open"))
                        {
                            action = "open";
                        }
                        else if (resultText.Contains("close"))
                        {
                            action = "close";
                        }
                        WSMessage messageToSend = new WSMessage
                        {
                            action = action,
                            app = e1.Result.Semantics.Value.ToString().ToLower(),
                            type = Types.voice.ToString(),
                            person = RecognizedPerson.recognizedPerson
                        };
                        NetworkCommunicator.SendToServer(messageToSend);
                        break;
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected");
        }
    }
}
