// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="FaceRecognizedHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using Sacknet.KinectFacialRecognition;
using System;
using System.Drawing;
using System.Linq;
using System.Timers;

/// <summary>
/// The Face namespace.
/// </summary>
namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class FaceRecognizedHandler.
    /// </summary>
    public class FaceLearnerHandler
    {
        /// <summary>
        /// The face
        /// </summary>
        private TrackedFace face = null;
        /// <summary>
        /// The new learned faces count
        /// </summary>
        private int newLearnedFacesCount = 0;
        /// <summary>
        /// The person name
        /// </summary>
        private static string personName = null;
        /// <summary>
        /// The face learner
        /// </summary>
        private FaceLearner faceLearner;
        /// <summary>
        /// The face loader
        /// </summary>
        private FaceLoader faceLoader;

        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceRecognizedHandler"/> class.
        /// </summary>
        public FaceLearnerHandler()
        {
            this.faceLearner = new FaceLearner();
            this.faceLoader = new FaceLoader();
            timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += (s, e1) =>
            {
                timer.Stop();
            };
        }

        public string PersonName
        {
            set
            {
                personName = value;
            }
        }

        /// <summary>
        /// Faces the recognition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void FaceRecognized(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
        {
            if (personName == null)
            {
                NetworkCommunicator.Instance.SendToServer(new WSMessage("face calibration", InteractionType.FaceRecognition, "fail", personName));
                FaceRecognition.Instance.OpenFacialRecognitionEngine();
                faceLoader.LoadAllTargetFaces();
                return;
            }

            if (e.Faces != null)
            {
                face = e.Faces.FirstOrDefault();
            }

            using (var processedBitmap = (Bitmap)e.ColorSpaceBitmap.Clone())
            {
                if (face != null)
                {
                    if (!timer.Enabled)
                    {
                        if (newLearnedFacesCount < 1)
                        {
                            Console.WriteLine("Starting face learning");
                            NetworkCommunicator.Instance.SendToServer(new WSMessage("face learning", InteractionType.FaceRecognition, "start", personName));
                            timer.Start();
                            newLearnedFacesCount++;
                            return;
                        }
                        String action = "forward";
                        switch (newLearnedFacesCount)
                        {
                            case 1: 
                                action = "forward";
                                break;
                            case 2:
                                action = "left";
                                break;
                            case 3:
                                action = "right";
                                break;
                            case 4:
                                action = "down";
                                break;
                            case 5:
                                action = "forward";
                                break;
                            case 6:
                                FaceRecognition.Instance.OpenFacialRecognitionEngine();
                                faceLoader.LoadAllTargetFaces();
                                Console.WriteLine("Finished learning");
                                return;
                            default:
                                break;
                        }
                        newLearnedFacesCount++;
                        faceLearner.LearnNewFaces(e, personName);
                        Console.WriteLine("Face with name: " + personName + " learned looking " + action);
                        NetworkCommunicator.Instance.SendToServer(new WSMessage("face calibration", InteractionType.FaceRecognition, action, personName));
                        timer.Start();
                    }
                }
                // Without an explicit call to GC.Collect here, memory runs out of control :(
                GC.Collect();
            }
        }
    }
}
