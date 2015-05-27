// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="FaceLearner.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class FaceLearner.
    /// </summary>
    public class FaceLearner
    {
        /// <summary>
        /// Store for the faces property.
        /// </summary>
        private List<BitmapSourceTargetFace> faces = new List<BitmapSourceTargetFace>();

        /// <summary>
        /// Learns new faces from the recognized.
        /// </summary>
        /// <param name="recognitionResult">The recognition result.</param>
        /// <param name="personName">Name of the person.</param>
        public void LearnNewFaces(RecognitionResult recognitionResult, string personName)
        {
            TrackedFace face = null;

            if (recognitionResult.Faces != null)
            {
                face = recognitionResult.Faces.FirstOrDefault();
            }

            var eoResult = (EigenObjectRecognitionProcessorResult)face.ProcessorResults.SingleOrDefault(x => x is EigenObjectRecognitionProcessorResult);
            var fmResult = (FaceModelRecognitionProcessorResult)face.ProcessorResults.SingleOrDefault(x => x is FaceModelRecognitionProcessorResult);

            var bstf = new BitmapSourceTargetFace();
            bstf.Key = personName;

            if (eoResult != null)
            {
                bstf.Image = (Bitmap)eoResult.Image.Clone();
            }
            else
            {
                bstf.Image = face.TrackingResult.GetCroppedFace(recognitionResult.ColorSpaceBitmap);
            }

            if (fmResult != null)
            {
                bstf.Deformations = fmResult.Deformations;
                bstf.HairColor = fmResult.HairColor;
                bstf.SkinColor = fmResult.SkinColor;
            }

            this.faces.Add(bstf);

            this.SerializeBitmapSourceTargetFace(bstf);
        }

        /// <summary>
        /// Saves the target face to disk
        /// </summary>
        /// <param name="bstf">The BSTF.</param>
        private void SerializeBitmapSourceTargetFace(BitmapSourceTargetFace bstf)
        {
            var filenamePrefix = "TF_" + DateTime.Now.Ticks.ToString();
            var suffix = ".pca";
            System.IO.File.WriteAllText(filenamePrefix + suffix, JsonConvert.SerializeObject(bstf));
            bstf.Image.Save(filenamePrefix + ".png");
        }
    }
}
