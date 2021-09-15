using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Floor
    {
        public delegate void FireDrawHandler(Tile t, Bitmap b);
        public event FireDrawHandler DrawFireEvent;

        public List<Room> rooms;
        private int SpeedTick = 1;

        public Floor(List<Room> r)
        {
            rooms = r;
            foreach(Room roms in rooms)
            {
                roms.FireSpread += DrawFireFromSpread;
                roms.RoomFlashover += DoorColapse;
            }
        }

        private void DoorColapse(List<Tile> doors, List<Fire> doorFires, int roomNumber)
        {
            foreach(Room r in rooms)
            {
                if(r.roomNo == roomNumber)
                {
                    foreach (Tile door in r.doors)
                    {
                        foreach (Tile roomDoor in doors)
                        {
                            if (roomDoor == door)
                            {
                                foreach(Room room in rooms)
                                {
                                    room.SpreadFireFromFlashover(doorFires);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawFireFromSpread(Tile t)
        {
            DrawFireEvent(t, Properties.Resources.FireTile);
        }

        public Room FindRoom(Tile t)
        {
            foreach (Room r in rooms)
            {
                foreach (Tile til in r.tiles)
                {
                    t.AddNeighbours();
                    foreach(Tile side in t.neighbours)
                    {
                        if (Room.CheckDoors(side))
                        {
                            return r;
                        }
                    }
                    if (t == til)
                    {
                        return r;
                    }
                }
            }
            return null;
        }

        public List<Room> GetAdjacentRooms(Room r)
        {
            List<Room> adjRooms = new List<Room>();

            foreach(Room room in rooms)
            {
                foreach(Tile door in room.doors)
                {
                    foreach(Tile roomDoor in r.doors)
                    {
                        if(door == roomDoor && room.roomNo != r.roomNo)
                        {
                            adjRooms.Add(room);
                        }
                    }
                }
            }
            return adjRooms;
        }

        private bool HasFire()
        {
            foreach(Room r in rooms)
            {
                if(r.fires.Count > 0)
                {
                    foreach(Fire f in r.fires)
                    {
                        if(!f.isExtinguished)
                            return true;
                    }
                }
            }
            return false;
        }

        public void RandomFire()
        {
            Random Rand = new Random();
            int room = Rand.Next(0, rooms.Count);
            int fireTile = Rand.Next(0, rooms[room].tiles.Count);
            rooms[room].AddFire(new Fire(rooms[room].tiles[fireTile]));
            DrawFireFromSpread(rooms[room].tiles[fireTile]);
        }

        public void OnTick(Object sender, EventArgs e)
        {
            if (HasFire())
            {
                if (SpeedTick == 1 || SpeedTick == 2)
                {
                    foreach (Room r in rooms)
                    {
                        if (r.fireSpeed == 3)
                        {
                            r.SpreadFire();
                        }
                        foreach (AirFlow a in r.AirFlows)
                        {
                            a.CreateCurrent();
                            a.IncreaseRoomOxygen();
                        }
                    }
                    SpeedTick = 2;
                }
                if (SpeedTick == 2)
                {
                    foreach (Room r in rooms)
                    {
                        if (r.fireSpeed == 6)
                        {
                            r.SpreadFire();
                        }
                    }
                    SpeedTick = 3;
                }
                if (SpeedTick == 3)
                {
                    foreach (Room r in rooms)
                    {
                        if (r.fireSpeed == 9)
                        {
                            r.SpreadFire();
                        }
                    }
                    SpeedTick = 1;
                }
                foreach (Room r in rooms)
                {
                    r.IncreaseTemp(1 * r.fires.Count());
                    r.DecreaseFuel();
                }
            }
        }

        public void ClearAll()
        {
            foreach (Room roms in rooms)
            {
                roms.FireSpread -= DrawFireFromSpread;
                roms.ClearAll();
            }
            rooms = null;
        }
    }
}
