using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions.Speech
{
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

        public SpeechRecognition(KinectSensor kinectSensor)
        {
            SpeechRecognizedHandler speechRecognizedHandler = new SpeechRecognizedHandler();
            // grab the audio stream from the kinect
            IReadOnlyList<AudioBeam> audioBeamList = kinectSensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            this.kinectAudioStream = new KinectAudioStream(audioStream);
            this.speechRecognizedEvent = speechRecognizedHandler.SpeechRecognized;
            this.speechRejectedEvent = speechRecognizedHandler.SpeechRejected;
        }

        public void OpenSpeechRecognitionEngine()
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

                this.speechEngine.SpeechRecognized += speechRecognizedEvent;
                this.speechEngine.SpeechRecognitionRejected += speechRejectedEvent;

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
