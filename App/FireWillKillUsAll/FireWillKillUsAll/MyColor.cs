using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    [DataContract]
    public class MyColor
    {
        public MyColor(int alpha, int red, int green, int blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }
        public MyColor(Color c)
        {
            Alpha = c.A;
            Red = c.R;
            Green = c.G;
            Blue = c.B;
        }
        [DataMember]
        public int Alpha { get; set; }

        [DataMember]
        public int Red { get; set; }
        [DataMember]
        public int Green { get; set; }
        [DataMember]
        public int Blue { get; set; }

    }
}
