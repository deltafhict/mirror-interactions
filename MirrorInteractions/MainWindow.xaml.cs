//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MirrorInteractions
{
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using MirrorGesture;
    using MirrorInteractions.Face;
    using MirrorInteractions.Network;
    using MirrorInteractions.Speech;
    using Newtonsoft.Json;
    using Sacknet.KinectFacialRecognition;
    using Sacknet.KinectFacialRecognition.ManagedEigenObject;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Timers;
    using System.Web.Script.Serialization;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using WebSocketSharp;

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
            speechRecognition.OpenSpeechRecognitionEngine();
            faceRecognition.OpenFacialRecognitionEngine();
            gestureRecognition.InitializeReaders();
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