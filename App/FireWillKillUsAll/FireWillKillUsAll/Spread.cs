using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    static class Spread
    {
        //spread by color algorithm ON grid
        public static List<Tile> SpreadOnColor(Tile startTile)
        {
            List<Tile> s = new List<Tile>();

            startTile.checkedInAlgorithm = true;
            s.Add(startTile);
            // MessageBox.Show($"Tile x y:");
            if (startTile.right != null)
            {
                // MessageBox.Show($"First if");
                if (startTile.colorInBitmap == startTile.right.colorInBitmap)
                {
                    // MessageBox.Show($"Secound if");
                    if (!startTile.right.checkedInAlgorithm)
                    {
                        //  MessageBox.Show($"Third if");
                        foreach (Tile t in SpreadOnColor(startTile.right))
                        {
                            s.Add(t);
                            // MessageBox.Show($"Tile x y:{t.StartPos.X},{t.StartPos.Y}");
                        }
                    }
                }

            }
            if (startTile.down != null)
            {
                // MessageBox.Show($"First if");
                if (startTile.colorInBitmap == startTile.down.colorInBitmap)
                {
                    //   MessageBox.Show($"Secound if");
                    if (!startTile.down.checkedInAlgorithm)
                    {
                        //  MessageBox.Show($"Third if");
                        foreach (Tile t in SpreadOnColor(startTile.down))
                        {
                            s.Add(t);
                            // MessageBox.Show($"Tile x y:{t.StartPos.X},{t.StartPos.Y}");
                        }
                    }
                }

            }
            if (startTile.left != null)
            {
                //  MessageBox.Show($"First if");
                if (startTile.colorInBitmap == startTile.left.colorInBitmap)
                {
                    //   MessageBox.Show($"Secound if");
                    if (!startTile.left.checkedInAlgorithm)
                    {
                        //   MessageBox.Show($"Third if");
                        foreach (Tile t in SpreadOnColor(startTile.left))
                        {
                            s.Add(t);
                            //MessageBox.Show($"Tile x y:{t.StartPos.X},{t.StartPos.Y}");
                        }
                    }
                }

            }
            if (startTile.up != null)
            {
                // MessageBox.Show($"First if");
                if (startTile.colorInBitmap == startTile.up.colorInBitmap)
                {
                    //  MessageBox.Show($"Secound if");
                    if (!startTile.up.checkedInAlgorithm)
                    {
                        // MessageBox.Show($"Third if");
                        foreach (Tile t in SpreadOnColor(startTile.up))
                        {
                            s.Add(t);
                            // MessageBox.Show($"Tile x y:{t.StartPos.X},{t.StartPos.Y}");
                        }
                    }
                }

            }
            return s;
        }

    }
}
