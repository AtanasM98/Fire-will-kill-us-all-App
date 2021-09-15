using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireWillKillUsAll
{
    public static class Dump
    {
        public static int startX = 25;
        public static int startY = 21;
        public static string Info { get; set; } = "No info yet";
        public static string[] bitmapArray { get; set; } = new string[startX * startY];
       

        public static void SendBitMap(Bitmap bitMap, int x, int y)
        {
            ChangeBitmap(bitMap);
            string[] a = bitmapArray;

            //callbackChannel.SetMap(a, x, y);

        }

        public static void ChangeBitmap(Bitmap b)
        {
            //Test 
            /*SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {



                
                b.Save(dialog.FileName + ".png", ImageFormat.Png);

            }*/

           // Bitmap test1 = new Bitmap(25 * 21, 1);
            //

            int currX = 0;
            int currY = 0;

            int indInArr = 0;
            for (int y = 0; y < startY; y++)
            {
                for (int x = 0; x < startX; x++)
                {
                    string s = ColorTranslator.ToHtml(b.GetPixel(x, y));
                    bitmapArray[indInArr] = s;
                   
                    indInArr++;
                    currX++;
                }
                currX = 0;
                currY++;
            }

            
            //test - WORKSSSS
            /*
            for (int i = 0; i < bitmapArray.Length; i++)
            {
                Color col = Color.FromArgb(bitmapArray[i].Alpha, bitmapArray[i].Red, bitmapArray[i].Green, bitmapArray[i].Blue);
                test1.SetPixel(i, 0, col);

            }

            SaveFileDialog dialog1 = new SaveFileDialog();
            if (dialog1.ShowDialog() == DialogResult.OK)
            {




                test1.Save(dialog1.FileName + ".png", ImageFormat.Png);

            }*/
            /* public static void ChangeBitmap(Bitmap b)
             {
                 int currX = 0;
                 int currY = 0;
                 for (int y = 0; y < startY; y++)
                  {
                     for (int x = 0; x < startX; x++)
                     {
                        bitmapArray[x,y] = b.GetPixel(x, y);
                         currX++;
                     }
                     currX = 0;
                     currY++;
                 }

             */
        }
    }
}
