// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
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

namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class FaceRecognition.
    /// </summary>
    public class FaceRecognition
    {
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
        EventHandler<RecognitionResult> faceRecognizedEvent;

        /// <summary>
        /// Store for the kinectSensor property
        /// </summary>
        KinectSensor kinectSensor;

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="kinectSensor">The kinect sensor initialized in MainWindow.cs.</param>
        public FaceRecognition(KinectSensor kinectSensor)
        {
            this.kinectSensor = kinectSensor;
            this.activeProcessor = EigenObjectRecognitionProcessor.Instance;
            FaceRecognizedHandler faceRecognizedHandler = new FaceRecognizedHandler();
            this.faceRecognizedEvent = faceRecognizedHandler.FaceRecognition;
        }

        /// <summary>
        /// Opens the facial recognition engine.
        /// </summary>
        public void OpenFacialRecognitionEngine()
        {

            if (this.facialRecognitionEngine == null)
            {
                this.facialRecognitionEngine = new KinectFacialRecognitionEngine(this.kinectSensor, this.activeProcessor);
                this.facialRecognitionEngine.RecognitionComplete += this.faceRecognizedEvent;
            }

            this.facialRecognitionEngine.Processors = new List<IRecognitionProcessor> { this.activeProcessor };
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
