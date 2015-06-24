// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 06-10-2015
// ***********************************************************************
// <copyright file="FaceLoader.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used to load all previous learned faces.</summary>
// ***********************************************************************
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

/// <summary>
/// The Face namespace, all face related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class used to load all previous learned faces.
    /// </summary>
    public class FaceLoader
    {
        /// <summary>
        /// Store for the faces property.
        /// </summary>
        private List<BitmapSourceTargetFace> faces = new List<BitmapSourceTargetFace>();

        /// <summary>
        /// Store for the activeProcessor property
        /// </summary>
        private IRecognitionProcessor activeProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceLoader" /> class.
        /// </summary>
        public FaceLoader()
        {
            activeProcessor = EigenObjectRecognitionProcessor.Instance;
            LoadAllTargetFaces();
        }

        /// <summary>
        /// Loads all BSTFs from the current directory
        /// </summary>
        public void LoadAllTargetFaces()
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
            UpdateTargetFaces();
        }

        /// <summary>
        /// Updates the target faces.
        /// </summary>
        public void UpdateTargetFaces()
        {
            if (this.faces.Count > 1)
                this.activeProcessor.SetTargetFaces(faces);
        }
    }
}
