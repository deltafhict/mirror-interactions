using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorInteractions
{
    public class WSMessage
    {
        public String app { get; set; }
        public String type { get; set; }
        public String action { get; set; }
        public String person { get; set; }
        public Appointment appointment { get; set; }
    }

    public class Appointment
    {
        public String title { get; set; }
        public String desc { get; set; }
        public String datetime { get; set; }
    }
}
