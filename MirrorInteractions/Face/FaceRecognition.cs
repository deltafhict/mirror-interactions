﻿// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="FaceRecognition.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used to recognize the detected new face.</summary>
// ***********************************************************************
using Microsoft.Kinect;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;

/// <summary>
/// The Face namespace, all face related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class used to recognize the detected new face.
    /// </summary>
    public class FaceRecognition
    {
        /// <summary>
        /// The instance of FaceRecognition
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
        /// Store for the faceRecognizedEvent property
        /// </summary>
        private EventHandler<RecognitionResult> faceLearnerEvent;

        /// <summary>
        /// The face learner handler
        /// </summary>
        private FaceLearnerHandler faceLearnerHandler;

        /// <summary>
        /// The class constructor.
        /// </summary>
        private FaceRecognition()
        {
            this.activeProcessor = EigenObjectRecognitionProcessor.Instance;
            faceLearnerHandler = new FaceLearnerHandler();
            FaceRecognizedHandler faceRecognizedHandler = new FaceRecognizedHandler();
            this.faceRecognizedEvent = faceRecognizedHandler.FaceRecognized;
            this.faceLearnerEvent = faceLearnerHandler.FaceRecognized;
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

        /// <summary>
        /// Opens the facial recognition engine.
        /// </summary>
        public void OpenFacialRecognitionEngine()
        {
            this.facialRecognitionEngine.RecognitionComplete -= this.faceLearnerEvent;
            this.facialRecognitionEngine.RecognitionComplete += this.faceRecognizedEvent;
        }

        /// <summary>
        /// Learns the new faces.
        /// </summary>
        /// <param name="name">The name.</param>
        public void LearnNewFaces(string name)
        {
            faceLearnerHandler.PersonName = name;
            this.facialRecognitionEngine.RecognitionComplete -= this.faceRecognizedEvent;
            this.facialRecognitionEngine.RecognitionComplete += this.faceLearnerEvent;
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
