// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : delta
// Created          : 05-27-2015
//
// Last Modified By : delta
// Last Modified On : 05-27-2015
// ***********************************************************************
// <copyright file="GestureDatabase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Kinect.VisualGestureBuilder;
using System.Linq;

namespace MirrorInteractions.Gestures
{
    /// <summary>
    /// Class GestureDatabase.
    /// </summary>
    public class GestureDatabase
    {
        /// <summary>
        /// The drag to left gesture
        /// </summary>
        public Gesture dragToLeftGesture;
        /// <summary>
        /// The drag to right gesture
        /// </summary>
        public Gesture dragToRightGesture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureDatabase"/> class.
        /// </summary>
        public GestureDatabase()
        {
            this.LoadGestures();
        }

        /// <summary>
        /// Loads the gestures.
        /// </summary>
        private void LoadGestures() {
            // Load GestureDataBase, assuming that the file exists
            VisualGestureBuilderDatabase db = new VisualGestureBuilderDatabase(
              @"Gestures/MirrorGestures.gbd");

            // Initialize Drag Gestures
            dragToLeftGesture = db.AvailableGestures.Where(g => g.Name == "SwipeToLeftProgress").Single();
            dragToRightGesture = db.AvailableGestures.Where(g => g.Name == "SwipeToRightProgress").Single();
        }
    }
}