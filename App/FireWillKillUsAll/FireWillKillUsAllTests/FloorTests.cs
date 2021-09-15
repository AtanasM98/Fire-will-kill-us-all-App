using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireWillKillUsAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FireWillKillUsAll.Tests
{
    [TestClass]
    public class FloorTests
    {     

        [TestMethod]
        public void GetAdjacentRoomTest()
        {
            Tile t1 = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 1);
            List<Tile> tiles1 = new List<Tile>{t1};
           
            List<Room> rooms = new List<Room> { new Room(tiles1, 0), new Room(tiles1, 1) };
            rooms[0].doors.Add(t1);
            rooms[1].doors.Add(t1);
            Floor floor = new Floor(rooms);

            Assert.AreEqual(rooms[1], floor.GetAdjacentRooms(rooms[0])[0]);
            Assert.AreEqual(rooms[0], floor.GetAdjacentRooms(rooms[1])[0]);
        }
        
        [TestMethod]
        public void TestFindRoom()
        {
            Tile t1 = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 1);
            Tile t2 = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 254), 1, 1);
            List<Tile> tiles1 = new List<Tile>{t1};
            List<Tile> tiles2 = new List<Tile>{t2};
            List<Room> rooms = new List<Room> { new Room(tiles1, 0), new Room(tiles2, 1) };
            Floor floor = new Floor(rooms);

            Assert.AreEqual(rooms[0],floor.FindRoom(t1));
            Assert.AreEqual(rooms[1], floor.FindRoom(t2));
            Assert.AreNotEqual(rooms[0], floor.FindRoom(t2));
            Assert.AreNotEqual(rooms[1], floor.FindRoom(t1));

        }

        [TestMethod]
        public void ClearAllTest()
        {
            Tile t = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 1);
            List<Tile> tiles = new List<Tile> { t};
           
            List<Room> rooms = new List<Room> { new Room(tiles, 0) };
            rooms[0].AddFire(new Fire(t));
            Room r = new Room(tiles, 1);
            Floor floor = new Floor(rooms);

            floor.ClearAll();

            Assert.IsNull(floor.rooms);
        }

    }
}