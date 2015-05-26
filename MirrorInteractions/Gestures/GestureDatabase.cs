using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorGesture
{
    public class GestureDatabase
    {
        public Gesture dragToLeftGesture;
        public Gesture dragToRightGesture;

        public GestureDatabase()
        {
            this.LoadGestures();
        }

        private void LoadGestures() {
            // Load GestureDataBase, assuming that the file exists
            VisualGestureBuilderDatabase db = new VisualGestureBuilderDatabase(
              @"Gestures/MirrorGestures.gbd");

            // Initialize Drag Gestures
            dragToLeftGesture = db.AvailableGestures.Where(g => g.Name == "DragToLeftProgress").Single();
            dragToRightGesture = db.AvailableGestures.Where(g => g.Name == "DragToRightProgress").Single();
        }
    }
}