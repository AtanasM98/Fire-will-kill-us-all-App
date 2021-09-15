using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Room
    {
        public delegate void FireSpreadHandler(Tile t);
        public event FireSpreadHandler FireSpread;
        public delegate void FlashoverHandler(List<Tile> t, List<Fire> f, int roomNumber);
        public event FlashoverHandler RoomFlashover;
        public event FireSpreadHandler Extinguished;
        public double temp;
        private double tempFlash = 300;
        public List<Tile> tiles;
        private double maxFuel;
        public List<Fire> fires;
        Random rand = new Random();
        //Fuel from 1-100
        public double fuel;
        public List<Tile> doors;
        public List<Tile> windows;
        public List<Tile> vents;
        //Faster - 3, Slower 6 depending on the fire count, 9 - when fire is just starting to build up
        public int fireSpeed;
        public int roomNo;
        //Check if a flashover has already happened
        private bool hasFlashover = false;
        public Statistics statistics;

        //List of tiles in specific directions to certain fire - works with GetWhereMostAir() method
        private List<Tile> upTiles;
        private List<Tile> leftTiles;
        private List<Tile> downTiles;
        private List<Tile> rightTiles;

        //List of tiles that the fire logic has set for spreading
        private List<Tile> fireQueue;

        private List<AirFlow> airFlows;

        public double MaxFuel { get => maxFuel; }
        internal List<AirFlow> AirFlows { get => airFlows; set => airFlows = value; }

        //event for statistics
        public event FireSpreadCountHandler FireSpreadCount;
        
        public Room(List<Tile> t, int roomNum)
        {
            roomNo = roomNum;
            fireSpeed = 6;
            fires = new List<Fire>();
            temp = 22.5;
            tiles = t;
            maxFuel = tiles.Count() * 5;
            fuel = maxFuel;

            doors = new List<Tile>();
            doors = FindDoors();
            //Currently always doors are open
            foreach(Tile tile in doors)
            {
                tile.DoorOpen = true;
            }
            windows = new List<Tile>();
            windows = FindWindows();
            vents = new List<Tile>();
            vents = FindVents();
            AirFlows = new List<AirFlow>();
            
            upTiles = new List<Tile>();
            leftTiles = new List<Tile>();
            downTiles = new List<Tile>();
            rightTiles = new List<Tile>();

            fireQueue = new List<Tile>();

            CreateAirFlow();
        }

        public void AddFire(Fire f)
        {
            if(fuel > 0)
            {
                IncreaseTemp(1.05 * fires.Count());
                fires.Add(f);

                Buffer.AddFireSpread(f.Tile.positionX, f.Tile.positionY);

                //invoke firespreadcount event for statistics
                if (FireSpreadCount != null)
                {
                    FireSpreadCount.Invoke();
                }

                f.SomeoneUsedExtinguisher += statistics.PersonUsesExtinguisher;

                
            }
            if(fires.Count() > tiles.Count() / 2)
            {
                fireSpeed = 3;
            }
            if (fires.Count > 3 && fires.Count < tiles.Count() / 2)
            {
                fireSpeed = 6;
            }
            if (fires.Count <= 3)
            {
                fireSpeed = 9;
            }
            f.Extinguished += ExtinguishedFire;
            
        }

        public void IncreaseFuel(double value)
        {
            if (fuel + value <= maxFuel)
                fuel += value;
            else
                fuel = maxFuel;
        }

        //Value changes for different materials
        public void DecreaseFuel()
        {
            if (fuel > 0)
                fuel -= 1.5 * fires.Count();
            else
            {
                fuel = 0;
                if(fires.Count() > 0)
                {
                    int r = rand.Next(0, fires.Count());
                    while (fires[r].isExtinguished)
                    {
                        r = rand.Next(0, fires.Count());
                    }
                    fires[r].Extinguish(25);
                }
            }
        }

        public void ExtinguishedFire(Tile t)
        {
            for(int i = 0; i < fires.Count; i++)
            {
                if(fires[i].Tile == t)
                {
                    IncreaseTemp(fires[i].Intensity);
                    fires.RemoveAt(i);
                }
            }
            Extinguished?.Invoke(t);
        }

        public void IncreaseTemp(double value)
        {
            if(temp <= 350)
            {
                temp += value/2;
                if(temp >= tempFlash && !hasFlashover)
                {
                    Flashover();
                }
                if(temp > 100)
                {
                    KillPeopleInRoom();
                }
            }
        }

        private void KillPeopleInRoom()
        {
            foreach(Tile t in tiles)
            {
                if (t.personOnTile)
                {
                    t.GetPerson().Die();
                }
            }
        }

        private void Flashover()
        {
            List<Tile> tempTiles = new List<Tile>();
            List<Fire> doorFires = new List<Fire>();
            foreach (Tile t in tiles)
            {
                if (!t.onFire && !t.isExtinguished)
                    tempTiles.Add(t);
            }
            foreach(Tile t in tempTiles)
            {
                AddFire(new Fire(t));
                FireSpread(t);
            }
            foreach(Tile door in doors)
            {
                Fire f = SetDoorOnFire(door);
                doorFires.Add(f);
            }
            hasFlashover = true;
            RoomFlashover?.Invoke(doors, doorFires, roomNo);
        }

        private Fire SetDoorOnFire(Tile door)
        {
            Fire f = new Fire(door);
            AddFire(f);
            FireSpread(door);
            return f;
        }

        public Fire GetFire(Tile t)
        {
            foreach(Fire f in fires)
            {
                if(f.Tile == t && !f.isExtinguished)
                {
                    return f;
                }
            }
            return null;
        }

        public static bool CheckDoors(Tile t)
        {
            //Finding door with colors from bitmap
            if (t.colorInBitmap == Color.FromArgb(255, 0, 255, 255) || t.colorInBitmap == Color.FromArgb(255, 0, 0, 255) ||
                t.colorInBitmap == Color.FromArgb(255, 255, 100, 255) || t.colorInBitmap == Color.FromArgb(255, 100, 0, 255) ||
                t.colorInBitmap == Color.FromArgb(255, 1, 255, 255) || t.colorInBitmap == Color.FromArgb(255, 1, 0, 255) ||
                t.colorInBitmap == Color.FromArgb(255, 255, 101, 255) || t.colorInBitmap == Color.FromArgb(255, 100, 1, 255))
            {
                return true;
            }
            return false;
        }

        private List<Tile> FindDoors()
        {
            List<Tile> doors = new List<Tile>();
            foreach(Tile t in tiles)
            {
                if (t.up != null)
                {
                    //Finding door with colors from bitmap
                    if (CheckDoors(t.up))
                    {
                        doors.Add(t.up);
                    }
                }
                if (t.left != null)
                {
                    //Finding door with colors from bitmap
                    if (CheckDoors(t.left))
                    {
                        doors.Add(t.left);
                    }
                }
                if (t.down != null)
                {
                    //Finding door with colors from bitmap
                    if (CheckDoors(t.down))
                    {
                        doors.Add(t.down);
                    }
                }
                if (t.right != null)
                {
                    //Finding door with colors from bitmap
                    if (CheckDoors(t.right))
                    {
                        doors.Add(t.right);
                    }
                }
            }
            foreach(Tile d in doors)
            {
                d.hasDoor = true;
            }
            return doors;
        }

        private List<Tile> FindWindows()
        {
            List<Tile> windows = new List<Tile>();
            foreach (Tile t in tiles)
            {
                if (t.up != null)
                {
                    //Finding door with colors from bitmap
                    if (t.up.colorInBitmap == Color.FromArgb(160, 255, 150, 0) || t.up.colorInBitmap == Color.FromArgb(160, 255, 170, 0))
                    {
                        windows.Add(t.up);
                    }
                }
                if (t.left != null)
                {
                    //Finding door with colors from bitmap
                    if (t.left.colorInBitmap == Color.FromArgb(160, 255, 150, 0) || t.left.colorInBitmap == Color.FromArgb(160, 255, 170, 0))
                    {
                        windows.Add(t.left);
                    }
                }
                if (t.down != null)
                {
                    //Finding door with colors from bitmap
                    if (t.down.colorInBitmap == Color.FromArgb(160, 255, 150, 0) || t.down.colorInBitmap == Color.FromArgb(160, 255, 170, 0))
                    {
                        windows.Add(t.down);
                    }
                }
                if (t.right != null)
                {
                    //Finding door with colors from bitmap
                    if (t.right.colorInBitmap == Color.FromArgb(160, 255, 150, 0) || t.right.colorInBitmap == Color.FromArgb(160, 255, 170, 0))
                    {
                        windows.Add(t.right);
                    }
                }
            }
            return windows;
        }

        private List<Tile> FindVents()
        {
            List<Tile> temp = new List<Tile>();
            foreach(Tile t in tiles)
            {
                if (t.hasVent)
                {
                    temp.Add(t);
                }
            }
            return temp;
        }

        private void CreateAirFlow()
        {
            foreach(Tile w in windows)
            {
                foreach (Tile v in vents)
                    airFlows.Add(new AirFlow(w, v, this));
            }
        }

        public void SpreadFire()
        {
            List<Fire> tempFires = GetOutermostFires();
            if (!hasFlashover && tempFires.Count() > 0)
            {
                #region Door fire spread
                foreach (Fire f in tempFires)
                {
                    if(f.Tile.left != null)
                    {
                        if (f.Tile.left.hasDoor && f.Tile.left.DoorOpen)
                        {
                            fireQueue.Add(f.Tile.left);
                        }
                    }
                    if (f.Tile.up != null)
                    {
                        if (f.Tile.up.hasDoor && f.Tile.up.DoorOpen)
                        {
                            fireQueue.Add(f.Tile.up);
                        }
                    }
                    if (f.Tile.right != null)
                    {
                        if (f.Tile.right.hasDoor && f.Tile.right.DoorOpen)
                        {
                            fireQueue.Add(f.Tile.right);
                        }
                    }
                    if (f.Tile.down != null)
                    {
                        if (f.Tile.down.hasDoor && f.Tile.down.DoorOpen)
                        {
                            fireQueue.Add(f.Tile.down);
                        }
                    }
                }
                #endregion
                #region Main Fire logic
                int r = rand.Next(0, tempFires.Count());
                int dir = 0;
                int max = 0;
                for(int i = 0; i < 4; i++)
                {
                    GetWhereMostAir(tempFires[r].Tile, i);
                }
                if (upTiles.Count > max)
                {
                    max = upTiles.Count;
                    dir = 0;
                }
                if (rightTiles.Count > max)
                {
                    max = rightTiles.Count;
                    dir = 1;
                }
                if (downTiles.Count > max)
                {
                    max = downTiles.Count;
                    dir = 2;
                }
                if (leftTiles.Count > max)
                {
                    max = leftTiles.Count;
                    dir = 3;
                }

                //int fireDir = GetFireDirection(tempFires[r]);
                Tile temp = tempFires[r].SpreadFire(dir);
                if (temp != null && !temp.onFire && !temp.isExtinguished)
                {
                    if(!fireQueue.Contains(temp))
                        fireQueue.Add(temp);
                    Fire tempFire = new Fire(fireQueue[0]);
                    if (fireQueue[0].hasDoor)
                    {
                        List<Tile> door = new List<Tile>();
                        door.Add(fireQueue[0]);
                        List<Fire> fires = new List<Fire>();
                        fires.Add(tempFire);
                        RoomFlashover(door, fires, roomNo);
                    }
                    AddFire(tempFire);
                    FireSpread?.Invoke(fireQueue[0]);
                    fireQueue.RemoveAt(0);
                }
                #endregion
                #region Air Current Fire Spread
                tempFires = GetOutermostFires();
                //Checks every fire against every tile of the air currents in the rooms
                //If a fire is on an airflow current tile it will spread faster to other air current tiles
                foreach (Fire f in tempFires)
                {
                    foreach (AirFlow a in airFlows)
                    {
                        foreach (Tile t in a.Tiles)
                        {
                            if (f.Tile == t)
                            {
                                foreach (Tile otherT in a.Tiles)
                                {
                                    if (!otherT.onFire && !otherT.isExtinguished && otherT.walkable && otherT.neighbours.Contains(f.Tile))
                                    {
                                        fireQueue.Add(otherT);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Off fire logics
                tempFires = GetOutermostFires();
                if (tempFires.Count() > 0)
                {
                    r = rand.Next(0, tempFires.Count());
                    dir = 0;
                    dir = GetFireDirection(tempFires[r]);               //Throws indexOutOfRange Exception sometimes

                    //int fireDir = GetFireDirection(tempFires[r]);
                    temp = tempFires[r].SpreadFire(dir);
                    if (temp != null && !temp.onFire && !temp.isExtinguished)
                    {
                        if (!fireQueue.Contains(temp))
                            fireQueue.Add(temp);
                        if (fires.Count > 5)
                        {
                            AddFire(new Fire(fireQueue[0]));
                            FireSpread?.Invoke(fireQueue[0]);
                            fireQueue.RemoveAt(0);
                        }
                        if (fires.Count > 10 && fireQueue.Count > 2)
                        {
                            AddFire(new Fire(fireQueue[0]));
                            FireSpread?.Invoke(fireQueue[0]);
                            fireQueue.RemoveAt(0);
                        }
                    }
                }
                #endregion
            }
        }

        public void SpreadFireFromFlashover(List<Fire> doorFire)
        {
            if (!hasFlashover)
            {
                foreach(Fire f in doorFire)
                {
                    foreach(Tile door in doors)
                    {
                        if(f.Tile == door)
                        {
                            int fireDir = GetFireDirection(f);
                            Tile temp;
                            if (fireDir >= 0 && fireDir <= 3)
                            {
                                temp = f.SpreadFire(fireDir);
                                AddFire(new Fire(temp));
                                FireSpread?.Invoke(temp);
                                temp = f.SpreadFire(fireDir);
                                AddFire(new Fire(temp));
                                FireSpread?.Invoke(temp);
                            }
                        }
                    }
                }
            }
        }

        //Gets a list of fires that are not between 4 fires
        public List<Fire> GetOutermostFires()
        {
            List<Fire> tempFires = new List<Fire>();
            foreach(Fire f in fires)
            {
                if (!f.isExtinguished)
                {
                    if (CheckSide(f.Tile.up)
                        || CheckSide(f.Tile.down)
                        || CheckSide(f.Tile.left)
                        || CheckSide(f.Tile.right))
                    {
                        tempFires.Add(f);
                    }
                }
            }
            return tempFires;
        }

        private bool CheckSide(Tile t)
        {
            bool b = false;
            if(t == null)
            {
                b = false;
            }
            if (!t.onFire && t.colorInBitmap == Color.FromArgb(255, 255, 255, 255) || !t.onFire && t.colorInBitmap.A == 254)
            {
                b = true;
            }
            return b;
        }

        //Gets an int direction 0 - up, 1 - right, 2 - down, 3 - left
        //with which the fire will spread to without spreading in another fire
        public int GetFireDirection(Fire f)
        {
            int dir = -1;
            if (!f.isExtinguished)
            {
                Random rand = new Random();
                //This retarded thing here:
                //It gets a random direction with a preference to the direction that тhe random chooses:
                //If the random direction is obstructed it chooses the one before it clockwise
                switch (rand.Next(0, 4))
                {
                    case 0:
                        if (CheckSide(f.Tile.left))
                        {
                            dir = 3;
                        }
                        if (CheckSide(f.Tile.down))
                        {
                            dir = 2;
                        }
                        if (CheckSide(f.Tile.right))
                        {
                            dir = 1;
                        }
                        if (CheckSide(f.Tile.up))
                        {
                            dir = 0;
                        }
                        break;
                    case 1:
                        if (CheckSide(f.Tile.left))
                        {
                            dir = 3;
                        }
                        if (CheckSide(f.Tile.down))
                        {
                            dir = 2;
                        }
                        if (CheckSide(f.Tile.up))
                        {
                            dir = 0;
                        }
                        if (CheckSide(f.Tile.right))
                        {
                            dir = 1;
                        }
                        break;
                    case 2:
                        if (CheckSide(f.Tile.left))
                        {
                            dir = 3;
                        }
                        if (CheckSide(f.Tile.up))
                        {
                            dir = 0;
                        }
                        if (CheckSide(f.Tile.right))
                        {
                            dir = 1;
                        }
                        if (CheckSide(f.Tile.down))
                        {
                            dir = 2;
                        }
                        break;
                    case 3:
                        if (CheckSide(f.Tile.up))
                        {
                            dir = 0;
                        }
                        if (CheckSide(f.Tile.right))
                        {
                            dir = 1;
                        }
                        if (CheckSide(f.Tile.down))
                        {
                            dir = 2;
                        }
                        if (CheckSide(f.Tile.left))
                        {
                            dir = 3;
                        }
                        break;
                }
            }
            return dir;
        }

        private void GetWhereMostAir(Tile f, int dir)
        {
            if (f.up != null)
            {
                if (CheckSide(f.up) && dir == 0)
                {
                    upTiles.Add(f);
                    GetWhereMostAir(f.up, dir);
                }
            }
            if(f.right != null)
            {
                if (CheckSide(f.right) && dir == 1)
                {
                    rightTiles.Add(f);
                    GetWhereMostAir(f.right, dir);
                }
            }
            if (f.down != null)
            {
                if (CheckSide(f.down) && dir == 2)
                {
                    downTiles.Add(f);
                    GetWhereMostAir(f.down, dir);
                }
            }
            if (f.left != null)
            {
                if (CheckSide(f.left) && dir == 3)
                {
                    leftTiles.Add(f);
                    GetWhereMostAir(f.left, dir);
                }
            }
        }

        public void ClearAll()
        {
            foreach (Fire f in fires)
            {
                f.Tile.onFire = false;
                f.Extinguished -= ExtinguishedFire;
            }
            tiles = null;
            fires = null;
        }

        public void passOnStatisticForm(Statistics s)
        {
            statistics = s;
        }
    }
}
