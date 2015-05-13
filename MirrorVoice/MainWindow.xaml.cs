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
    using System.Web.Script.Serialization;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using WebSocketSharp;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Types { voice, speech, faceRecognition }

        private readonly String serverAdress = "ws://127.0.0.1:1337";
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

        /// <summary>
        /// Facial recognition engine using face detection from Kinect.
        /// </summary>
        private KinectFacialRecognitionEngine facialRecognitionEngine;

        private IRecognitionProcessor activeProcessor;

        List<BitmapSourceTargetFace> faces = new List<BitmapSourceTargetFace>();

        public MainWindow()
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

            InitializeComponent();

            LoadFacialRecognitionEngine();
            LoadVoiceDetectionEngine();
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

        private void LoadFacialRecognitionEngine()
        {
            this.activeProcessor = new EigenObjectRecognitionProcessor();

            this.LoadAllTargetFaces();
            this.UpdateTargetFaces();

            if (this.facialRecognitionEngine == null)
            {
                this.facialRecognitionEngine = new KinectFacialRecognitionEngine(this.kinectSensor, this.activeProcessor);
                this.facialRecognitionEngine.RecognitionComplete += this.Engine_RecognitionComplete;
            }

            this.facialRecognitionEngine.Processors = new List<IRecognitionProcessor> { this.activeProcessor };
        }

        private void Engine_RecognitionComplete(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
        {
            TrackedFace face = null;

            if (e.Faces != null)
            {
                face = e.Faces.FirstOrDefault();
            }

            using (var processedBitmap = (Bitmap)e.ColorSpaceBitmap.Clone())
            {
                if (face != null)
                {
                    using (var g = Graphics.FromImage(processedBitmap))
                    {
                        var rect = face.TrackingResult.FaceRect;

                        if (!string.IsNullOrEmpty(face.Key))
                        {
                            var score = Math.Round(face.ProcessorResults.First().Score, 2);

                            // Write the key on the image...
                            MessageBox.Show(face.Key + " " + score);
                        }
                    }
                }
                // Without an explicit call to GC.Collect here, memory runs out of control :(
                GC.Collect();
            }
        }

        private void LoadVoiceDetectionEngine()
        {
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
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.facialRecognitionEngine.RecognitionComplete -= this.Engine_RecognitionComplete;
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
        private void SpeechRecognized(object sender1, SpeechRecognizedEventArgs e1)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e1.Result.Confidence >= ConfidenceThreshold)
            {
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
                            app = e1.Result.Semantics.Value.ToString(),
                            type = Types.voice.ToString()
                        };
                        SendToServer(messageToSend);
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
            Console.WriteLine("Speech rejected");
        }

        /// <summary>
        /// Loads all BSTFs from the current directory
        /// </summary>
        private void LoadAllTargetFaces()
        {
            //this.viewModel.TargetFaces.Clear();
            var result = new List<BitmapSourceTargetFace>();
            var suffix = ".pca";

            foreach (var file in Directory.GetFiles(".", "TF_*" + suffix))
            {
                var bstf = JsonConvert.DeserializeObject<BitmapSourceTargetFace>(File.ReadAllText(file));
                bstf.Image = (Bitmap)Bitmap.FromFile(file.Replace(suffix, ".png"));
                faces.Add(bstf);
            }
        }

        private void UpdateTargetFaces()
        {
            if (this.faces.Count > 1)
                this.activeProcessor.SetTargetFaces(faces);
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Loads a bitmap into a bitmap source
        /// </summary>
        public static BitmapSource LoadBitmap(Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                bs.Freeze();
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

        private void SendToServer(WSMessage wsMessage)
        {
            String json = new JavaScriptSerializer().Serialize(wsMessage);
            using (var ws = new WebSocket(serverAdress))
            {
                ws.Connect();
                ws.Send(json);
            }
        }
    }
}