// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="GestureRecognizedHandler.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>s
// ***********************************************************************
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using MirrorInteractions.Models;
using MirrorInteractions.Network;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// The MirrorGesture namespace.
/// </summary>
namespace MirrorInteractions.Gestures
{
    /// <summary>
    /// Class GestureRecognizedHandler.
    /// </summary>
    public class GestureRecognizedHandler
    {
        /// <summary>
        /// The bodies
        /// </summary>
        Body[] bodies;
        /// <summary>
        /// The gesture source
        /// </summary>
        VisualGestureBuilderFrameSource gestureSource;
        /// <summary>
        /// The gesture reader
        /// </summary>
        VisualGestureBuilderFrameReader gestureReader;

        /// <summary>
        /// The gesture complete
        /// </summary>
        bool gestureComplete = false;
        /// <summary>
        /// The gesture database
        /// </summary>
        public GestureDatabase gestureDatabase = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureRecognizedHandler"/> class.
        /// </summary>
        /// <param name="bodies">The bodies.</param>
        /// <param name="gestureSource">The gesture source.</param>
        /// <param name="gestureReader">The gesture reader.</param>
        public GestureRecognizedHandler(Body[] bodies, VisualGestureBuilderFrameSource gestureSource, VisualGestureBuilderFrameReader gestureReader)
        {
            this.gestureDatabase = new GestureDatabase();
            this.bodies = bodies;
            this.gestureSource = gestureSource;
            this.gestureReader = gestureReader;

            this.gestureSource.TrackingIdLost += OnTrackingIdLost;
            this.gestureReader.FrameArrived += OnGestureFrameArrived;

            this.gestureSource.AddGesture(gestureDatabase.dragToLeftGesture);
            this.gestureSource.AddGesture(gestureDatabase.dragToRightGesture);
        }

        /// <summary>
        /// Handles the <see cref="E:BodyFrameArrived"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BodyFrameArrivedEventArgs"/> instance containing the event data.</param>
        public void OnBodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(this.bodies);

                    var trackedBody = this.bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (trackedBody != null)
                    {
                        
                        if (this.gestureReader.IsPaused)
                        {
                            this.gestureSource.TrackingId = trackedBody.TrackingId;
                            this.gestureReader.IsPaused = false;
                        }
                    }
                    else
                    {
                        this.OnTrackingIdLost(null, null);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:TrackingIdLost"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TrackingIdLostEventArgs"/> instance containing the event data.</param>
        public void OnTrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            this.gestureReader.IsPaused = true;
        }

        /// <summary>
        /// Handles the <see cref="E:GestureFrameArrived"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="VisualGestureBuilderFrameArrivedEventArgs"/> instance containing the event data.</param>
        public void OnGestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    //Debug.WriteLine("Frame != null");
                    var continuousResults = frame.ContinuousGestureResults;

                    // DragToLeftGesture
                    if ((continuousResults != null) && (continuousResults.ContainsKey(gestureDatabase.dragToLeftGesture)))
                    {
                        var result = continuousResults[gestureDatabase.dragToLeftGesture];


                        if (gestureComplete)
                        {
                            if (result.Progress <= 0.3f)
                            {
                                gestureComplete = false;
                            }
                            return;
                        }

                        if (result.Progress >= 0.9f)
                        {
                            Debug.WriteLine("Drag to Left complete");
                            gestureComplete = true;
                            WSMessage messageToSend = new WSMessage("gesture", "dragToLeft");
                            NetworkCommunicator.SendToServer(messageToSend);
                        }
                    }

                    //DragToRight Gesture
                    if ((continuousResults != null) && (continuousResults.ContainsKey(gestureDatabase.dragToRightGesture)))
                    {
                        var result = continuousResults[gestureDatabase.dragToRightGesture];

                        if (gestureComplete)
                        {
                            if (result.Progress <= 0.3f)
                            {
                                gestureComplete = false;
                            }
                            return;
                        }

                        if (result.Progress >= 0.9f)
                        {
                            Debug.WriteLine("Drag to Right complete");
                            gestureComplete = true;
                            WSMessage messageToSend = new WSMessage("gesture", "DragToRight");

                            NetworkCommunicator.SendToServer(messageToSend);
                        }
                    }
                }
            }
        }
    }
}
