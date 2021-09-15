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
    [TestClass()]
    public class RoomTests
    {
        [TestMethod()]
        public void RoomTest()
        {
            Room r;
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            Assert.AreNotEqual(r, null);
        }

        [TestMethod()]
        public void AddFireTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);
            Fire f = new Fire(tiles[1]);
            r.AddFire(f);


            Assert.AreEqual(r.fires.Count, 1);
        }

        [TestMethod()]
        public void MaxFuelTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            r.IncreaseFuel(100);

            Assert.AreEqual(r.fuel, r.MaxFuel);
        }

        [TestMethod()]
        public void IncreaseFuelTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            r.IncreaseFuel(-100);
            r.IncreaseFuel(50);

            Assert.AreNotEqual(r.fuel, r.MaxFuel);
        }

        [TestMethod()]
        public void DecreaseFuelTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            r.IncreaseFuel(-100);

            Assert.AreNotEqual(r.fuel, r.MaxFuel);
        }

        [TestMethod()]
        public void ExtinguishedFireTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);
            Fire f = new Fire(tiles[1]);
            r.AddFire(f);

            r.ExtinguishedFire(f.Tile);

            Assert.AreEqual(r.fires.Count, 0);
        }

        [TestMethod()]
        public void IncreaseTempTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            r.IncreaseTemp(30);


            Assert.AreNotEqual(r.temp, 22.5);
        }

        [TestMethod()]
        public void GetFireTest()
        {
            List<Tile> tiles = new List<Tile>();
            Room r;
            for (int i = 0; i < 16; i++)
            {
                tiles.Add(new Tile(new Vector(i, i), 1, null, null, null, null, Color.White, i, i));
            }
            r = new Room(tiles, 0);

            Fire f = new Fire(tiles[1]);
            Fire testFire = f;
            r.AddFire(f);
            f = new Fire(tiles[2]);
            r.AddFire(f);

            f = r.GetFire(tiles[1]);

            Assert.AreEqual(f, testFire);
        }

        [TestMethod()]
        public void SpreadFireTest()
        {
            Tile[,] tiles = new Tile[16, 16];
            Tile[,] grid;
            Room r;
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    tiles[i,y] = new Tile(new Vector(i, y), 1, null, null, null, null, Color.FromArgb(255, 255, 255,255), i, y);
                }
            }
            grid = Grid.CreateGridNeighbourReferences(tiles, 16, 16);

            List<Tile> roomTiles = new List<Tile>();

            foreach (Tile t in grid)
            {
                roomTiles.Add(t);
            }

            r = new Room(roomTiles, 0);

            Fire f = new Fire(roomTiles[25]);
            r.AddFire(f);
            f = new Fire(roomTiles[44]);
            r.AddFire(f);
            f = new Fire(roomTiles[24]);
            r.AddFire(f);

            r.SpreadFire();
            r.SpreadFire();
            r.SpreadFire();

            Assert.AreEqual(r.fires.Count, 7);
        }

        [TestMethod()]
        public void SpreadFireFromFlashoverTest()
        {
            Tile[,] tiles = new Tile[16, 16];
            Tile[,] grid;
            Room r;
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    tiles[i, y] = new Tile(new Vector(i, y), 1, null, null, null, null, Color.FromArgb(255, 255, 255, 255), i, y);
                }
            }
            grid = Grid.CreateGridNeighbourReferences(tiles, 16, 16);

            List<Tile> roomTiles = new List<Tile>();

            foreach (Tile t in grid)
            {
                roomTiles.Add(t);
            }

            r = new Room(roomTiles, 0);

            Fire f = new Fire(roomTiles[24]);
            List<Fire> doorFires = new List<Fire>();
            doorFires.Add(f);

            r.SpreadFireFromFlashover(doorFires);

            Assert.AreEqual(r.fires.Count, 1);
        }
    }
}