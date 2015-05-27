// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="GestureRecognition.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

/// <summary>
/// The MirrorGesture namespace.
/// </summary>
namespace MirrorInteractions.Gestures
{
    /// <summary>
    /// Class GestureRecognition.
    /// </summary>
    public class GestureRecognition
    {
        /// <summary>
        /// The sensor
        /// </summary>
        KinectSensor sensor;
        /// <summary>
        /// The bodies
        /// </summary>
        Body[] bodies;
        /// <summary>
        /// The body reader
        /// </summary>
        BodyFrameReader bodyReader;
        /// <summary>
        /// The gesture source
        /// </summary>
        VisualGestureBuilderFrameSource gestureSource;
        /// <summary>
        /// The gesture reader
        /// </summary>
        VisualGestureBuilderFrameReader gestureReader;

        /// <summary>
        /// The gesture recognized handler
        /// </summary>
        GestureRecognizedHandler gestureRecognizedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureRecognition"/> class.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        public GestureRecognition(KinectSensor sensor)
        {
            this.sensor = sensor;
        }

        /// <summary>
        /// Initializes the readers.
        /// </summary>
        public void InitializeReaders()
        {
            OpenBodyReader();
            OpenGestureReader();
            this.gestureRecognizedHandler = new GestureRecognizedHandler(this.bodies, this.gestureSource, this.gestureReader);
            this.bodyReader.FrameArrived += gestureRecognizedHandler.OnBodyFrameArrived;
        }

        /// <summary>
        /// Opens the body reader.
        /// </summary>
        private void OpenBodyReader()
        {
            if (this.bodies == null)
            {
                this.bodies = new Body[this.sensor.BodyFrameSource.BodyCount];
            }
            this.bodyReader = this.sensor.BodyFrameSource.OpenReader();
        }

        /// <summary>
        /// Opens the gesture reader.
        /// </summary>
        private void OpenGestureReader()
        {
            this.gestureSource = new VisualGestureBuilderFrameSource(this.sensor, 0);
            this.gestureReader = this.gestureSource.OpenReader();
            this.gestureReader.IsPaused = true;
        }

        /// <summary>
        /// Closes the readers.
        /// </summary>
        public void CloseReaders()
        {
            if (this.gestureReader != null)
            {
                this.gestureReader.FrameArrived -= gestureRecognizedHandler.OnGestureFrameArrived;
                this.gestureReader.Dispose();
                this.gestureReader = null;
            }
            if (this.gestureSource != null)
            {
                this.gestureSource.TrackingIdLost -= gestureRecognizedHandler.OnTrackingIdLost;
                this.gestureSource.Dispose();
            }
            this.bodyReader.Dispose();
            this.bodyReader = null;
        }
    }
}
