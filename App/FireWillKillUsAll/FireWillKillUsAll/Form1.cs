using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FireWillKillUsAll
{
    //statistics declarations
    public delegate void DeadHandler(Person person);
    public delegate void EscapedHandler();
    public delegate void ExtinguisherUsed(Tile t);
    public delegate void SpawnHandler();
    public delegate void SpawnFireHandler();
    public delegate void FireSpreadCountHandler();
    public delegate void FireExtinguishHandler();
    public delegate void LoadMapHandler(Bitmap bitmap);
    public delegate void RedrawExtinguisher(Tile t); //draw a wall when the extinguisher is picked up

    public partial class Form1 : Form
    {
        //declerations
        int formW = 1600;
        int formH = 920;
        public static int gridSizeX = 25;
        public static int gridSizeY = 21;
        List<Tile> fireExits;
        List<Tile> fireExtinguishers;
        public List<Person> people;
        List<Person> deadAndSavedPeople;
        
        //statistics declarations
        public Statistics statistics;

        public List<Color> walkableTiles = new List<Color>() {

            Color.FromArgb(155, 155, 155, 155), //Air current
            Color.FromArgb(230, 230, 230, 230), //Vent
            Color.FromArgb(255,255,255,255), //floor
            Color.FromArgb(255,255,170,0), //FireEsc

            Color.FromArgb(180, 255, 170, 0), //window vertical
            Color.FromArgb(180, 255, 150, 0), //window horizontal

            Color.FromArgb(255, 0,255, 0), // Properties.Resources.FireEscS),
            Color.FromArgb(255, 150,255, 50),//Properties.Resources.FireEscE),
            Color.FromArgb(255, 155,55, 255),// Properties.Resources.FireEscW),
            Color.FromArgb(255, 80,255, 100),// Properties.Resources.FireEscN),

            
            
            //doors
            Color.FromArgb(255, 0, 255, 255), // Properties.Resources.DoorE),
            Color.FromArgb(255, 1, 255, 255), // Door e closed
            Color.FromArgb(255, 0, 0, 255), // Properties.Resources.DoorN),
            Color.FromArgb(255, 1, 0, 255), // Properties.Resources.DoorN___Closed),
            Color.FromArgb(255, 255, 100, 255), // Properties.Resources.DoorW),
            Color.FromArgb(255, 255, 101, 255), // Properties.Resources.DoorW___Closed),
            Color.FromArgb(255, 100, 0, 255), // Properties.Resources.DoorS),
            Color.FromArgb(255, 100, 1, 255), //Properties.Resources.DoorS___Closed),

        };

        //grid
        public static Tile[,] grid;
        //drawing
        Bitmap activeBrush;
        Bitmap createdMap;
        int penBoarderWidth;
        Pen penBorder;
        Pen penBlack;
        Brush b1;
        Brush EraserBrush;
        Graphics g;
        Color backgroundColor;
        bool drawingOnMap;
        bool erasing = false;
        Bitmap baseGrid25x21 = Properties.Resources.BaseBitMap25x21v2;
        bool drawingPerson = false; //keep track if drawing a person
        Floor floor;

        //borders of grid
        public static int startDrawPositionX;
        public static int startDrawPositionY;
        public static int endDrawPositionX;
        public static int endDrawPositionY;
        //spreadAlgorithm
       public static Color nullColor = Color.Crimson;
        public static Color outsideColor = Color.FromArgb(255, 123, 123, 123);
        private bool fireSpawnCheck = false;
        Random rand;
        List<Room> rooms;
        bool firesOnMap = false;

        public Timer timer;
        public Timer fireTimer;

        //Used for saving the map while the simulation is running
        public string bitmapFileName;

        //events
        public delegate void ChangeTileColor(int xInArr, int yInArr, int tileSize, Color c);
        public event ChangeTileColor DrawOnTile;

        //statistics events
        public event SpawnHandler SomeoneSpawns;
        public event SpawnFireHandler FireSpawns;
        public event FireExtinguishHandler FireIsExtinguished;
        public event LoadMapHandler MapIsLoaded;

        public Form1()
        {
            InitializeComponent();

            statistics = new Statistics(); // the window for Statistics
            //SERVICE FOR UNITY
            /*using(ServiceHost sh = new ServiceHost(typeof(UnityService1)))
             {
                 sh.Open();
             }*/
            
            ServiceHost host = new ServiceHost(typeof(UnityService1));


            host.Open(); // start hosting

             Dump.ChangeBitmap(Properties.Resources.BaseBitMap25x21v2);
            //Dump.SendBitMap(Properties.Resources.BaseBitMap25x21v2, 25, 21);      /deadLocks so only change it on LoadMap()

            //declerations
            this.Width = formW;

            this.Height = formH;
            
            //drawing graphics
            g = this.CreateGraphics();
            penBoarderWidth = 20; // must be even
            backgroundColor = Color.White;
            drawingOnMap = false;
            //set up brush for map Creation
            activeBrush = Properties.Resources.wallHor;
            pictureBox1.Image = activeBrush;
            rand = new Random();
            rooms = new List<Room>();

            penBorder = new Pen(Color.FromArgb(255, 31, 133, 222), penBoarderWidth);
            penBlack = new Pen(Color.FromArgb(255, 0, 0, 0));
            b1 = new SolidBrush(Color.FromArgb(100, 31, 222, 31));
            EraserBrush = new SolidBrush(backgroundColor);

            startDrawPositionX = penBoarderWidth;
            startDrawPositionY = penBoarderWidth;

            //Initialize lists for extinguishers and exits
            fireExits = new List<Tile>();
            fireExtinguishers = new List<Tile>();

            //Start methods
            CreateButtonImage();
            //grid
            Grid.PopulateGrid();

            //add methods to events
            DrawOnTile += new ChangeTileColor(PaintTile);

            timer = new Timer();
            timer.Interval = 1000;

            fireTimer = new Timer();
            fireTimer.Interval = 2000;

            if (cbSimSpeed.SelectedItem != null)
            {
                timer.Interval = Convert.ToInt32(1000 * Convert.ToDouble(cbSimSpeed.SelectedItem));
                fireTimer.Interval = Convert.ToInt32(1000 * Convert.ToDouble(cbSimSpeed.SelectedItem));
            }

            this.people = new List<Person>();
            this.deadAndSavedPeople = new List<Person>();

            //subscribe methods of statistics form
            SomeoneSpawns += statistics.PersonSpawns;
            FireSpawns += statistics.FireSpawns;
            FireIsExtinguished += statistics.FireIsExtinguised;
            MapIsLoaded += statistics.LoadCopyOfBitmap;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            ResetGraphics();
            //tests
            #region test
            /*
            g.FillRectangle(b1, startDrawPositionX, startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 40 + startDrawPositionX, 40 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 80 +startDrawPositionX, 80 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 120 + startDrawPositionX, 120 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 160 + startDrawPositionX, 160 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 200 + startDrawPositionX, 200 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 240 + startDrawPositionX, 240 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 280 + startDrawPositionX, 280 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 320 + startDrawPositionX, 320 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 360 + startDrawPositionX, 360 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 400 + startDrawPositionX, 400 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 440 + startDrawPositionX, 440 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 480 + startDrawPositionX, 480 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 520 + startDrawPositionX, 520 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 560 + startDrawPositionX, 560 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 600 + startDrawPositionX, 600 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 640 + startDrawPositionX, 640 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 680 + startDrawPositionX, 680 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 720 + startDrawPositionX, 720 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 760 + startDrawPositionX, 760 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 800 + startDrawPositionX, 800 + startDrawPositionY, 40, 40);

            g.FillRectangle(b1, 840 + startDrawPositionX, 800 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 880 + startDrawPositionX, 800 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 920 + startDrawPositionX, 800 + startDrawPositionY, 40, 40);
            g.FillRectangle(b1, 960 + startDrawPositionX, 800 + startDrawPositionY, 40, 40);
           
            */
            /* g.FillRectangle(b1, startDrawPositionX, startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 50 + startDrawPositionX, 50 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 100 + startDrawPositionX, 100 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 150 + startDrawPositionX, 150 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 200 + startDrawPositionX, 200 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 250 + startDrawPositionX, 250 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 300 + startDrawPositionX, 300 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 350 + startDrawPositionX, 350 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 400 + startDrawPositionX, 400 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 450 + startDrawPositionX, 450 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 500 + startDrawPositionX, 500 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 550 + startDrawPositionX, 550 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 600 + startDrawPositionX, 600 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 650 + startDrawPositionX, 650 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 700 + startDrawPositionX, 700 + startDrawPositionY, 50, 50);
             g.FillRectangle(b1, 750 + startDrawPositionX, 750 + startDrawPositionY, 50, 50);/*
             /*
             g.DrawLine(penBlack, startDrawPositionX,startDrawPositionY, startDrawPositionX, startDrawPositionY+100);
             g.DrawLine(penBlack, startDrawPositionX, startDrawPositionY, startDrawPositionX+100, startDrawPositionY);*/

            #endregion test
        }

        public void SetPeopleMovesInBuffer(Object sender, EventArgs e)
        {
            Buffer.GetAllMoves(people);
        }

        //add to paintEvent
        void PaintTile(int tileXGrid, int tileYGrid, int size, Color c)
        {
            Brush b = new SolidBrush(c);
            g.FillRectangle(b, grid[tileXGrid, tileYGrid].StartPos.X, grid[tileXGrid, tileYGrid].StartPos.Y, size, size);
        }

        void PaintTile(Tile t, Color c)
        {
            Brush b = new SolidBrush(c);
            g.FillRectangle(b, t.StartPos.X, t.StartPos.Y, t.Size, t.Size);
        }

        private void btnT1_Click(object sender, EventArgs e)
        {
            DisablePersonSpawn();

            Color c = Color.Gainsboro;


            for (int y = 0; y < gridSizeY; y++)
            {

                for (int x = 0; x < gridSizeX; x++)
                {
                    Random r = new Random();
                    int a = r.Next(0, 7);
                    switch (a)
                    {
                        case 0:
                            c = Color.Aqua;
                            break;
                        case 1:
                            c = Color.YellowGreen;
                            break;
                        case 2:
                            c = Color.Salmon;
                            break;
                        case 3:
                            c = Color.MediumPurple;
                            break;
                        case 4:
                            c = Color.Khaki;
                            break;
                        case 5:
                            c = Color.ForestGreen;
                            break;
                        case 6:
                            c = Color.CornflowerBlue;
                            break;
                        default:
                            c = Color.Fuchsia;
                            break;
                    }

                    DrawOnTile(x, y, grid[x, y].Size, c);
                }


            }




            
        }

        private void btnLoadBase_Click(object sender, EventArgs e)
        {
            DisablePersonSpawn();
            deadAndSavedPeople.Clear();
            people.Clear();
            ClearFloorAndFires();
            ResetGraphics();

            LoadBaseLayout();
            LoadFloor();
            Dump.ChangeBitmap(Properties.Resources.BaseBitMap25x21v2);
            createdMap = Properties.Resources.BaseBitMap25x21v2;
            btnStartSim.Enabled = true;
        }

        private void LoadFloor()
        {
            //Floor instantiation
            floor = new Floor(GetRooms());

            //This event is called from the floor which is called from the room when a fire spreads
            //Will make a less confusing solution some day.
            floor.DrawFireEvent += DrawTileNoOverwrite;

            btnSpawnPerson.Enabled = true;
            btnSpawnRandomPeople.Enabled = true;
            fireTimer.Enabled = true;
            fireTimer.Tick += floor.OnTick;
            fireTimer.Tick += TempCheck;
            fireTimer.Tick += SetPeopleMovesInBuffer; // set people moves for unity in buffer
        }

        private void TempCheck(object sender, EventArgs e)
        {
            if(floor != null)
            {
                lbRoomTemp.Items.Clear();
                foreach (Room r in floor.rooms)
                {
                    lbRoomTemp.Items.Add("Room Number " + r.roomNo + " has temp: " + r.temp + ", fuel: " + r.fuel);
                }
                foreach (Tile t in grid)
                {
                    t.AddNeighbours();
                }
            }
        }

        void LoadBaseLayout()
        {
            //MessageBox.Show(baseGrid25x21.ToString());

            Bitmap ma = Grid.AddOutlineToBitmap(baseGrid25x21);
            Bitmap markedOutsides = Grid.MarkOutsideBitmap(ma, 0, 0);
            pictureBox2.Image = markedOutsides;
            DrawFromBitmap(ma);
            createdMap = ma;
            if(MapIsLoaded != null)
            {
                MapIsLoaded.Invoke(ma);
            }
        }

        // 25x21
        /* void DrawFromBitmap(Bitmap bmp25x21)
         {


             for (int y = 0; y < gridSizeY; y++)
             {

                 for (int x = 0; x < gridSizeX; x++)
                 {
                     if (bmp.GetPixel(x, y) != Color.FromArgb(255,255,255,255))
                     {
                         foreach (ColorToDrawable a in drs)
                         {

                             if (bmp.GetPixel(x, y) == a.Color)
                             {
                                 g.DrawImage(a.Bitmap, new Point(grid[x, y].StartPos.X , grid[x, y].StartPos.Y ));

                                 break;
                             }
                         }
                     }

                 }


             }
         }*/
        // 27x23 does not read from the out most ring of pixels
        void DrawFromBitmap(Bitmap bmp)
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
                                    foreach (ColorToDrawable a in Grid.Drs)
                                    {

                                        if (bmp.GetPixel(x, y) == a.Color)
                                        {
                                           
                                            g.DrawImage(a.Bitmap, new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y));
                                            grid[x - 1, y - 1].colorInBitmap = a.Color;
                                            if (walkableTiles.Contains(grid[x-1,y-1].colorInBitmap))
                                                grid[x - 1, y - 1].walkable = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {

                                        foreach (ColorToDrawable a in Grid.Drs)
                                        {

                                            if (bmp.GetPixel(x, y) == a.Color)
                                            {
                                                g.DrawImage(a.Bitmap, new Point(grid[x - 1, y - 1].StartPos.X, grid[x - 1, y - 1].StartPos.Y));
                                                grid[x - 1, y - 1].colorInBitmap = a.Color;
                                                grid[x-1, y-1].outOfBound = true;
                                                break;
                                            }
                                        }
                                        
                                       
                                    }
                                    catch (Exception)
                                    {

                                       
                                    }
                                    
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


            //Check all tiles and store fire exits and extinguishers
            for(int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    //IF TILE IS WALKABLE
                    if (walkableTiles.Contains(grid[x, y].colorInBitmap))
                    {
                        grid[x, y].walkable = true;
                    }

                    //IF TILE IS A FIRE EXIT
                    if(grid[x,y].colorInBitmap == Color.FromArgb(255,0,255,0) ||
                        grid[x,y].colorInBitmap == Color.FromArgb(255,150,255,50) ||
                        grid[x, y].colorInBitmap == Color.FromArgb(255, 155, 55, 255) ||
                        grid[x, y].colorInBitmap == Color.FromArgb(255, 80, 255, 100))
                    {
                        fireExits.Add(grid[x, y]);
                    }

                    //IF TILE IS EXTINGUISHER
                    else if(grid[x, y].colorInBitmap == Color.FromArgb(255, 150, 0, 150) ||
                            grid[x, y].colorInBitmap == Color.FromArgb(255, 50, 200, 50) ||
                            grid[x, y].colorInBitmap == Color.FromArgb(255, 255, 0, 0) ||
                            grid[x, y].colorInBitmap == Color.FromArgb(255, 255, 0, 50))
                    {
                        fireExtinguishers.Add(grid[x, y]);
                    }
                }
            }
            

        }

        public void DrawSquareGrid()
        {
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int a = 0; a < gridSizeY; a++)
                {
                    g.DrawRectangle(penBlack, grid[i, a].StartPos.X, grid[i, a].StartPos.Y, grid[i, a].Size, grid[i, a].Size);
                }
            }
        }

        public void DrawTile(Tile a)
        {
            g.DrawRectangle(penBlack, a.StartPos.X, a.StartPos.Y, a.Size, a.Size);
        }

        public void DrawTile(Tile a, Bitmap b)
        {
            Tile drawOn;
            int x;
            int y;
            bool t = Grid.SearchGrid(new Vector(a.StartPos.X, a.StartPos.Y), out drawOn, out x, out y);

            Color color = Grid.GetColorOfActiveBrushFromBitmap(b);
            EraseTile(a);
            a.colorInBitmap = color;
            g.DrawImage(b, new Point(a.StartPos.X, a.StartPos.Y));
            createdMap.SetPixel(x, y, color);
        }

        public void EraseTile(Tile a)
        {
            Tile drawOn;
            int x;
            int y;
            bool t = Grid.SearchGrid(new Vector(a.StartPos.X, a.StartPos.Y), out drawOn, out x, out y);

            g.FillRectangle(EraserBrush, a.StartPos.X, a.StartPos.Y, a.Size, a.Size);
            g.DrawRectangle(penBlack, a.StartPos.X, a.StartPos.Y, a.Size, a.Size);
            a.colorInBitmap = Color.FromArgb(255, 255, 255, 255);
            createdMap.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
        }

        public void DrawTileNoOverwrite(Tile a, Bitmap b)
        {
            if(a != null)
            {
                Tile drawOn;
                int x;
                int y;
                bool t = Grid.SearchGrid(new Vector(a.StartPos.X, a.StartPos.Y), out drawOn, out x, out y);

                byte[] bytesForColor = new byte[3];
                bytesForColor = Grid.encodeColor(a, Grid.GetColorOfActiveBrushFromBitmap(b));
                Color AddedColor = Color.FromArgb(254, bytesForColor[0], bytesForColor[1], bytesForColor[2]);
                List<Bitmap> ColorsToDraw = Grid.decodeColor(AddedColor);

                foreach(Bitmap bmp in ColorsToDraw)
                {
                    if (Grid.GetColorOfActiveBrushFromBitmap(bmp) != Color.FromArgb(255))
                    {
                        g.DrawImage(bmp, new Point(a.StartPos.X, a.StartPos.Y));
                    }
                }
                a.colorInBitmap = AddedColor;
                createdMap.SetPixel(x, y, AddedColor);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DisablePersonSpawn();
            deadAndSavedPeople.Clear();
            people.Clear();
            fireExtinguishers.Clear();
            fireExits.Clear();
            ClearFloorAndFires();
            ResetGraphics();

            btnStartSim.Enabled = true;
            btnStopSim.Enabled = false;
            btnSpawnPerson.Enabled = false;
            btnSpawnRandomPeople.Enabled = false;
            btnSpawnFireAtClick.Enabled = false;
            btnSpawnRandomFire.Enabled = false;
        }

        void ResetGraphics()
        {
            g.Clear(backgroundColor);
            //draw boarder
            g.DrawRectangle(penBorder, penBoarderWidth / 2, penBoarderWidth / 2, formW - 40, formH - 60); // the draw method cuts of half of the line when it starts from 0,0 so we add half of the lines width to the starting coordinates
            g.DrawLine(penBorder, 1030, penBoarderWidth / 2, 1030, formH - 58);
            //fill bottom
            g.DrawLine(penBorder, 0, formH - 30, 1600, formH - 30);

            DrawSquareGrid();
        }

        //Sets all variables of tiles to false
        void ResetTiles()
        {
            foreach (Tile t in grid)
            {
                t.colorInBitmap = Color.FromArgb(255, 255, 255, 255);
                t.walkable = false;
                t.hasDoor = false;
                t.hasVent = false;
                t.isExtinguished = false;
                t.personOnTile = false;
            }
        }

        void ClearFloorAndFires()
        {
            ResetTiles();
            if (floor != null) //prevent crash when clear is pressed before floor is created
            {
                timer.Tick -= Timer_tick;
                fireTimer.Tick -= floor.OnTick;
                fireTimer.Tick -= TempCheck;
                foreach (Room r in floor.rooms)
                {
                    r.Extinguished -= FireExtinguished;
                    foreach (AirFlow a in r.AirFlows)
                    {
                        a.AirFlowCreated -= DrawAirFlow;
                    }
                }
                floor.ClearAll();
                floor = null;
            }
        }
         
        void CreateButtonImage()
        {
            btnD1.Image = Properties.Resources.wallHor;
            btnD2.Image = Properties.Resources.wallVer;
            btnD3.Image = Properties.Resources._4Wall;
            //btnD4.Image = Properties.Resources;
            //3Wall
            btnD5.Image = Properties.Resources._3WallN;
            btnD6.Image = Properties.Resources._3WallE;
            btnD7.Image = Properties.Resources._3WallS;
            btnD8.Image = Properties.Resources._3WallW;
            //Door
            btnD9.Image = Properties.Resources.DoorN;
            btnD10.Image = Properties.Resources.DoorE;
            btnD11.Image = Properties.Resources.DoorS;
            btnD12.Image = Properties.Resources.DoorW;
            //FireEsc
            btnD13.Image = Properties.Resources.FireEscN;
            btnD14.Image = Properties.Resources.FireEscE;
            btnD15.Image = Properties.Resources.FireEscS;
            btnD16.Image = Properties.Resources.FireEscW;
            //Extinguisher
            btnD17.Image = Properties.Resources.wallExtinguisherN;
            btnD18.Image = Properties.Resources.wallExtinguisherE;
            btnD19.Image = Properties.Resources.wallExtinguisherS;
            btnD20.Image = Properties.Resources.wallExtinguisherW;
            //Extinguisher
            btnD21.Image = Properties.Resources.CornerBotLeft;
            btnD22.Image = Properties.Resources.CornerBotRight;
            btnD23.Image = Properties.Resources.CornerTopLeft;
            btnD24.Image = Properties.Resources.CornerTopRight;
        }

        void MakePanelVisible(bool t)
        {
            if (t)
            {
                panelCreateFloor.Enabled = true;
                panelCreateFloor.BackColor = Color.Red;
            }
            else
            {
                panelCreateFloor.Enabled = false;
                panelCreateFloor.BackColor = Color.FromArgb(255, 247, 84, 84);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Tile drawOn;
            int x;
            int y;
            bool t = Grid.SearchGrid(new Vector(e.X, e.Y), out drawOn, out x, out y);

            //MessageBox.Show(drawOn.colorInBitmap.ToString());

            //SpreadRed(drawOn);
            //MessageBox.Show($"{e.X}, {e.Y}");
            if (drawingOnMap)
            {
                if (t)
                {
                    if (!erasing)
                    {
                        DrawTile(drawOn, activeBrush);
                        Color n = Grid.GetColorOfActiveBrushFromBitmap(activeBrush);

                        createdMap.SetPixel(x, y, n);
                        //MessageBox.Show($"This pos:{drawOn.StartPos.X},{drawOn.StartPos.Y},Up:{drawOn.up.StartPos.X},{drawOn.up.StartPos.Y},down:{drawOn.down.StartPos.X},{drawOn.down.StartPos.Y},left:{drawOn.left.StartPos.X},{drawOn.left.StartPos.Y},Right:{drawOn.right.StartPos.X},{drawOn.right.StartPos.Y}");
                        //MessageBox.Show($"possitionX:{drawOn.positionX}, possitionY:{drawOn.positionY}");
                    }
                    else
                    {
                        EraseTile(drawOn);
                    }
                }
            }
            if (fireSpawnCheck)
            {
                if (!drawOn.isExtinguished && drawOn.walkable && !drawOn.onFire)
                {
                    SetOnFire(drawOn);
                    if (FireSpawns != null)
                    {
                        FireSpawns.Invoke();
                    }
                }
                Color n = Grid.GetColorOfActiveBrushFromBitmap(activeBrush);
            }
            if (drawingPerson) //if spawning a person
            {
                if (t)
                {
                    if (drawOn.walkable) //if the tile is compatible
                    {
                        if (!drawOn.onFire)
                        {
                            Person p = SpawnNewPerson(drawOn);
                            p.SomeoneDies += statistics.PersonDies;
                            p.SomeoneEscapes += statistics.PersonEscapes;
                            p.RedrawHandler += this.RedrawExtinguisher;

                            if (p.Personality == PERSONALITY.HERO_OF_MANKIND)
                            {
                                activeBrush = Properties.Resources.johnny;

                            }
                            else if (p.Personality == PERSONALITY.PUSSY)
                            {
                                activeBrush = Properties.Resources.courage;
                            }
                            else if (p.Personality == PERSONALITY.SELFISH_PRICK)
                            {
                                activeBrush = Properties.Resources.simpson;
                            }
                            else
                            {
                                activeBrush = Properties.Resources.shaggy;
                            }

                            pictureBox1.Image = activeBrush;
                            DrawTileNoOverwrite(drawOn, activeBrush);
                            Color n = Grid.GetColorOfActiveBrushFromBitmap(activeBrush);

                            this.people.Add(p);
                        }
                    }
                }
                
            }
            
        }

        //Test for spread
        /*   public void SpreadRed(Tile t)
           {
               foreach (Tile i in GetConnectedList(t))
               {
                   PaintTile(i, Color.Red);
               } 
           }*/
        //mark outOfBounds
        public void markOut()
        {

        }

        private void BtnStopSim_Click(object sender, EventArgs e)
        {
            fireTimer.Enabled = false;
            fireTimer.Tick -= floor.OnTick;
            fireTimer.Tick -= TempCheck;
            timer.Enabled = false;
            timer.Stop();
            timer.Tick -= Timer_tick;
            btnStartSim.Enabled = true;
            btnStopSim.Enabled = false;

            btnSpawnFireAtClick.Enabled = false;
            btnSpawnRandomFire.Enabled = false;
        }

        private void BtnStartSim_Click(object sender, EventArgs e)
        {
            if(floor != null)
            {
                fireTimer.Enabled = true;
                timer.Enabled = true;
                timer.Start();
                timer.Tick += Timer_tick;

                btnSpawnFireAtClick.Enabled = true;
                btnSpawnRandomFire.Enabled = true;

                btnStartSim.Enabled = false;
                btnStopSim.Enabled = true;
                btnSpawnRandomFire.Enabled = true;
            }
        }

        private void btnCreateMap_Click(object sender, EventArgs e)
        {
            //create new bitmap and set all it's pixels to white
            drawingOnMap = true;
            DisablePersonSpawn();
            btnSpawnPerson.Enabled = false;
            createdMap = new Bitmap(gridSizeX, gridSizeY);
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int a = 0; a < gridSizeY; a++)
                {
                    createdMap.SetPixel(i, a, Color.White);
                }
            }
            MakePanelVisible(true);
            ResetGraphics();
        }

        #region MapCreateOnClicks

        private void btnExitCreation_Click(object sender, EventArgs e)
        {
            drawingOnMap = false;
            erasing = false;
            MakePanelVisible(false);
            btnSpawnPerson.Enabled = true;
            fireExits.Clear();
            fireExtinguishers.Clear();
            people.Clear();
            deadAndSavedPeople.Clear();
        }

        private void btnD1_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallHor;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD2_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallVer;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD3_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources._4Wall;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD5_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources._3WallN;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD6_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources._3WallE;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD7_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources._3WallS;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD8_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources._3WallW;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD9_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.DoorN___Closed;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }
        private void btnD10_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.DoorE___Closed;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD11_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.DoorS___Closed;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD12_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.DoorW___Closed;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD13_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.FireEscN;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD14_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.FireEscE;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD15_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.FireEscS;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD16_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.FireEscW;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD17_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallExtinguisherN;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD18_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallExtinguisherE;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD19_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallExtinguisherS;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD20_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wallExtinguisherW;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD21_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.CornerBotLeft;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD22_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.CornerBotRight;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD23_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.CornerTopLeft;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD24_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.CornerTopRight;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD25_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.wIndowHor;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD26_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.windowVer;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        private void btnD27_Click(object sender, EventArgs e)
        {
            activeBrush = Properties.Resources.Vent;
            pictureBox1.Image = activeBrush;
            erasing = false;
        }

        #endregion MapCreateOnClicks

        private void btnEraserMapCreation_Click(object sender, EventArgs e)
        {
            erasing = true;
            DisablePersonSpawn();
        }

        private void btnSaveBitmap_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = Grid.AddOutlineToBitmap(createdMap);
                bmp.Save(dialog.FileName + ".png", ImageFormat.Png);
                bitmapFileName = dialog.FileName;
            }
        }

        private void btnLoadBitmap_Click(object sender, EventArgs e)
        {
            DisablePersonSpawn();
            deadAndSavedPeople.Clear();
            people.Clear();
            ClearFloorAndFires();
            ResetGraphics();

            DisablePersonSpawn();

            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    Bitmap b;
                    b = (Bitmap)Image.FromFile(filePath);
                    Grid.MarkOutsideBitmap(b, 0, 0);
                    DrawFromBitmap(b);

                    createdMap = b;
                    //Dump.ChangeBitmap(b);// send bitmap to Unity
                    Dump.SendBitMap(b, 25, 21);

                    if(MapIsLoaded != null)
                    {
                        MapIsLoaded.Invoke(b);
                    }
                }
                bitmapFileName = openFileDialog.FileName;
            }

            LoadFloor();
            btnStartSim.Enabled = true;
        }

        /*
        public Bitmap addOutlineToBitmap(Bitmap b)
        {
            Bitmap newB = new Bitmap(27, 23);
            int counterX = 0;
            int counterY = 0;
            for (int y = 0; y < gridSizeY + 2; y++)
            {
                if (counterY != 0 && counterY != 22)
                {
                    for (int x = 0; x < gridSizeX + 2; x++)
                    {
                        if (counterX != 0 && counterX != 26)
                        {
                            Color c = b.GetPixel(x-1, y-1);
                            newB.SetPixel(x, y,c);
                          //  MessageBox.Show($"Main {x},{y}");

                        }
                        else
                        {
                            newB.SetPixel(x, y, Color.FromArgb(255, 123, 123, 123));
                           // MessageBox.Show($"x else {x},{y}");
                        }
                        counterX++;

                    }


                }
                else
                {
                    // newB.SetPixel(counterX, y, Color.FromArgb(255, 123, 123, 123));
                    // MessageBox.Show($"y else {counterX},{y}");

                    for (int x = 0; x < gridSizeX + 2; x++)
                    {                       
                            newB.SetPixel(x , y , Color.FromArgb(255, 123, 123, 123));
                            //MessageBox.Show($"Main secound{x},{y}");

                        counterX++;

                    }
                }

                counterY++;
                counterX = 0;
            }

            return newB;
        }*/

        void Timer_tick(object sender, EventArgs e)
        {
            firesOnMap = false;
            if(floor != null)
            {
                foreach (Room r in floor.rooms)
                {
                    if (r.fires.Count > 0)
                    {
                        foreach (Fire f in r.fires)
                        {
                            if (!f.isExtinguished)
                            {
                                firesOnMap = true;
                            }
                        }
                    }
                }
            }
            if (people.Count > 0 && firesOnMap) //if there are people spawned and fires on the map
            {
                try
                {
                    foreach (Person p in people)
                    {
                        if (!p.IsSaved && !p.IsDead)
                        {
                            MovePerson(p);
                        }
                        else
                        {
                            people.Remove(p);
                            deadAndSavedPeople.Add(p);

                            //if person is saved, disappear from the map and redraw the exit
                            if (p.IsSaved)
                            {
                                DrawTile(p.CurrentLocation, Grid.GetBitmapOfActiveBrushFromColor(p.exitDoorColor));
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    //just to prevent crash
                }
            }
        }

        private void DrawAirFlow(List<Tile> tiles)
        {
            if(tiles != null)
            {
                foreach (Tile t in tiles)
                {
                    if (t.colorInBitmap != Color.FromArgb(155, 155, 155, 155) && t.colorInBitmap != Color.FromArgb(155, 155, 155, 160))
                    {
                        DrawTileNoOverwrite(t, Properties.Resources.airHor);
                    }
                }
            }
        }

        private void btnSpawnFireAtClick_Click_1(object sender, EventArgs e)
        {
            DisablePersonSpawn();

            drawingOnMap = false;
            if (fireSpawnCheck != true)
            {
                btnSpawnFireAtClick.Text = "Cancel";
                fireSpawnCheck = true;
                btnSpawnPerson.Enabled = false;
            }
            else
            {
                btnSpawnFireAtClick.Text = "Spawn Fire on Click";
                fireSpawnCheck = false;
                btnSpawnPerson.Enabled = true;
            }
        }

        private List<Room> GetRooms()
        {
            int counter = 1;
            List<Room> rooms = new List<Room>();
            bool newRoom = true;
            foreach(Tile t in grid)
            {
                if (t.colorInBitmap == Color.FromArgb(230, 230, 230, 230))
                {
                    t.hasVent = true;
                    t.colorInBitmap = Color.FromArgb(255, 255, 255, 255);
                }
            }
            foreach (Tile t in grid)
            {
                if (t.colorInBitmap == Color.FromArgb(255, 255, 255, 255))
                {
                    for (int i = 0; i < rooms.Count(); i++)
                    {
                        foreach (Tile tile in rooms[i].tiles)
                        {
                            if (t == tile)
                            {
                                newRoom = false;
                            }
                        }
                    }
                    if (newRoom)
                    {
                        Room newR = new Room(Grid.GetConnectedList(t), counter);
                        
                        foreach (AirFlow a in newR.AirFlows)
                        {
                            a.AirFlowCreated += DrawAirFlow;
                            a.CreateCurrent();
                        }
                         

                        //subcribe method to event for statistics
                        newR.FireSpreadCount += statistics.FireSpreads;

                        counter++;

                        newR.passOnStatisticForm(statistics);
                        rooms.Add(newR);
                    }
                    else
                    {
                        newRoom = true;
                    }
                }
            }
            foreach(Room r in rooms)
            {
                r.Extinguished += FireExtinguished;
            }
            return rooms;
        }

        public void SetOnFire(Tile t)
        {
            Room r = floor.FindRoom(t);
            r.AddFire(new Fire(t));
            DrawTileNoOverwrite(t, Properties.Resources.FireTile);
            r.GetFire(t).Extinguished += FireExtinguished;
            r.GetFire(t).SomeoneUsedExtinguisher += statistics.PersonUsesExtinguisher;
            firesOnMap = true;
        }

        private void btnSpawnPerson_Click(object sender, EventArgs e)
        {
            drawingOnMap = false;
            if (!drawingPerson)
            {
                this.drawingPerson = true;
                erasing = false;
                btnSpawnPerson.Text = "Cancel";
            }
            else
            {
                erasing = false;
                DisablePersonSpawn();
            }
        }
        
        //disable person spawn when any other button is clicked
        private void DisablePersonSpawn()
        {
            drawingPerson = false;
            btnSpawnPerson.Text = "Spawn Person";
            
        }

        private Person SpawnNewPerson(Tile start)
        {
            Person p;

            if(cbPersonality.Text == "Random")
            {
                p = new Person(start, fireExits, fireExtinguishers, grid); //Spawn person with a random personality
            }
            else
            {
                PERSONALITY pers; //temporary holder for the personality
                
                if(cbPersonality.Text == "Hero")
                {
                    pers = PERSONALITY.HERO_OF_MANKIND; //assign a Hero personality
                }
                else if(cbPersonality.Text == "Pussy")
                {
                    pers = PERSONALITY.PUSSY; //assign a Pussy personality
                }
                else if(cbPersonality.Text == "Selfish")
                {
                    pers = PERSONALITY.SELFISH_PRICK; //assign a Selfish personality
                }
                else
                {
                    pers = PERSONALITY.SHAGGY_FROM_SCOOBY_DOO; //assign a Shaggy
                }

                p = new Person(start, fireExits, fireExtinguishers, grid, pers); //Spawn person with chosen personality
            }

            if (SomeoneSpawns != null)
            {
                SomeoneSpawns.Invoke();
            }

            return p;
        }

        private void MovePerson(Person p)
        {
            Bitmap current; //store the current image for the person

            if (p.Personality == PERSONALITY.HERO_OF_MANKIND)
            {
                p.RunForExtinguisher();
                current = Properties.Resources.johnny;
            }
            else if(p.Personality == PERSONALITY.PUSSY)
            {
                p.RunForTheExit();
                current = Properties.Resources.courage;
            }
            else if (p.Personality == PERSONALITY.SELFISH_PRICK)
            {
                p.ExitOrExtinguisher();
                current = Properties.Resources.simpson;
            }
            else
            {
                p.BeShaggy();
                current = Properties.Resources.shaggy;
            }

            if(p.PreviousLocation != p.CurrentLocation && !p.PreviousLocation.onFire)
            {
                if (p.PreviousLocation.colorInBitmap.A == 254)
                {
                    List<Bitmap> bmps = Grid.decodeColor(p.PreviousTileColor);
                    foreach(Bitmap bmp in bmps)
                    {
                        if(personCheck(bmp))
                            DrawTile(p.PreviousLocation, bmp); // color the previous tile what it originally was
                    }
                }
                else
                {
                    DrawTile(p.PreviousLocation, Grid.GetBitmapOfActiveBrushFromColor(p.PreviousTileColor)); // color the previous tile what it originally was
                }
                if (p.PreviousLocation.hasDoor)
                {
                    //OpenDoor(p.PreviousLocation, p.LeaveDoorOpen, p.PreviousTileColor);
                }
            }
            if(!p.CurrentLocation.onFire)
                DrawTileNoOverwrite(p.CurrentLocation, current); //draw the person on the new tile
            else
                p.Die();

        }

        private bool personCheck(Bitmap bmp)
        {
            if (Grid.GetColorOfActiveBrushFromBitmap(bmp) == Color.FromArgb(255, 2, 3, 4) || Grid.GetColorOfActiveBrushFromBitmap(bmp) == Color.FromArgb(255, 2, 2, 2) ||
                Grid.GetColorOfActiveBrushFromBitmap(bmp) == Color.FromArgb(255, 3, 3, 3) || Grid.GetColorOfActiveBrushFromBitmap(bmp) == Color.FromArgb(255, 4, 4, 4))
                return false;
            else
                return true;
        }

        //Whenever a person passes through a door there will be a chance that the person opens
        //or closes the door after him - this will draw the result
        private void OpenDoor(Tile t, bool open, Color c)
        {
            List<Color> colors = new List<Color>();
            if (t.colorInBitmap.A == 254)
            {
                foreach (Bitmap bmp in Grid.decodeColor(t.colorInBitmap))
                {
                    colors.Add(Grid.GetColorOfActiveBrushFromBitmap(bmp));
                }
            }
            else
            {
                colors.Add(c);
            }

            if (open)
            {
                if (colors.Contains(Color.FromArgb(255, 1, 255, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorE);
                }
                if (colors.Contains(Color.FromArgb(255, 1, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorN);
                }
                if (colors.Contains(Color.FromArgb(255, 255, 101, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorW);
                }
                if (colors.Contains(Color.FromArgb(255, 100, 1, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorS);
                }

                if (colors.Contains(Color.FromArgb(255, 0, 255, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorE);
                }
                if (colors.Contains(Color.FromArgb(255, 0, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorN);
                }
                if (colors.Contains(Color.FromArgb(255, 255, 100, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorW);
                }
                if (colors.Contains(Color.FromArgb(255, 100, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorS);
                }
            }
            if (!open)
            {
                if (colors.Contains(Color.FromArgb(255, 0, 255, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorE___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 0, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorN___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 255, 100, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorW___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 100, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorS___Closed);
                }

                if (colors.Contains(Color.FromArgb(255, 1, 255, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorE___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 1, 0, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorN___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 255, 101, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorW___Closed);
                }
                if (colors.Contains(Color.FromArgb(255, 100, 1, 255)))
                {
                    DrawTile(t, Properties.Resources.DoorS___Closed);
                }
            }
        }

        private void btnToUnity_Click(object sender, EventArgs e)
        {
            Dump.Info = tbToUnity.Text;
        }

        private void FireExtinguished(Tile t)
        {
            DrawTile(t, Properties.Resources.CharredTile);

            //invoke event for statistics
            if (FireIsExtinguished != null)
            {
                FireIsExtinguished.Invoke();
            }
        }

        private void cbSimSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer.Interval = Convert.ToInt32(1000 * Convert.ToDouble(cbSimSpeed.SelectedItem.ToString()));
            fireTimer.Interval = Convert.ToInt32(2000 * Convert.ToDouble(cbSimSpeed.SelectedItem.ToString()));
        }

        private void btnSpawnRandomFire_Click(object sender, EventArgs e)
        {
            if(floor != null)
            {
                floor.RandomFire();
            }
        }


        //Method is called via an event to redraw the extinguisher when it has been picked up
        private void RedrawExtinguisher(Tile t)
        {
            if (t.colorInBitmap == Color.FromArgb(255, 150, 0, 150) || t.colorInBitmap == Color.FromArgb(255, 255, 0, 50)) //if up or down
                this.DrawTile(t, Properties.Resources.wallHor); //draw horizontal wall
            else                                                //if left or right
                this.DrawTile(t, Properties.Resources.wallVer); //draw vertical wall
        }


        //Spawn the given number of people randomly on the map
        private void btnSpawnRandomPeople_Click(object sender, EventArgs e)
        {
            int numberToSpawn = (int)numPeopleSpawn.Value; //the value provided
            int availableSpots = 0; //the available spots to place a person
            int position; //the tile on which to spawn person
            Bitmap brush;
            Random rand = new Random((int)DateTime.Now.Ticks);
            List<Tile> available = new List<Tile>(); //the actual available tiles

            foreach(Tile t in grid) //for all the tiles
            {
                if(t.walkable && !t.onFire && !t.personOnTile) //if the tile is walkable and free
                {
                    availableSpots++; //increase the available spots by 1
                    available.Add(t);
                }
            }

            if (availableSpots == 0)
            {
                MessageBox.Show("No available places to spawn people");
                return;
            }
                
            if (numberToSpawn > availableSpots)
            {
                if (availableSpots == 1)
                {
                    numberToSpawn = 1;
                }
                else
                {
                    numberToSpawn = availableSpots / 2;
                }

                MessageBox.Show("Not enough space for all of the people. Available spots: " + availableSpots + ". Spawning " + numberToSpawn + " people.");
            }

            for(int i = 0; i< numberToSpawn; i++)
            {
                position = rand.Next(0, available.Count);

                if (!available[position].personOnTile && !available[position].onFire) //check again in case a person moves or a fire spreads meanwhile
                {
                    Person p = SpawnNewPerson(available[position]); //spawn a new person on the random tile

                    p.SomeoneDies += statistics.PersonDies;
                    p.SomeoneEscapes += statistics.PersonEscapes;
                    p.RedrawHandler += this.RedrawExtinguisher;

                    if (p.Personality == PERSONALITY.HERO_OF_MANKIND)
                    {
                        brush = Properties.Resources.johnny;
                    }
                    else if (p.Personality == PERSONALITY.PUSSY)
                    {
                        brush = Properties.Resources.courage;
                    }
                    else if (p.Personality == PERSONALITY.SELFISH_PRICK)
                    {
                        brush = Properties.Resources.simpson;
                    }
                    else
                    {
                        brush = Properties.Resources.shaggy;
                    }
                    
                    DrawTileNoOverwrite(available[position], brush);

                    this.people.Add(p);

                }
                available.RemoveAt(position); //remove this tile from the available ones
            }
        }

        private void BtnSaveCurrentSituation_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = Grid.AddOutlineToBitmap(createdMap);
                bmp.Save(dialog.FileName + ".png", ImageFormat.Png);
                bitmapFileName = dialog.FileName;
            }
        }
    }
}
