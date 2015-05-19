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
        /// Facial recognition engine using face detection from Kinect.
        /// </summary>
        private KinectFacialRecognitionEngine facialRecognitionEngine;

        private List<BitmapSourceTargetFace> faces = new List<BitmapSourceTargetFace>();

        private IRecognitionProcessor activeProcessor;

        EventHandler<RecognitionResult> faceRecognizedEvent;

        KinectSensor kinectSensor;

        public FaceRecognition(KinectSensor kinectSensor)
        {
            FaceRecognizedHandler faceRecognizedHandler = new FaceRecognizedHandler();
            this.faceRecognizedEvent = faceRecognizedHandler.FaceRecognition;
            this.kinectSensor = kinectSensor;
        }

        public void OpenFacialRecognitionEngine()
        {
            this.activeProcessor = new EigenObjectRecognitionProcessor();

            this.LoadAllTargetFaces();
            this.UpdateTargetFaces();

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

        /// <summary>
        /// Loads all BSTFs from the current directory
        /// </summary>
        private void LoadAllTargetFaces()
        {
            //this.viewModel.TargetFaces.Clear();
            var result = new List<BitmapSourceTargetFace>();
            var suffix = ".pca";

            foreach (var file in Directory.GetFiles(".", "TF_*" + suffix))
            {
                var bstf = JsonConvert.DeserializeObject<BitmapSourceTargetFace>(File.ReadAllText(file));
                bstf.Image = (Bitmap)Bitmap.FromFile(file.Replace(suffix, ".png"));
                faces.Add(bstf);
            }
        }

        private void UpdateTargetFaces()
        {
            if (this.faces.Count > 1)
                this.activeProcessor.SetTargetFaces(faces);
        }
    }
}
