// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : daan
// Created          : 06-02-2015
//
// Last Modified By : daan
// Last Modified On : 06-02-2015
// ***********************************************************************
// <copyright file="BitmapSourceTargetFace.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used as model for the BitmapSourceTargetFace</summary>
// ***********************************************************************
using Microsoft.Kinect.Face;
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// The Face namespace, all face related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Face
{
    /// <summary>
    /// Class used as model for the BitmapSourceTargetFace
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class BitmapSourceTargetFace : IEigenObjectTargetFace, IFaceModelTargetFace
    {
        /// <summary>
        /// Gets or sets the key returned when this face is found.
        /// </summary>
        /// <value>String containing a Key.</value>
        [JsonProperty]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the grayscale, 100x100 target image.
        /// </summary>
        /// <value>Bitmap containing a Image.</value>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Gets or sets the detected hair color of the face.
        /// </summary>
        /// <value>Color containing a HairColor.</value>
        [JsonProperty]
        public Color HairColor { get; set; }

        /// <summary>
        /// Gets or sets the detected skin color of the face.
        /// </summary>
        /// <value>Color containing a SkinColor.</value>
        [JsonProperty]
        public Color SkinColor { get; set; }

        /// <summary>
        /// Gets or sets the deformations.
        /// </summary>
        /// <value>The deformations.</value>
        [JsonProperty]
        public IReadOnlyDictionary<FaceShapeDeformations, float> Deformations { get; set; }
    }
}
