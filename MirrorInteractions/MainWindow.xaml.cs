//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MirrorInteractions
{
    using Microsoft.Kinect;
    using MirrorInteractions.Face;
    using MirrorInteractions.Gestures;
    using MirrorInteractions.Speech;
    using System.ComponentModel;
    using System.Windows;
    using System.IO;
    using System.Text;
    using System;
    using Microsoft.Speech.Recognition;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor kinectSensor = null;

        private SpeechRecognition speechRecognition;

        private FaceRecognition faceRecognition;

        private GestureRecognition gestureRecognition;

        private int calibrationSpeechCount = 0;
        private string lastSpeechRecognized = String.Empty;
        private double step = 0.05;
        private double threshold = 0.0;

        public MainWindow()
        {
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                speechRecognition = new SpeechRecognition(kinectSensor);
                faceRecognition = new FaceRecognition(kinectSensor);
                gestureRecognition = new GestureRecognition(kinectSensor);
            }
            else
            {
                return;
            }

            InitializeComponent();

            // Set up speech recognition for calibration
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.CalibrationSpeechGrammar));

            speechRecognition.OpenSpeechRecognitionEngine(memoryStream, SpeechRecognized, SpeechRejected);

            // start calibration
            Console.WriteLine("===== Calibration start ======");
            Console.WriteLine("Say 'weather' plz");

            this.Hide();
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            HandleCalibrationEvents(sender, e);
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            HandleCalibrationEvents(sender, e);
        }

        private void HandleCalibrationEvents(object sender, EventArgs e)
        {
            if (calibrationSpeechCount == 0)
            {
                threshold = ((SpeechRecognizedEventArgs)e).Result.Confidence;
            }

            // recognized + uitspraak good enough
            if (e is SpeechRecognizedEventArgs && ((SpeechRecognizedEventArgs)e).Result.Confidence >= threshold)
            {
                if ("yes" == lastSpeechRecognized) // no kantelpunt
                {
                    step = Math.Abs(step);
                    threshold += step;
                    Console.WriteLine("yes - no kantel. step: " + step + " threshold: " + threshold);
                }
                else if (calibrationSpeechCount > 0)// wel kantelpunt
                {
                    step = Math.Abs(step);
                    step *= -0.5;
                    threshold -= step;
                    Console.WriteLine("yes - wel kantel. step: " + step + " threshold: " + threshold);
                }

                calibrationSpeechCount++;
                lastSpeechRecognized = "yes";
            }
            else // not recognized or uitspraak not good enough
            {
                if ("no" == lastSpeechRecognized) // no kantelpunt
                {
                    step = Math.Abs(step);
                    threshold -= step;
                    Console.WriteLine("no - no kantel. step: " + step + " threshold: " + threshold);
                }
                else if (calibrationSpeechCount > 0)// wel kantelpunt
                {
                    step = Math.Abs(step);
                    step *= -0.5;
                    threshold += step;
                    Console.WriteLine("no - wel kantel. step: " + step + " threshold: " + threshold);
                }

                calibrationSpeechCount++;
                lastSpeechRecognized = "no";
            }

            if (Math.Abs(step) < 0.015)
            {
                // Calibration is done, current old speech engine
                speechRecognition.CloseSpeechRecognitionEngine();

                // and make a new one with the new grammar
                var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar));
                SpeechRecognizedHandler speechRecognizedHandler = new SpeechRecognizedHandler();

                if (threshold > 0.80)
                {
                    threshold = 0.80;
                }
                speechRecognizedHandler.ConfidenceThreshold = threshold;

                speechRecognition.OpenSpeechRecognitionEngine(memoryStream, speechRecognizedHandler.SpeechRecognized, speechRecognizedHandler.SpeechRejected);
                faceRecognition.OpenFacialRecognitionEngine();
                gestureRecognition.InitializeReaders();

                Console.WriteLine("===== Calibration end   ======");
            }
            else
            {
                Console.WriteLine("Say 'weather' again plz");
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            speechRecognition.CloseSpeechRecognitionEngine();
            faceRecognition.CloseFacialRecognitionEngine();
            gestureRecognition.CloseReaders();

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

    }
}