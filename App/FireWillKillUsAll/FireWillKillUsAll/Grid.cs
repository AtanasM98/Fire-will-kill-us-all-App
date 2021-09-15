using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FireWillKillUsAll
{
    public static class Grid 
    {
        private static List<ColorToDrawable> drs = new List<ColorToDrawable>() {
            new ColorToDrawable(Color.FromArgb(255, 0, 0, 0), Properties.Resources.wallHor),
            new ColorToDrawable(Color.FromArgb(255, 50, 50, 50), Properties.Resources.wallVer),
            new ColorToDrawable(Color.FromArgb(255, 250, 150, 51), Properties.Resources._4Wall),
            //FireEsc
            new ColorToDrawable(Color.FromArgb(255, 0,255, 0), Properties.Resources.FireEscS),
            new ColorToDrawable(Color.FromArgb(255, 150,255, 50), Properties.Resources.FireEscE),
            new ColorToDrawable(Color.FromArgb(255, 155,55, 255), Properties.Resources.FireEscW),
            new ColorToDrawable(Color.FromArgb(255, 80,255, 100), Properties.Resources.FireEscN),


            new ColorToDrawable(Color.FromArgb(255, 150, 0, 150), Properties.Resources.wallExtinguisherN),
            new ColorToDrawable(Color.FromArgb(255, 50, 200, 50), Properties.Resources.wallExtinguisherW),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 0), Properties.Resources.wallExtinguisherE),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 50), Properties.Resources.wallExtinguisherS),
            //doors
            new ColorToDrawable(Color.FromArgb(255, 0, 255, 255), Properties.Resources.DoorE),
            new ColorToDrawable(Color.FromArgb(255, 1, 255, 255), Properties.Resources.DoorE___Closed),
            new ColorToDrawable(Color.FromArgb(255, 0, 0, 255), Properties.Resources.DoorN),
            new ColorToDrawable(Color.FromArgb(255, 1, 0, 255), Properties.Resources.DoorN___Closed),
            new ColorToDrawable(Color.FromArgb(255, 255, 100, 255), Properties.Resources.DoorW),
            new ColorToDrawable(Color.FromArgb(255, 255, 101, 255), Properties.Resources.DoorW___Closed),
            new ColorToDrawable(Color.FromArgb(255, 100, 0, 255), Properties.Resources.DoorS),
            new ColorToDrawable(Color.FromArgb(255, 100, 1, 255), Properties.Resources.DoorS___Closed),

            //corners
            new ColorToDrawable(Color.FromArgb(255, 200, 200, 200), Properties.Resources.CornerBotLeft),
            new ColorToDrawable(Color.FromArgb(255, 125, 0, 255), Properties.Resources.CornerBotRight),
            new ColorToDrawable(Color.FromArgb(255, 100, 100, 100), Properties.Resources.CornerTopLeft),
            new ColorToDrawable(Color.FromArgb(255, 150, 150, 150), Properties.Resources.CornerTopRight),
            //3Walls
            new ColorToDrawable(Color.FromArgb(255, 255, 255, 0), Properties.Resources._3WallE),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 100), Properties.Resources._3WallN),
            new ColorToDrawable(Color.FromArgb(255, 255, 125, 0), Properties.Resources._3WallS),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 255), Properties.Resources._3WallW),
            new ColorToDrawable(Color.FromArgb(255, 123, 123, 123), Properties.Resources.Outside),

            //Fires
            new ColorToDrawable(Color.FromArgb(255, 255, 150, 0), Properties.Resources.FireTile),
            new ColorToDrawable(Color.FromArgb(255, 255, 170, 0), Properties.Resources.EmptyTile),
            new ColorToDrawable(Color.FromArgb(255, 255, 254, 0), Properties.Resources.CharredTile),

            //People
            new ColorToDrawable(Color.FromArgb(255, 2, 3, 4), Properties.Resources.shaggy),
            new ColorToDrawable(Color.FromArgb(255, 2, 2, 2), Properties.Resources.simpson),
            new ColorToDrawable(Color.FromArgb(255, 3, 3, 3), Properties.Resources.johnny),
            new ColorToDrawable(Color.FromArgb(255, 4, 4, 4), Properties.Resources.courage),

            //Air
            new ColorToDrawable(Color.FromArgb(155, 155, 155, 155), Properties.Resources.airHor),

            //vent
            new ColorToDrawable(Color.FromArgb(230, 230, 230, 230), Properties.Resources.Vent),

            //Windows
            new ColorToDrawable(Color.FromArgb(160, 255, 150, 0), Properties.Resources.wIndowHor),
            new ColorToDrawable(Color.FromArgb(160, 255, 170, 0), Properties.Resources.windowVer),
        };
        public static int personIdCounter = 1;
        internal static List<ColorToDrawable> Drs { get => drs; set => drs = value; }

        public static Person List_Of_People
        {
            get => default;
            set
            {
            }
        }

        internal static Floor Floor
        {
            get => default;
            set
            {
            }
        }

        public static Statistics Statistics_Form
        {
            get => default;
            set
            {
            }
        }

        public static UnityService1 UnityService1
        {
            get => default;
            set
            {
            }
        }

        static public Color GetColorOfActiveBrushFromBitmap(Bitmap a)
        {
            foreach (ColorToDrawable c in Drs)
            {
                if (CompareBitmapsFast(a, c.Bitmap))
                {
                    return c.Color;
                }
            }
            return Color.White;
        }

        public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] != b2bytes[n])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }

        static public Bitmap GetBitmapOfActiveBrushFromColor(Color a)
        {
            foreach (ColorToDrawable c in Grid.Drs)
            {
                if (c.Color == a)
                {
                    return c.Bitmap;
                }

            }
            return new Bitmap(1, 1);

        }
        static public List<Bitmap> GetBitmapsOfActiveBrushFromColor(Color a)
        {
            List<Bitmap> bmps = new List<Bitmap>();
            foreach (ColorToDrawable c in Grid.Drs)
            {
                if (c.Color == a)
                {
                    bmps.Add(c.Bitmap);
                }
            }
            return bmps;
        }

        public static void PopulateGrid()
        {
            Form1.grid = new Tile[Form1.gridSizeX, Form1.gridSizeY];

            int currPosX = Form1.startDrawPositionX;
            int currPosY = Form1.startDrawPositionY;
            // create grid
            for (int y = 0; y < Form1.gridSizeY; y++)
            {

                for (int x = 0; x < Form1.gridSizeX; x++)
                {
                    Form1.grid[x, y] = new Tile(new Vector(currPosX, currPosY), 40, null, null, null, null, Form1.nullColor, x, y);
                    currPosX += 40;
                }
                currPosY += 40;
                currPosX = Form1.startDrawPositionX;
            }
            Form1.endDrawPositionX = Form1.grid[24, 20].StartPos.X + Form1.grid[24, 20].Size;
            Form1.endDrawPositionY = Form1.grid[24, 20].StartPos.Y + Form1.grid[24, 20].Size;


            //create references from cell to cell

            for (int y = 0; y < Form1.gridSizeY; y++)
            {

                for (int x = 0; x < Form1.gridSizeX; x++)
                {
                    if (y == 0)
                    {
                        Form1.grid[x, y].up = null;
                        if (x == 0)
                        {
                            Form1.grid[x, y].left = null;

                            Form1.grid[x, y].down = Form1.grid[x, y + 1];

                            Form1.grid[x, y].right = Form1.grid[x + 1, y];
                        }
                        else
                        {
                            if (x == Form1.gridSizeX - 1)
                            {
                                Form1.grid[x, y].right = null;

                                Form1.grid[x, y].down = Form1.grid[x, y + 1];
                                Form1.grid[x, y].left = Form1.grid[x - 1, y];

                            }
                            else
                            {
                                 Form1.grid[x, y].left = Form1.grid[x - 1, y];
                                 Form1.grid[x, y].right = Form1.grid[x + 1, y];
                                Form1.grid[x, y].down = Form1.grid[x, y + 1];
                            }
                        }
                    }
                    else
                    {
                        if (y == Form1.gridSizeY - 1)
                        {
                            Form1.grid[x, y].down = null;
                            if (x == 0)
                            {
                                Form1.grid[x, y].left = null;
                                Form1.grid[x, y].up = Form1.grid[x, y - 1];


                                Form1.grid[x, y].right = Form1.grid[x + 1, y];

                            }
                            else
                            {
                                if (x == Form1.gridSizeX - 1)
                                {
                                    Form1.grid[x, y].right = null;
                                    Form1.grid[x, y].up = Form1.grid[x, y - 1];

                                    Form1.grid[x, y].left = Form1.grid[x - 1, y];

                                }
                                else
                                {
                                     Form1.grid[x, y].left = Form1.grid[x - 1, y];
                                     Form1.grid[x, y].right = Form1.grid[x + 1, y];
                                    Form1.grid[x, y].up = Form1.grid[x, y - 1];
                                }
                            }
                        }
                        else
                        {
                            if (x == 0)
                            {
                                Form1.grid[x, y].left = null;

                                 Form1.grid[x, y].up = Form1.grid[x, y - 1];
                                Form1.grid[x, y].down = Form1.grid[x, y + 1];

                                Form1.grid[x, y].right = Form1.grid[x + 1, y];

                            }
                            else
                            {
                                if (x == Form1.gridSizeX - 1)
                                {
                                    Form1.grid[x, y].right = null;

                                     Form1.grid[x, y].up = Form1.grid[x, y - 1];
                                     Form1.grid[x, y].down =  Form1.grid[x, y + 1];
                                    Form1.grid[x, y].left = Form1.grid[x - 1, y];

                                }
                                else
                                {
                                     Form1.grid[x, y].left = Form1.grid[x - 1, y];
                                     Form1.grid[x, y].right = Form1.grid[x + 1, y];
                                     Form1.grid[x, y].up = Form1.grid[x, y - 1];
                                    Form1.grid[x, y].down = Form1.grid[x, y + 1];
                                }
                            }

                        }

                    }

                }

            }


        }

        public static bool SearchGrid(Vector v, out Tile a)
        {

            if (v.X < Form1.startDrawPositionX || v.Y < Form1.startDrawPositionY || v.X > Form1.endDrawPositionX || v.Y > Form1.endDrawPositionY)
            {
                a = new Tile(new Vector(0, 0), 0, null, null, null, null, Form1.nullColor, -1, -1);
                MessageBox.Show("Coordinates out of bound");
                return false;
            }
            else
            {
                for (int i = 0; i < Form1.gridSizeX; i++)
                {
                    if (v.X == Form1.grid[i, 1].StartPos.X)
                    {
                        for (int b = 0; b < Form1.gridSizeY; b++)
                        {

                            if (v.Y == Form1.grid[i, b].StartPos.Y)
                            {

                                a = Form1.grid[i, b];

                                return true;
                            }
                            else if (v.Y > Form1.grid[i, b].StartPos.Y)
                            {
                                if (v.Y < Form1.grid[i, b].StartPos.Y + Form1.grid[i, b].Size)
                                {
                                    a = Form1.grid[i, b];

                                    return true;
                                }

                            }
                        }

                    }
                    else if (v.X > Form1.grid[i, 1].StartPos.X)
                    {
                        if (v.X < Form1.grid[i, 1].StartPos.X + Form1.grid[i, 1].Size)
                        {

                            for (int c = 0; c < Form1.gridSizeY; c++)
                            {
                                if (v.Y == Form1.grid[i, c].StartPos.Y)
                                {
                                    a = Form1.grid[i, c];

                                    return true;
                                }
                                else if (v.Y > Form1.grid[i, c].StartPos.Y)
                                {
                                    if (v.Y < Form1.grid[i, c].StartPos.Y + Form1.grid[i, c].Size)
                                    {
                                        a = Form1.grid[i, c];

                                        return true;
                                    }

                                }
                            }

                        }

                    }

                }

                a = new Tile(new Vector(0, 0), 0, null, null, null, null, Form1.nullColor, -1, -1);
                MessageBox.Show("Coordinates not found");
                return false;
            }
        }

        public static Tile[,] CreateGridNeighbourReferences(Tile[,] grid, int gridSizeY, int gridSizeX)
        {
            for (int y = 0; y < gridSizeY; y++)
            {

                for (int x = 0; x < gridSizeX; x++)
                {
                    if (y == 0)
                    {
                        grid[x, y].up = null;
                        if (x == 0)
                        {
                            grid[x, y].left = null;

                            grid[x, y].down = grid[x, y + 1];

                            grid[x, y].right = grid[x + 1, y];
                        }
                        else
                        {
                            if (x == gridSizeX - 1)
                            {
                                grid[x, y].right = null;

                                grid[x, y].down = grid[x, y + 1];
                                grid[x, y].left = grid[x - 1, y];

                            }
                            else
                            {
                                grid[x, y].left = grid[x - 1, y];
                                grid[x, y].right = grid[x + 1, y];
                                grid[x, y].down = grid[x, y + 1];
                            }
                        }
                    }
                    else
                    {
                        if (y == gridSizeY - 1)
                        {
                            grid[x, y].down = null;
                            if (x == 0)
                            {
                                grid[x, y].left = null;
                                grid[x, y].up = grid[x, y - 1];


                                grid[x, y].right = grid[x + 1, y];

                            }
                            else
                            {
                                if (x == gridSizeX - 1)
                                {
                                    grid[x, y].right = null;
                                    grid[x, y].up = grid[x, y - 1];

                                    grid[x, y].left = grid[x - 1, y];

                                }
                                else
                                {
                                    grid[x, y].left = grid[x - 1, y];
                                    grid[x, y].right = grid[x + 1, y];
                                    grid[x, y].up = grid[x, y - 1];
                                }
                            }
                        }
                        else
                        {
                            if (x == 0)
                            {
                                grid[x, y].left = null;

                                grid[x, y].up = grid[x, y - 1];
                                grid[x, y].down = grid[x, y + 1];

                                grid[x, y].right = grid[x + 1, y];

                            }
                            else
                            {
                                if (x == gridSizeX - 1)
                                {
                                    grid[x, y].right = null;

                                    grid[x, y].up = grid[x, y - 1];
                                    grid[x, y].down = grid[x, y + 1];
                                    grid[x, y].left = grid[x - 1, y];

                                }
                                else
                                {
                                    grid[x, y].left = grid[x - 1, y];
                                    grid[x, y].right = grid[x + 1, y];
                                    grid[x, y].up = grid[x, y - 1];
                                    grid[x, y].down = grid[x, y + 1];
                                }
                            }

                        }

                    }

                }

            }
            return grid;
        }

        public static bool SearchGrid(Vector v, out Tile a, out int x, out int y)
        {

            if (v.X < Form1.startDrawPositionX || v.Y < Form1.startDrawPositionY || v.X > Form1.endDrawPositionX || v.Y > Form1.endDrawPositionY)
            {
                a = new Tile(new Vector(0, 0), 0, null, null, null, null, Form1.nullColor, -1, -1);
                MessageBox.Show("Coordinates out of bound");
                x = -1;
                y = -1;
                return false;
            }
            else
            {
                for (int i = 0; i < Form1.gridSizeX; i++)
                {
                    if (v.X == Form1.grid[i, 1].StartPos.X)
                    {
                        for (int b = 0; b < Form1.gridSizeY; b++)
                        {

                            if (v.Y == Form1.grid[i, b].StartPos.Y)
                            {

                                a = Form1.grid[i, b];
                                x = i;
                                y = b;
                                return true;
                            }
                            else if (v.Y > Form1.grid[i, b].StartPos.Y)
                            {
                                if (v.Y < Form1.grid[i, b].StartPos.Y + Form1.grid[i, b].Size)
                                {
                                    a = Form1.grid[i, b];
                                    x = i;
                                    y = b;
                                    return true;
                                }

                            }
                        }

                    }
                    else if (v.X > Form1.grid[i, 1].StartPos.X)
                    {
                        if (v.X < Form1.grid[i, 1].StartPos.X + Form1.grid[i, 1].Size)
                        {

                            for (int c = 0; c < Form1.gridSizeY; c++)
                            {
                                if (v.Y == Form1.grid[i, c].StartPos.Y)
                                {
                                    a = Form1.grid[i, c];
                                    x = i;
                                    y = c;
                                    return true;
                                }
                                else if (v.Y > Form1.grid[i, c].StartPos.Y)
                                {
                                    if (v.Y < Form1.grid[i, c].StartPos.Y + Form1.grid[i, c].Size)
                                    {
                                        a = Form1.grid[i, c];
                                        x = i;
                                        y = c;
                                        return true;
                                    }

                                }
                            }

                        }

                    }

                }

                a = new Tile(new Vector(0, 0), 0, null, null, null, null, Form1.nullColor, -1, -1);
                MessageBox.Show("Coordinates not found");
                x = -1;
                y = -1;
                return false;
            }
        }

        public static List<Tile> GetConnectedList(Tile startTile)
        {
            foreach (Tile t in Form1.grid)
            {
                t.checkedInAlgorithm = false;
            }
            return Spread.SpreadOnColor(startTile);
        }

        // spread by color algorithm ON bitmap
        public static  Bitmap MarkOutsideBitmap(Bitmap b, int x, int y)
        {
            b.SetPixel(x, y, Form1.outsideColor);


            if (x + 1 < b.Width)
            {
                //MessageBox.Show($"{b.Width},{b.Height}");
                if (b.GetPixel(x + 1, y) == Color.FromArgb(255, 255, 255, 255))
                {
                    MarkOutsideBitmap(b, x + 1, y);
                }

            }
            if (y + 1 < b.Height)
            {

                if (b.GetPixel(x, y + 1) == Color.FromArgb(255, 255, 255, 255))
                {
                    MarkOutsideBitmap(b, x, y + 1);
                }

            }
            if (x > 0)
            {

                if (b.GetPixel(x - 1, y) == Color.FromArgb(255, 255, 255, 255))
                {
                    MarkOutsideBitmap(b, x - 1, y);
                }

            }
            if (y > 0)
            {

                if (b.GetPixel(x, y - 1) == Color.FromArgb(255, 255, 255, 255))
                {
                    MarkOutsideBitmap(b, x, y - 1);
                }

            }
            return b;
        }
        
        public static Bitmap AddOutlineToBitmap(Bitmap b)
        {
            //MessageBox.Show($" 2 {b.Width},{b.Height}");
           
            Bitmap newB = new Bitmap(27, 23);

            // MessageBox.Show($"3 {newB.Width},{newB.Height}");

            for (int y = 0; y < 23; y++)
            {

                for (int x = 0; x < 27; x++)
                {
                    newB.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                    //newB.SetPixel(x, y, Color.FromArgb(255, 123, 123, 123));
                    // MessageBox.Show($"x else {x},{y}");
                }

            }


            for (int y1 = 0; y1 < 21; y1++)
            {
                for (int x1 = 0; x1 < 25; x1++)
                {

                    Color c = b.GetPixel(x1, y1);

                    newB.SetPixel(x1 + 1, y1 + 1, c);
                    //MessageBox.Show($"x1 else {x1 + 1},{y1 + 1}");
                }
            }
            return newB;
        }
        public static byte[] encodeColor(Tile t, Color futureColor)
        {
            byte[] bytes = new byte[3];
            byte r = 0b0000_0000;
            byte g = 0b0000_0000;
            byte b = 0b0000_0000;

            #region Red
            //Door East open
            if (t.colorInBitmap == Color.FromArgb(255, 0, 255, 255))
            {
                r = 0b0000_0001;
            }
            //Exit East open
            if (t.colorInBitmap == Color.FromArgb(255, 150, 255, 50))
            {
                r = 0b0000_0011;
            }
            //Door East closed
            if (t.colorInBitmap == Color.FromArgb(255, 1, 255, 255))
            {
                r = 0b0000_0010;
            }
            //Door North open
            if (t.colorInBitmap == Color.FromArgb(255, 0, 0, 255))
            {
                r = 0b0000_0100;
            }
            //Exit North open
            if (t.colorInBitmap == Color.FromArgb(255, 80, 255, 100))
            {
                r = 0b0000_1001;
            }
            //Door North closed
            if (t.colorInBitmap == Color.FromArgb(255, 1, 0, 255))
            {
                r = 0b0000_1000;
            }
            //Door West open
            if (t.colorInBitmap == Color.FromArgb(255, 255, 100, 255))
            {
                r = 0b0001_0000;
            }
            //Exit West open
            if (t.colorInBitmap == Color.FromArgb(255, 155, 55, 255))
            {
                r = 0b0001_0001;
            }
            //Door West closed
            if (t.colorInBitmap == Color.FromArgb(255, 255, 101, 255))
            {
                r = 0b0010_0000;
            }
            //Door South open
            if (t.colorInBitmap == Color.FromArgb(255, 100, 0, 255))
            {
                r = 0b0100_0000;
            }
            //Exit South open
            if (t.colorInBitmap == Color.FromArgb(255, 0, 255, 0))
            {
                r = 0b0100_0001;
            }
            //Door South closed
            if (t.colorInBitmap == Color.FromArgb(255, 100, 1, 255))
            {
                r = 0b1000_0000;
            }
            #endregion

            #region Green
            //Charred tile
            if (futureColor == Color.FromArgb(255, 255, 254, 0))
            {
                g = 0b0000_0001;
            }
            //Fire
            if (futureColor == Color.FromArgb(255, 255, 150, 0))
            {
                g = 0b0000_0010;
            }
            //Vent
            if (futureColor == Color.FromArgb(230, 230, 230, 230))
            {
                g = 0b0000_0100;
            }
            //Air
            if (futureColor == Color.FromArgb(155, 155, 155, 155))
            {
                g = 0b0000_1000;
            }
            //Shaggy
            if (futureColor == Color.FromArgb(255, 2, 3, 4))
            {
                g = 0b0001_0000;
            }
            //Simpson
            if (futureColor == Color.FromArgb(255, 2, 2, 2))
            {
                g = 0b0010_0000;
            }
            //Johnny
            if (futureColor == Color.FromArgb(255, 3, 3, 3))
            {
                g = 0b0100_0000;
            }
            //Courage
            if (futureColor == Color.FromArgb(255, 4, 4, 4))
            {
                g = 0b1000_0000;
            }
            #endregion

            #region Blue
            if(t.colorInBitmap.A == 254 && t.colorInBitmap.G > 0)
            {
                //Charred tile
                if (futureColor == Color.FromArgb(255, 255, 254, 0))
                {
                    b = 0b0000_0001;
                }
                //Fire
                if (futureColor == Color.FromArgb(255, 255, 150, 0))
                {
                    b = 0b0000_0010;
                }
                //Vent
                if (futureColor == Color.FromArgb(230, 230, 230, 230))
                {
                    b = 0b0000_0100;
                }
                //Air
                if (futureColor == Color.FromArgb(155, 155, 155, 155))
                {
                    b = 0b0000_1000;
                }
                if(b == g)
                {
                    b = 0;
                }
            }
            #endregion

            bytes[0] = r;
            bytes[1] = g;
            bytes[2] = b;

            return bytes;
        }

        public static List<Bitmap> decodeColor(Color c)
        {
            List<Bitmap> colors = new List<Bitmap>();

            #region Red
            if (c.R != 0)
            {
                colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 170, 0)));
                //Door East open
                if (c.R == 0b0000_0001)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 0, 255, 255));
                }
                //Exit East open
                if (c.R == 0b0000_0011)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 150, 255, 50));
                }
                //Door East closed
                if (c.R == 0b0000_0010)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 1, 255, 255));
                }
                //Door North open
                if (c.R == 0b0000_1000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 0, 0, 255));
                }
                //Exit North open
                if (c.R == 0b0000_1001)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 80, 255, 100));
                }
                //Door North closed
                if (c.R == 0b0000_1000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 1, 0, 255));
                }
                //Door West open
                if (c.R == 0b0001_0000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 100, 255));
                }
                //Exit West open
                if (c.R == 0b0001_0001)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 155, 55, 255));
                }
                //Door West closed
                if (c.R == 0b0010_0000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 101, 255));
                }
                //Door South open
                if (c.R == 0b0100_0000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 100, 0, 255));
                }
                //Exit South open
                if (c.R == 0b0100_0001)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 0, 255, 0));
                }
                //Door South closed
                if (c.R == 0b1000_0000)
                {
                    colors[0] = GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 100, 1, 255));
                }
            }
            #endregion

            #region Green
            if (c.G != 0)
            {
                //Charred tile
                if (c.G == 0b0000_00001)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 254, 0)));
                }
                //Fire
                if (c.G == 0b0000_0010)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 150, 0)));
                }
                //Vent
                if (c.G == 0b0000_0100)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(230, 230, 230, 230)));
                }
                //Air
                if (c.G == 0b0000_1000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(155, 155, 155, 155)));
                }
                //Shaggy
                if (c.G == 0b0001_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 2, 3, 4)));
                }
                //Simpson
                if (c.G == 0b0010_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 2, 2, 2)));
                }
                //Johnny
                if (c.G == 0b0100_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 3, 3, 3)));
                }
                //Courage
                if (c.G == 0b1000_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 4, 4, 4)));
                }
            }
            #endregion

            #region Blue
            if (c.B != 0)
            {
                //Charred tile
                if (c.B == 0b0000_00001)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 254, 0)));
                }
                //Fire
                if (c.B == 0b0000_0010)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 255, 150, 0)));
                }
                //Vent
                if (c.B == 0b0000_0100)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(230, 230, 230, 230)));
                }
                //Air
                if (c.B == 0b0000_1000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(155, 155, 155, 155)));
                }
                //Shaggy
                if (c.B == 0b0001_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 2, 3, 4)));
                }
                //Simpson
                if (c.B == 0b0010_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 2, 2, 2)));
                }
                //Johnny
                if (c.B == 0b0100_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 3, 3, 3)));
                }
                //Courage
                if (c.B == 0b1000_0000)
                {
                    colors.Add(GetBitmapOfActiveBrushFromColor(Color.FromArgb(255, 4, 4, 4)));
                }
            }
            #endregion
            if (colors.Count == 0)
            {
                colors.Add(GetBitmapOfActiveBrushFromColor(c));
            }
            return colors;
        }
    }
}
