using MirrorInteractions.Models;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
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
        private TrackedFace face = null;
        private int newLearnedFacesCount = 0;
        private static bool learnNewFaces = false;
        private static string personName = null;
        private FaceLearner faceLearner;
        private FaceLoader faceLoader;

        public FaceRecognizedHandler()
        {
            this.faceLearner = new FaceLearner();
            this.faceLoader = new FaceLoader();
            faceLoader.LoadAllTargetFaces();
        }

        public void FaceRecognition(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
        {
            Console.WriteLine("face detected");

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

                    if (learnNewFaces && newLearnedFacesCount != 20)
                    {
                        faceLearner.LearnNewFaces(e, personName);
                    }
                    else if (newLearnedFacesCount == 20)
                    {
                        faceLoader.LoadAllTargetFaces();
                        learnNewFaces = false;
                        newLearnedFacesCount = 0;
                    }
                }
                // Without an explicit call to GC.Collect here, memory runs out of control :(
                GC.Collect();
            }
        }

        public static void SetLearnNewFaces(String personName)
        {
            learnNewFaces = true;
        }

        private void OnFaceRecognizedExpired(object sender, ElapsedEventArgs e)
        {
            faceRecognitionExpireTimer.Stop();
            RecognizedPerson.recognizedPerson = "unknown";
        }

    }
}
