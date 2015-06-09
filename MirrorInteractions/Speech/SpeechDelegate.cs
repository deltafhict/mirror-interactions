using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions.Speech
{
    public abstract class SpeechDelegate
    {
        public delegate void SpeechCalibratedDelegate(double threshold);
    }
}
