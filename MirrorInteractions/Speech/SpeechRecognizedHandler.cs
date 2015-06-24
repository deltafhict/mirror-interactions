// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="SpeechRecognizedHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Speech.Recognition;
using MirrorInteractions.Face;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System;
using System.Windows;

namespace MirrorInteractions.Speech {
    /// <summary>
    /// Class SpeechRecognizedHandler.
    /// </summary>
    class SpeechRecognizedHandler {
        public double ConfidenceThreshold = 0.5;
        /// <summary>
        /// The speech calibrate delegate
        /// </summary>
        private SpeechDelegate.SpeechCalibrateDelegate speechCalibrateDelegate;


        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechCalibrationHandler"/> class.
        /// </summary>
        /// <param name="speechCalibratedDelegate">The speech calibrated delegate.</param>
        public SpeechRecognizedHandler(SpeechDelegate.SpeechCalibrateDelegate speechCalibrateDelegate)
        {
            this.speechCalibrateDelegate = speechCalibrateDelegate;
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender1">object sending the event.</param>
        /// <param name="e1">event arguments.</param>
        public void SpeechRecognized(object sender1, SpeechRecognizedEventArgs e1) {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            //const double ConfidenceThreshold = 0.3;

            if (e1.Result.Confidence >= ConfidenceThreshold) {
                Console.WriteLine("Speech recognized: " + e1.Result.Text.ToLower());
                String app = e1.Result.Semantics.Value.ToString().ToLower();
                String resultText = e1.Result.Text.ToLower();
                String action = "";

                switch (app) {
                    case "agenda":
                        if (resultText.Contains("open")) {
                            action = "open";
                        } else if (resultText.Contains("close")) {
                            action = "close";
                        }
                        NetworkCommunicator.Instance.SendToServer(new WSMessage(app, InteractionType.Voice, action, RecognizedPerson.recognizedPerson));
                        break;

                    case "initialize face":
                        String personName = resultText.Remove(0,20);
                        FaceRecognition.Instance.LearnNewFaces(personName);
                        break;

                    case "initialize speech":
                        speechCalibrateDelegate();
                        break;

                    case "opus":
                        if (resultText.Contains("on"))
                        {
                            action = "on";
                        }
                        else if (resultText.Contains("off"))
                        {
                            action = "off";
                        }
                        else
                        {
                            return;
                        }
                        NetworkCommunicator.Instance.SendToServer(new WSMessage(app, InteractionType.Voice, action, RecognizedPerson.recognizedPerson));
                        break;
                    case "weather":
                    case "mail":
                        MessageBox.Show(resultText);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Speech recognized but confidence too low: " + e1.Result.Confidence);
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