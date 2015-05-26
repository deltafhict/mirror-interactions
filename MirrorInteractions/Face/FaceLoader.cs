using MirrorInteractions.Models;
using Newtonsoft.Json;
using Sacknet.KinectFacialRecognition;
using Sacknet.KinectFacialRecognition.KinectFaceModel;
using Sacknet.KinectFacialRecognition.ManagedEigenObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions.Face
{
    public class FaceLoader
    {
        /// <summary>
        /// Store for the faces property. </summary>
        private List<BitmapSourceTargetFace> faces = new List<BitmapSourceTargetFace>();

        /// <summary>
        /// Store for the activeProcessor property</summary>
        private IRecognitionProcessor activeProcessor;

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

        public void UpdateTargetFaces()
        {
            if (this.faces.Count > 1)
                this.activeProcessor.SetTargetFaces(faces);
        }
    }
}
