//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    using System;
    using System.Collections.Generic;    
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;    
    using System.Windows;    
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Kinect;    
    using Microsoft.Speech.AudioFormat;
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
        /// Stream for 32b-16b conversion.
        /// </summary>
        private KinectAudioStream convertStream = null;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);
            }
            else
            {
                // on failure, set the status text
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar));

                // Create a grammar from grammar definition XML file.
                using (memoryStream)
                {
                    var g = new Grammar(memoryStream);
                    this.speechEngine.LoadGrammar(g);
                }

                this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                //this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
                if (null != this.convertStream)
                {
                    this.convertStream.SpeechActive = false;
                }

                if (null != this.speechEngine)
                {
                    this.speechEngine.SpeechRecognized -= this.SpeechRecognized;
                    this.speechEngine.SpeechRecognitionRejected -= this.SpeechRejected;
                    this.speechEngine.RecognizeAsyncStop();
                }

                if (null != this.kinectSensor)
                {
                    this.kinectSensor.Close();
                    this.kinectSensor = null;
                }
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "AGENDA":
                        MessageBox.Show("Agenda called");
                        break;
                }
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            MessageBox.Show("SpeechRejected");
        }
    }
}