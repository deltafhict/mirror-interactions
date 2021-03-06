﻿// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Main class used to start the speech and face engine.</summary>
// ***********************************************************************

/// <summary>
/// The MirrorInteractions namespace.
/// </summary>
namespace MirrorInteractions
{
    using Microsoft.Kinect;
    using MirrorInteractions.Face;
    using MirrorInteractions.Speech;
    using System.ComponentModel;
    using System.Windows;
    using System.IO;
    using System.Text;
    using System;
    using Microsoft.Speech.Recognition;

    /// <summary>
    /// Main class used to start the speech and face engine.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// The speech recognition
        /// </summary>
        private SpeechRecognition speechRecognition;

        /// <summary>
        /// The face recognition
        /// </summary>
        private FaceRecognition faceRecognition;

        /// <summary>
        /// The default threshold for speech recognition
        /// </summary>
        private readonly static double defaultSpeechThreshold = 0.50;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                speechRecognition = new SpeechRecognition(kinectSensor);
                faceRecognition = FaceRecognition.Instance;
            }
            else
            {
                return;
            }

            InitializeComponent();
            speechRecognition.InitializeSpeechRecognition(defaultSpeechThreshold);
            faceRecognition.InitializeFacialRecognitionEngine(kinectSensor);
            // Hide the main window, we don't use the UI anyway
            this.Hide();
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            speechRecognition.CloseSpeechRecognitionEngine();
            faceRecognition.CloseFacialRecognitionEngine();

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

    }
}