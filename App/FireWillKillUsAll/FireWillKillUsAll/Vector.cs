using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Vector
    {
        private int x;
        private int y;
        public int X { get => x;}
        public int Y { get => y; }

        public Vector(int x,int y)
        {
            this.x = x;
            this.y = y;

        }
    }
}
