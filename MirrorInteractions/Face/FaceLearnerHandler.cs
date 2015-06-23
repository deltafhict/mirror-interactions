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
        /// The learn new faces
        /// </summary>
        private static bool learnNewFaces = false;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceRecognizedHandler"/> class.
        /// </summary>
        public FaceLearnerHandler()
        {
            this.faceLearner = new FaceLearner();
            this.faceLoader = new FaceLoader();
        }

        /// <summary>
        /// Faces the recognition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void FaceRecognized(object sender, Sacknet.KinectFacialRecognition.RecognitionResult e)
        {

            if (e.Faces != null)
            {
                face = e.Faces.FirstOrDefault();
            }

            using (var processedBitmap = (Bitmap)e.ColorSpaceBitmap.Clone())
            {
                if (face != null)
                {
                    Console.WriteLine("face recognized and learned");
                    if (newLearnedFacesCount != 2)
                    {
                        newLearnedFacesCount++;
                        faceLearner.LearnNewFaces(e, personName);
                    }
                    else
                    {
                        FaceRecognition.Instance.OpenFacialRecognitionEngine();
                        faceLoader.LoadAllTargetFaces();
                    }
                }
                // Without an explicit call to GC.Collect here, memory runs out of control :(
                GC.Collect();
            }
        }
    }
}
