using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    class AirFlow
    {
        public delegate void AirFlowHandler(List<Tile> tiles);
        public event AirFlowHandler AirFlowCreated; 
        List<Tile> tiles;
        Room room;
        Tile startPos;
        Tile endPos;
        Tile currentPos;
        double currentSpeed;
        Pathfinding path;

        public List<Tile> Tiles
        {
            get { return tiles; }
        }

        public AirFlow(Tile start, Tile end, Room rom)
        {
            room = rom;
            startPos = start;
            endPos = end;
            currentPos = startPos;
            path = new Pathfinding(currentPos, endPos);
            tiles = new List<Tile>();
        }

        public void CreateCurrent()
        {
            tiles = path.Whoosh();
            tiles.RemoveAt(0);
            AirFlowCreated?.Invoke(tiles);
            currentSpeed = tiles.Count * 1.5;
        }

        public void IncreaseRoomOxygen()
        {
            room.IncreaseFuel(currentSpeed);
        }
    }
}
