// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-10-2015
// ***********************************************************************
// <copyright file="FaceRecognition.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Kinect;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;

/// <summary>
/// The Face namespace.
/// </summary>
namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class FaceRecognition.
    /// </summary>
    public class FaceRecognition
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static FaceRecognition instance;

        /// <summary>
        /// Store for the facialRecognitionEngine property.
        /// </summary>
        private KinectFacialRecognitionEngine facialRecognitionEngine;

        /// <summary>
        /// Store for the activeProcessor property
        /// </summary>
        private IRecognitionProcessor activeProcessor;

        /// <summary>
        /// Store for the faceRecognizedEvent property
        /// </summary>
        private EventHandler<RecognitionResult> faceRecognizedEvent;

        /// <summary>
        /// The class constructor.
        /// </summary>
        private FaceRecognition()
        {
            this.activeProcessor = EigenObjectRecognitionProcessor.Instance;
            FaceRecognizedHandler faceRecognizedHandler = new FaceRecognizedHandler();
            this.faceRecognizedEvent = faceRecognizedHandler.FaceRecognized;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static FaceRecognition Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FaceRecognition();
                }
                return instance;
            }
        }

        /// <summary>
        /// Opens the facial recognition engine.
        /// </summary>
        /// <param name="kinectSensor">The kinect sensor.</param>
        public void InitializeFacialRecognitionEngine(KinectSensor kinectSensor)
        {
            if (this.facialRecognitionEngine == null)
            {
                this.facialRecognitionEngine = new KinectFacialRecognitionEngine(kinectSensor, this.activeProcessor);
                OpenFacialRecognitionEngine();
            }

            this.facialRecognitionEngine.Processors = new List<IRecognitionProcessor> { this.activeProcessor };
        }

        public void OpenFacialRecognitionEngine()
        {
            this.facialRecognitionEngine.RecognitionComplete += this.faceRecognizedEvent;
        }

        public void LearnNewFaces(string name)
        {
            FaceLearnerHandler faceLearnerHandler = new FaceLearnerHandler();
            this.facialRecognitionEngine.RecognitionComplete += faceLearnerHandler.FaceRecognized;
        }

        /// <summary>
        /// Closes the facial recognition engine.
        /// </summary>
        public void CloseFacialRecognitionEngine()
        {
            this.facialRecognitionEngine.RecognitionComplete -= this.faceRecognizedEvent;
        }
    }
}
