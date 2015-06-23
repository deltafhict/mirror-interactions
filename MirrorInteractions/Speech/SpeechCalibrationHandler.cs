// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 06-09-2015
//
// Last Modified By : delta
// Last Modified On : 06-09-2015
// ***********************************************************************
// <copyright file="SpeechCalibrationHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Speech namespace.
/// </summary>
namespace MirrorInteractions.Speech
{
    /// <summary>
    /// Class SpeechCalibrationHandler.
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
        private string wordToCalibrate = "close mail";
        /// <summary>
        /// The threshold
        /// </summary>
        private double threshold = 0.0;
        /// <summary>
        /// The speech calibrated delegate
        /// </summary>
        private SpeechDelegate.SpeechCalibratedDelegate speechCalibratedDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechCalibrationHandler"/> class.
        /// </summary>
        /// <param name="speechCalibratedDelegate">The speech calibrated delegate.</param>
        public SpeechCalibrationHandler(SpeechDelegate.SpeechCalibratedDelegate speechCalibratedDelegate)
        {
            kalibration = new double[9];
            this.speechCalibratedDelegate = speechCalibratedDelegate;
        }

        /// <summary>
        /// Speeches the recognized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        public void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            HandleCalibrationEvents(sender, e);
        }

        /// <summary>
        /// Speeches the rejected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpeechRecognitionRejectedEventArgs"/> instance containing the event data.</param>
        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            HandleCalibrationEvents(sender, e);
        }

        /// <summary>
        /// Handles the calibration events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HandleCalibrationEvents(object sender, EventArgs e)
        {
            threshold = (double)((SpeechRecognizedEventArgs)e).Result.Confidence;
            // Noise cancelling
            // Everything below 0.30 is basically nonsense or rando people yelling, so we don't even process it.
            if (threshold > 0.30)
            {
                if (calibrationSpeechCount < 3)
                {
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
                }
                else if (calibrationSpeechCount < 6)
                {
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    wordToCalibrate = "thumbleweed";
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
                }
                else if (calibrationSpeechCount < 9)
                {
                    Console.WriteLine(threshold + " " + calibrationSpeechCount);
                    wordToCalibrate = "colonel";
                    Console.WriteLine("Say " + wordToCalibrate + " please.");
                    kalibration[calibrationSpeechCount] = threshold;
                    calibrationSpeechCount++;
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
