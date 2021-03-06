﻿// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 06-09-2015
//
// Last Modified By : delta
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="SpeechCalibrationHandler.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used to handle speech calibration events.</summary>
// ************************************************************************
using Microsoft.Speech.Recognition;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

/// <summary>
/// The Speech namespace, all Speech related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Speech
{
    /// <summary>
    /// Class used to handle speech calibration events.
    /// </summary>
    class SpeechCalibrationHandler
    {
        /// <summary>
        /// The calibration speech count
        /// </summary>
        private int calibrationSpeechCount = 0;
        /// <summary>
        /// The kalibration
        /// </summary>
        private double[] kalibration;
        /// <summary>
        /// The last speech recognized
        /// </summary>
        private string lastSpeechRecognized = String.Empty;
        /// <summary>
        /// The word to calibrate
        /// </summary>
        private string wordToCalibrate = "start calibration";
        /// <summary>
        /// The threshold
        /// </summary>
        private double threshold = 0.0;
        /// <summary>
        /// The speech calibrated delegate
        /// </summary>
        private SpeechDelegate.SpeechCalibratedDelegate speechCalibratedDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechCalibrationHandler" /> class.
        /// </summary>
        /// <param name="speechCalibratedDelegate">The speech calibrated delegate.</param>
        public SpeechCalibrationHandler(SpeechDelegate.SpeechCalibratedDelegate speechCalibratedDelegate)
        {
            NetworkCommunicator.Instance.SendToServer(new WSMessage("voice calibration", InteractionType.Voice, "open voice calibration", RecognizedPerson.recognizedPerson));
            kalibration = new double[9];
            this.speechCalibratedDelegate = speechCalibratedDelegate;
        }

        /// <summary>
        /// Speeches the recognized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognizedEventArgs" /> instance containing the event data.</param>
        public void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            HandleCalibrationEvents(sender, e);
        }

        /// <summary>
        /// Speeches the rejected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognitionRejectedEventArgs" /> instance containing the event data.</param>
        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the calibration events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void HandleCalibrationEvents(object sender, EventArgs e)
        {
            threshold = (double)((SpeechRecognizedEventArgs)e).Result.Confidence;
            // Noise cancelling
            // Everything below 0.30 is basically nonsense or random people yelling, so we don't even process it.
            if (threshold > 0.30)
            {
                if (calibrationSpeechCount < 3)
                {
                    wordToCalibrate = "close mail";
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
                    NetworkCommunicator.Instance.SendToServer(new WSMessage("voice calibration", InteractionType.Voice, wordToCalibrate, RecognizedPerson.recognizedPerson));
                }
                else if (calibrationSpeechCount < 6)
                {
                    wordToCalibrate = "tumbleweed";
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
                    NetworkCommunicator.Instance.SendToServer(new WSMessage("voice calibration", InteractionType.Voice, wordToCalibrate, RecognizedPerson.recognizedPerson));
                }
                else if (calibrationSpeechCount < 9)
                {
                    wordToCalibrate = "colonel";
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
                    NetworkCommunicator.Instance.SendToServer(new WSMessage("voice calibration", InteractionType.Voice, wordToCalibrate, RecognizedPerson.recognizedPerson));
                }
                else
                {
                    double firstAverage = 0.0;
                    double secondAverage = 0.0;
                    double thirdAverage = 0.0;
                    for (int i = 0; i < 3; i++)
                    {
                        firstAverage += kalibration[i];
                        secondAverage += kalibration[i + 3];
                        thirdAverage += kalibration[i + 6];
                    }
                    firstAverage /= 3;
                    secondAverage /= 3;
                    thirdAverage /= 3;

                    firstAverage = Math.Min(firstAverage, thirdAverage);
                    threshold = Math.Min(firstAverage, secondAverage);

                    if (threshold > 0.80)
                    {
                        threshold = 0.80;
                    }
                    NetworkCommunicator.Instance.SendToServer(new WSMessage("voice calibration", InteractionType.Voice, "finish", RecognizedPerson.recognizedPerson));
                    Console.WriteLine("===== Calibration end   ====== " + threshold);
                    speechCalibratedDelegate(threshold);
                }

            }
            else
            {
                Console.WriteLine("Say " + wordToCalibrate + " again plz");
            }
        }
    }
}
