﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Face;

namespace Sacknet.KinectFacialRecognition.KinectFaceModel
{
    /// <summary>
    /// A target face for face model recognition
    /// </summary>
    public interface IFaceModelTargetFace : ITargetFace
    {
        /// <summary>
        /// Gets or sets the detected hair color of the face
        /// </summary>
        Color HairColor { get; set; }

        /// <summary>
        /// Gets or sets the detected skin color of the face
        /// </summary>
        Color SkinColor { get; set; }

        /// <summary>
        /// Gets or sets the detected face deformations
        /// </summary>
        IReadOnlyDictionary<FaceShapeDeformations, float> Deformations { get; set; }
    }
}
