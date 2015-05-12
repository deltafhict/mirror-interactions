using Microsoft.Kinect.Face;
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MirrorInteractions
{

    /// <summary>
    /// Target face with a BitmapSource accessor for the face
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class BitmapSourceTargetFace : IEigenObjectTargetFace, IFaceModelTargetFace
    {
        private BitmapSource bitmapSource;

        /// <summary>
        /// Gets the BitmapSource version of the face
        /// </summary>
        public BitmapSource BitmapSource
        {
            get
            {
                if (this.bitmapSource == null)
                    this.bitmapSource = MainWindow.LoadBitmap(this.Image);

                return this.bitmapSource;
            }
        }

        /// <summary>
        /// Gets or sets the key returned when this face is found
        /// </summary>
        [JsonProperty]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the grayscale, 100x100 target image
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Gets or sets the detected hair color of the face
        /// </summary>
        [JsonProperty]
        public Color HairColor { get; set; }

        /// <summary>
        /// Gets or sets the detected skin color of the face
        /// </summary>
        [JsonProperty]
        public Color SkinColor { get; set; }

        /// <summary>
        /// Gets or sets the detected deformations of the face
        /// </summary>
        [JsonProperty]
        public IReadOnlyDictionary<FaceShapeDeformations, float> Deformations { get; set; }
    }
}
