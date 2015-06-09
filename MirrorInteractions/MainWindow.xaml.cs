// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-09-2015
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

/// <summary>
/// The MirrorInteractions namespace.
/// </summary>
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

        /// <summary>
        /// The speech recognition
        /// </summary>
        private SpeechRecognition speechRecognition;

        /// <summary>
        /// The face recognition
        /// </summary>
        private FaceRecognition faceRecognition;

        /// <summary>
        /// The gesture recognition
        /// </summary>
        private GestureRecognition gestureRecognition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
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
                faceRecognition = new FaceRecognition(kinectSensor);
                gestureRecognition = new GestureRecognition(kinectSensor);
            }
            else
            {
                return;
            }

            InitializeComponent();
            speechRecognition.InitializeSpeechCalibration();
            faceRecognition.OpenFacialRecognitionEngine();
            gestureRecognition.InitializeReaders();
            // Hide the main window, we don't use the UI anyway
            this.Hide();
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
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