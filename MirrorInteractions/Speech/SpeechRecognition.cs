// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="SpeechRecognition.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used for speech recognition.</summary>
// ************************************************************************
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// The Speech namespace, all Speech related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Speech
{
    /// <summary>
    /// Class used for speech recognition.
    /// </summary>
    public class SpeechRecognition
    {
        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;

        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
        private KinectAudioStream kinectAudioStream;

        /// <summary>
        /// Eventhandler to handle the speech recognized event
        /// </summary>
        private EventHandler<SpeechRecognizedEventArgs> speechRecognizedEvent;

        /// <summary>
        /// Eventhandler to handle the speech rejected event
        /// </summary>
        private EventHandler<SpeechRecognitionRejectedEventArgs> speechRejectedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechRecognition" /> class.
        /// </summary>
        /// <param name="kinectSensor">The kinect sensor.</param>
        public SpeechRecognition(KinectSensor kinectSensor)
        {
            // grab the audio stream from the kinect
            IReadOnlyList<AudioBeam> audioBeamList = kinectSensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();
            
            this.kinectAudioStream = new KinectAudioStream(audioStream);
        }

        /// <summary>
        /// Initializes the speech recognition.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        public void InitializeSpeechRecognition(double threshold)
        {
            // and make a new one with the new grammar
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar));
            SpeechRecognizedHandler speechRecognizedHandler = new SpeechRecognizedHandler(new SpeechDelegate.SpeechCalibrateDelegate(InitializeSpeechCalibration));
            speechRecognizedHandler.ConfidenceThreshold = threshold;
            OpenSpeechRecognitionEngine(memoryStream, speechRecognizedHandler.SpeechRecognized, speechRecognizedHandler.SpeechRejected);
        }

        /// <summary>
        /// Initializes the speech calibration.
        /// </summary>
        public void InitializeSpeechCalibration()
        {
            // Set up speech recognition for calibration
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.CalibrationSpeechGrammar));
            SpeechCalibrationHandler speechCalibrationHandler = new SpeechCalibrationHandler(new SpeechDelegate.SpeechCalibratedDelegate(InitializeSpeechRecognition));
            OpenSpeechRecognitionEngine(memoryStream, speechCalibrationHandler.SpeechRecognized, speechCalibrationHandler.SpeechRejected);
            // start calibration
            Console.WriteLine("===== Calibration start ======");
            Console.WriteLine("Say start calibration' plz");
        }

        /// <summary>
        /// Opens the speech recognition engine.
        /// </summary>
        /// <param name="memoryStream">The memory stream.</param>
        /// <param name="speechRecognizedEvent">The speech recognized event.</param>
        /// <param name="speechRejectedEvent">The speech rejected event.</param>
        public void OpenSpeechRecognitionEngine(MemoryStream memoryStream, EventHandler<SpeechRecognizedEventArgs> speechRecognizedEvent, EventHandler<SpeechRecognitionRejectedEventArgs> speechRejectedEvent)
        {
            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                // Create a grammar from grammar definition XML file.
                using (memoryStream)
                {
                    var g = new Grammar(memoryStream);
                    this.speechEngine.LoadGrammar(g);
                }

                this.speechEngine.SpeechRecognized += speechRecognizedEvent;
                this.speechEngine.SpeechRecognitionRejected += speechRejectedEvent;

                this.speechRecognizedEvent = speechRecognizedEvent;
                this.speechRejectedEvent = speechRejectedEvent;

                // let the convertStream know speech is going active
                this.kinectAudioStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                //speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.kinectAudioStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>RecognizerInfo if found, <code>null</code> otherwise.</returns>
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

        /// <summary>
        /// Closes the speech recognition engine.
        /// </summary>
        public void CloseSpeechRecognitionEngine()
        {

            if (null != this.kinectAudioStream)
            {
                this.kinectAudioStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= speechRecognizedEvent;
                this.speechEngine.SpeechRecognitionRejected -= speechRejectedEvent;
                this.speechEngine.RecognizeAsyncStop();
            }
        }
    }
}
