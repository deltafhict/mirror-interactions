// BitmapSourceTargetFace.cs
// compile with: /doc:BitmapSourceTargetFace.xml
using Microsoft.Kinect.Face;
using MirrorInteractions.Face;
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MirrorInteractions
{
    /// <summary>
    /// Target face with a BitmapSource accessor for the face. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class BitmapSourceTargetFace : IEigenObjectTargetFace, IFaceModelTargetFace
    {
        /// <summary>
        /// Gets or sets the key returned when this face is found. </summary>
        /// <value>
        /// String containing a Key.</value>
        [JsonProperty]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the grayscale, 100x100 target image. </summary>
        /// <value>
        /// Bitmap containing a Image.</value>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Gets or sets the detected hair color of the face. </summary>
        /// <value>
        /// Color containing a HairColor. </value>
        [JsonProperty]
        public Color HairColor { get; set; }

        /// <summary>
        /// Gets or sets the detected skin color of the face. </summary>
        /// <value>
        /// Color containing a SkinColor. </value>
        [JsonProperty]
        public Color SkinColor { get; set; }

        /// <summary>
        /// Gets or sets the detected deformations of the face. </summary>
        /// <value>
        /// IReadOnlyDictionary<FaceShapeDeformations, float> containing Deformations. </value>
        [JsonProperty]
        public IReadOnlyDictionary<FaceShapeDeformations, float> Deformations { get; set; }
    }
}
