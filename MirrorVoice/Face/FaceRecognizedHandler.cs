using MirrorInteractions.Models;
using Sacknet.KinectFacialRecognition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MirrorInteractions.Face
{
    public class FaceRecognizedHandler
    {

        private Timer faceRecognitionExpireTimer;

        public void FaceRecognition(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
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
                            if (score > 1000)
                            {

                                Console.WriteLine("face recognized " + face.Key);
                                faceRecognitionExpireTimer = new Timer(60000);
                                faceRecognitionExpireTimer.Elapsed += new ElapsedEventHandler(OnFaceRecognizedExpired);
                                faceRecognitionExpireTimer.AutoReset = false;
                                faceRecognitionExpireTimer.Enabled = true;
                                RecognizedPerson.recognizedPerson = face.Key;
                            }
                        }
                    }
                }
                // Without an explicit call to GC.Collect here, memory runs out of control :(
                GC.Collect();
            }
        }

        private void OnFaceRecognizedExpired(object sender, ElapsedEventArgs e)
        {
            faceRecognitionExpireTimer.Stop();
            RecognizedPerson.recognizedPerson = "unknown";
        }

    }
}
