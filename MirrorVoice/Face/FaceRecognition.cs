using Microsoft.Kinect;
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MirrorInteractions.Face
{
    public class FaceRecognition
    {
        /// <summary>
        /// Store for the facialRecognitionEngine property. </summary>
        private KinectFacialRecognitionEngine facialRecognitionEngine;

        /// <summary>
        /// Store for the activeProcessor property</summary>
        private IRecognitionProcessor activeProcessor;

        /// <summary>
        /// Store for the faceRecognizedEvent property</summary>
        EventHandler<RecognitionResult> faceRecognizedEvent;

        /// <summary>
        /// Store for the kinectSensor property</summary>
        KinectSensor kinectSensor;

        /// <summary>
        /// The class constructor. </summary>
        /// <param name="kinectSensor">The kinect sensor initialized in MainWindow.cs. </param>
        public FaceRecognition(KinectSensor kinectSensor)
        {
            this.kinectSensor = kinectSensor;
            this.activeProcessor = EigenObjectRecognitionProcessor.Instance;
            FaceRecognizedHandler faceRecognizedHandler = new FaceRecognizedHandler();
            this.faceRecognizedEvent = faceRecognizedHandler.FaceRecognition;
        }

        public void OpenFacialRecognitionEngine()
        {

            if (this.facialRecognitionEngine == null)
            {
                this.facialRecognitionEngine = new KinectFacialRecognitionEngine(this.kinectSensor, this.activeProcessor);
                this.facialRecognitionEngine.RecognitionComplete += this.faceRecognizedEvent;
            }

            this.facialRecognitionEngine.Processors = new List<IRecognitionProcessor> { this.activeProcessor };
        }

        public void CloseFacialRecognitionEngine()
        {
            this.facialRecognitionEngine.RecognitionComplete -= this.faceRecognizedEvent;
        }
    }
}
