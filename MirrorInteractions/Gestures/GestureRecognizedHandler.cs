using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using MirrorInteractions;
using MirrorInteractions.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorGesture
{
    class GestureRecognizedHandler
    {
        Body[] bodies;
        VisualGestureBuilderFrameSource gestureSource;
        VisualGestureBuilderFrameReader gestureReader;

        bool gestureComplete = false;
        public GestureDatabase gestureDatabase = null;

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

        public void OnTrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            this.gestureReader.IsPaused = true;
        }

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
                            WSMessage messageToSend = new WSMessage
                            {
                                action = "DragToLeft",
                                app = "gesture"
                            };
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
                            WSMessage messageToSend = new WSMessage
                            {
                                action = "DragToRight",
                                app = "gesture"
                            };
                            NetworkCommunicator.SendToServer(messageToSend);
                        }
                    }
                }
            }
        }
    }
}
