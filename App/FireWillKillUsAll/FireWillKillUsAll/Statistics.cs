using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FireWillKillUsAll
{
    public partial class Statistics : Form
    {
        StatisticsManager sm;
        Graphics g = null;
        int gridSizeX = 25;
        int gridSizeY = 21;
        Color outsideColor = Color.FromArgb(255, 123, 123, 123);
        Tile[,] grid;
        int startDrawPositionX = 10;
        int startDrawPositionY = 10;
        int endDrawPositionX;
        int endDrawPositionY;
        Color nullColor = Color.Crimson;
        Bitmap bmp;
        Brush EraserBrush;
        Pen penBlack;
        List<Tile> currentPosForAllDead;
        List<Tile> currentPosForAllExtinghuisedFires;


        List<ColorToDrawable> drs = new List<ColorToDrawable>() {
            new ColorToDrawable(Color.FromArgb(255, 0, 0, 0), Properties.Resources.wallHor),
            new ColorToDrawable(Color.FromArgb(255, 50, 50, 50), Properties.Resources.wallVer),
            new ColorToDrawable(Color.FromArgb(255, 250, 150, 51), Properties.Resources._4Wall),
            //FireEsc
            new ColorToDrawable(Color.FromArgb(255, 0,255, 0), Properties.Resources.FireEscS),
            new ColorToDrawable(Color.FromArgb(255, 150,255, 50), Properties.Resources.FireEscE),
            new ColorToDrawable(Color.FromArgb(255, 155,55, 255), Properties.Resources.FireEscW),
            new ColorToDrawable(Color.FromArgb(255, 80,255, 100), Properties.Resources.FireEscN),


            new ColorToDrawable(Color.FromArgb(255, 150, 0, 150), Properties.Resources.wallExtinguisherN),
            new ColorToDrawable(Color.FromArgb(255, 50, 200, 50), Properties.Resources.wallExtinguisherE),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 0), Properties.Resources.wallExtinguisherE),
            new ColorToDrawable(Color.FromArgb(255, 255, 0, 50), Properties.Resources.wallExtinguisherS),
            //doors
            new ColorToDrawable(Color.FromArgb(255, 0, 255, 255), Properties.Resources.DoorE),
            new ColorToDrawable(Color.FromArgb(255, 0, 0, 255), Properties.Resources.DoorN),
            new ColorToDrawable(Color.FromArgb(255, 255, 100, 255), Properties.Resources.DoorW),
            new ColorToDrawable(Color.FromArgb(255, 100, 0, 255), Properties.Resources.DoorS),
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
        };
        public List<Color> walkableTiles = new List<Color>() {
            Color.FromArgb(255,255,255,255), // floor
            Color.FromArgb(255,255,170,0),
            //FireEsc
            Color.FromArgb(255, 0,255, 0), // Properties.Resources.FireEscS),
            Color.FromArgb(255, 150,255, 50),//Properties.Resources.FireEscE),
            Color.FromArgb(255, 155,55, 255),// Properties.Resources.FireEscW),
            Color.FromArgb(255, 80,255, 100),// Properties.Resources.FireEscN),

            
            
            //doors
            Color.FromArgb(255, 0, 255, 255),// Properties.Resources.DoorE),
            Color.FromArgb(255, 0, 0, 255),// Properties.Resources.DoorN),
            Color.FromArgb(255, 255, 100, 255),// Properties.Resources.DoorW),
            Color.FromArgb(255, 100, 0, 255),// Properties.Resources.DoorS),
            
        };

        public Statistics()
        {
            InitializeComponent();
            this.Visible = true;
            g = panel3.CreateGraphics();
            PopulateGrid();
            EraserBrush = new SolidBrush(Color.White);
            penBlack = new Pen(Color.FromArgb(255, 0, 0, 0));
            currentPosForAllDead = new List<Tile>();
            currentPosForAllExtinghuisedFires = new List<Tile>();
            sm = new StatisticsManager();
        }

        public void PersonDies(Person p)
        {
            lbDeadCount.Text = sm.IncreaseDeadcount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A person died");
            

            currentPosForAllDead.Add(p.CurrentLocation);
            panel3.Invalidate();

        }

        public void PersonUsesExtinguisher(Tile t)
        {
            lblExtinguishCount.Text = sm.IncreaseFireExtinguisedCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A Person has extinguished a fire");


            currentPosForAllExtinghuisedFires.Add(t);
            panel3.Invalidate();

        }

        public void PersonEscapes()
        {
            lbEscapedCount.Text = sm.IncreaseEscapeCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A person escaped");
        }

        public void PersonSpawns()
        {
            lblSpawnCount.Text = sm.IncreaseSpawnCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A person has spawned");
        }

        public void FireSpawns()
        {
            lbFireSpawnCount.Text = sm.IncreaseFireSpawnCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A fire has spawned");
        }

        public void FireSpreads()
        {
            lbFireSpreadsCount.Text = sm.IncreaseFireSpreadCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A fire has spread");
        }

        public void FireIsExtinguised()
        {
            lblExtinguishCount.Text = sm.IncreaseFireExtinguisedCount().ToString();
            lbHappenings.TopIndex = lbHappenings.Items.Add("A fire has been extinguished");
        }

        public void SimulationFinished()
        {
            sm.simulationFinished();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                try
                {
                    sw.WriteLine("Statistics fire simulation");
                    sw.WriteLine("Date and time: " + System.DateTime.Now.ToString());
                    sw.WriteLine("-----------------------------------------------------------");
                    sw.WriteLine("Number of persons spawned: " + sm.spawnCount.ToString());
                    sw.WriteLine("Number of person who died: " + sm.deadCount.ToString());
                    sw.WriteLine("Number of persons who escaped: " + sm.escapedCount.ToString());
                    sw.WriteLine("Number of fires spawned: " + sm.fireSpawnCount.ToString());
                    sw.WriteLine("Number of fires spread: " + sm.fireSpreadCount.ToString());
                    sw.WriteLine("Number of fires extinguished: " + sm.fireExtinguisedCount.ToString());

                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                finally { sw.Close(); }
            }
        }

        public void LoadCopyOfBitmap(Bitmap newBmp)
        {
            bmp = newBmp;
            panel3.Invalidate();
        }
        void PopulateGrid()
        {
            grid = new Tile[gridSizeX, gridSizeY];

            int currPosX = startDrawPositionX;
            int currPosY = startDrawPositionY;
            // create grid
            for (int y = 0; y < gridSizeY; y++)
            {

                for (int x = 0; x < gridSizeX; x++)
                {
                    grid[x, y] = new Tile(new Vector(currPosX, currPosY), 20, null, null, null, null, nullColor, x, y);
                    currPosX += 20;
                }
                currPosY += 20;
                currPosX = startDrawPositionX;
            }
            endDrawPositionX = grid[24, 20].StartPos.X + grid[24, 20].Size;
            endDrawPositionY = grid[24, 20].StartPos.Y + grid[24, 20].Size;
        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {
            if (currentPosForAllDead != null)
            {
                foreach (Tile t in currentPosForAllDead)
                {
                    DrawTile(t, Properties.Resources.GraveStone);
                }
            }
            if (currentPosForAllExtinghuisedFires != null)
            {
                foreach (Tile t in currentPosForAllExtinghuisedFires)
                {
                    DrawTile(t, Properties.Resources.WaterDr);
                }
            }
            DrawBaseMap();
        }

        public void DrawTile(Tile a, Bitmap b)
        {
            //EraseTile(a);
            Point upperLeft = new Point(a.StartPos.X/2, a.StartPos.Y/2);
            Point upperRight = new Point(a.StartPos.X/2 + 20, a.StartPos.Y/2);
            Point lowerLeft = new Point(a.StartPos.X/2, a.StartPos.Y/2 + 20);
            Point[] points = { upperLeft, upperRight, lowerLeft };

            g.DrawImage(b, points);
        }

        public void EraseTile(Tile a)
        {
            g.FillRectangle(EraserBrush, a.StartPos.X, a.StartPos.Y, a.Size, a.Size);
            g.DrawRectangle(penBlack, a.StartPos.X, a.StartPos.Y, a.Size, a.Size);
        }

        public void DrawBaseMap()
        {
            if (bmp != null)
            {
                int counterX = 1;
                int counterY = 1;
                for (int y = 0; y < gridSizeY + 2; y++)
                {
                    if (counterY != 1 && counterY != 23)
                    {
                        for (int x = 0; x < gridSizeX + 2; x++)
                        {
                            if (counterX != 1 && counterX != 27)
                            {
                                if (bmp.GetPixel(x, y) != Color.FromArgb(255, 255, 255, 255))
                                {
                                    if (bmp.GetPixel(x, y) != outsideColor)
                                    {
                                        foreach (ColorToDrawable a in drs)
                                        {
                                            if (bmp.GetPixel(x, y) == a.Color)
                                            {
                                                Point upperLeft = new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y);
                                                Point upperRight = new Point(grid[x - 1, y - 1].StartPos.X + 20, grid[x - 1, y - 1].StartPos.Y);
                                                Point lowerLeft = new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y + 20);
                                                Point[] points = { upperLeft, upperRight, lowerLeft };

                                                g.DrawImage(a.Bitmap, points);

                                                grid[x - 1, y - 1].colorInBitmap = a.Color;
                                                if (walkableTiles.Contains(grid[x - 1, y - 1].colorInBitmap))
                                                    grid[x - 1, y - 1].walkable = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {

                                            foreach (ColorToDrawable a in drs)
                                            {
                                                if (bmp.GetPixel(x, y) == a.Color)
                                                {
                                                    Point upperLeft = new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y);
                                                    Point upperRight = new Point(grid[x - 1, y - 1].StartPos.X + 20, grid[x - 1, y - 1].StartPos.Y);
                                                    Point lowerLeft = new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y + 20);
                                                    Point[] points = { upperLeft, upperRight, lowerLeft };

                                                    g.DrawImage(a.Bitmap, points);
                                                    //g.DrawImage(a.Bitmap, new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y));
                                                    grid[x - 1, y - 1].colorInBitmap = a.Color;
                                                    grid[x - 1, y - 1].outOfBound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception) { }
                                    }
                                }
                                else
                                {
                                    grid[x - 1, y - 1].colorInBitmap = Color.FromArgb(255, 255, 255, 255);
                                }
                            }
                            counterX++;
                        }
                    }
                    counterY++;
                    counterX = 1;
                }
            }
        }
    }
}
