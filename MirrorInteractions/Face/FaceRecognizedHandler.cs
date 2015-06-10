﻿// ***********************************************************************
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
    public class FaceRecognizedHandler
    {

        /// <summary>
        /// The face recognition expire timer
        /// </summary>
        private Timer faceRecognitionExpireTimer;
        /// <summary>
        /// The face
        /// </summary>
        private TrackedFace face = null;
        /// <summary>
        /// The face loader
        /// </summary>
        private FaceLoader faceLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceRecognizedHandler"/> class.
        /// </summary>
        public FaceRecognizedHandler()
        {
            this.faceLoader = new FaceLoader();
            faceLoader.LoadAllTargetFaces();
        }

        /// <summary>
        /// Faces the recognition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void FaceRecognized(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
        {
            //Console.WriteLine("face detected");

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

        /// <summary>
        /// Handles the <see cref="E:FaceRecognizedExpired" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnFaceRecognizedExpired(object sender, ElapsedEventArgs e)
        {
            faceRecognitionExpireTimer.Stop();
            RecognizedPerson.recognizedPerson = "unknown";
        }

    }
}
