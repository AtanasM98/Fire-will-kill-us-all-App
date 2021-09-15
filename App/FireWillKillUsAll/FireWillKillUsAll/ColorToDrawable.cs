using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    class ColorToDrawable
    {

        Color color;
        Bitmap bitmap;
        public Color Color { get => color; }
        public Bitmap Bitmap { get => bitmap; }

        public ColorToDrawable(Color color, Bitmap bitmap)
        {
            this.color = color;
            this.bitmap = bitmap;
        }

       
    }
}
