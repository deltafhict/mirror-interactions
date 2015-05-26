using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.Linq;
using MirrorInteractions;
using MirrorInteractions.Network;
using System.Diagnostics;
using MirrorGesture;

namespace MirrorGesture
{
    public class GestureRecognition
    {
        KinectSensor sensor;
        Body[] bodies;
        BodyFrameReader bodyReader;
        VisualGestureBuilderFrameSource gestureSource;
        VisualGestureBuilderFrameReader gestureReader;

        GestureRecognizedHandler gestureRecognizedHandler;

        public GestureRecognition(KinectSensor sensor)
        {
            this.sensor = sensor;
        }

        public void InitializeReaders()
        {
            OpenBodyReader();
            OpenGestureReader();
            this.gestureRecognizedHandler = new GestureRecognizedHandler(this.bodies, this.gestureSource, this.gestureReader);
            this.bodyReader.FrameArrived += gestureRecognizedHandler.OnBodyFrameArrived;
        }

        private void OpenBodyReader()
        {
            if (this.bodies == null)
            {
                this.bodies = new Body[this.sensor.BodyFrameSource.BodyCount];
            }
            this.bodyReader = this.sensor.BodyFrameSource.OpenReader();
        }

        private void OpenGestureReader()
        {
            this.gestureSource = new VisualGestureBuilderFrameSource(this.sensor, 0);
            this.gestureReader = this.gestureSource.OpenReader();
            this.gestureReader.IsPaused = true;
        }

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
