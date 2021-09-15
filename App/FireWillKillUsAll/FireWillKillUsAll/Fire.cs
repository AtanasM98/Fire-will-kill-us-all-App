using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Fire
    {
        public delegate void ExtinguishHandler(Tile t);
        public event ExtinguishHandler Extinguished;
        public event ExtinguisherUsed SomeoneUsedExtinguisher;
        private int intensity;
        private Tile tile;
        public bool isExtinguished;

        public int Intensity
        {
            get { return intensity; }
        }
        public Tile Tile
        {
            get { return tile; }
        }

        public Fire(Tile t)
        {
            intensity = 25;
            tile = t;
            t.SetFire(this);
            tile.onFire = true;
            isExtinguished = false;
        }

        public void Extinguish(int extValue)
        {
            intensity -= extValue;
            if(intensity <= 0)
            {
                tile.isExtinguished = true;
                tile.onFire = false;
                Extinguished?.Invoke(tile);
            }
        }

        public void ExtinguishWithExtinguisher(int extValue)
        {
            intensity -= extValue;
            if (intensity <= 0)
            {
                tile.isExtinguished = true;
                tile.onFire = false;
                Extinguished?.Invoke(tile);

                //Invoke extinguisher used event for statistics
                if (SomeoneUsedExtinguisher != null)
                {
                    SomeoneUsedExtinguisher.Invoke(tile);
                }
            }
        }

        public Tile SpreadFire(int dir)
        {
            //Mathematical stuff, for now only random dir
            switch (dir)
            {
                case 0:
                    if (tile.up.colorInBitmap == Color.FromArgb(255, 255, 255, 255) || tile.up.colorInBitmap.A == 254)
                    {
                        return tile.up;
                    }
                    break;
                case 1:
                    if (tile.right.colorInBitmap == Color.FromArgb(255, 255, 255, 255) || tile.right.colorInBitmap.A == 254)
                    {
                        return tile.right;
                    }
                    break;
                case 2:
                    if (tile.down.colorInBitmap == Color.FromArgb(255, 255, 255, 255) || tile.down.colorInBitmap.A == 254)
                    {
                        return tile.down;
                    }
                    break;
                case 3:
                    if (tile.left.colorInBitmap == Color.FromArgb(255, 255, 255, 255) || tile.left.colorInBitmap.A == 254)
                    {
                        return tile.left;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}