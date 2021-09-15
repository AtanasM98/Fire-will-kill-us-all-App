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
    public class PathfindingTests
    {
        [TestMethod()]
        public void PathfindingTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.Black, 0, 0);
            Tile jose = new Tile(null, 0, null, null, null, null, Color.Wheat, 0, 0);
            Pathfinding path = null;
            path = new Pathfinding(huan, jose);
            Assert.IsNotNull(path);
            Assert.IsInstanceOfType(path, typeof(Pathfinding));
        }

        [TestMethod()]
        public void GetNextMoveTest()
        {
            #region INITIAL SETUP
            Tile huan = new Tile(null, 0, null, null, null, null, Color.Pink, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, null, null, Color.Red, 1, 0);
            Tile antoan = new Tile(null, 0, jose, null, null, null, Color.Blue, 2, 0);
            huan.down = jose;
            jose.down = antoan;
            Tile[,] grid = new Tile[3, 1];  //Pattern is like
            grid[0, 0] = huan;              //      X - start
            grid[1, 0] = jose;              //      X
            grid[2, 0] = antoan;            //      X - goal

            foreach(Tile t in grid)
            {
                t.walkable = true;
                t.onFire = false;
                t.AddNeighbours();
            }
            #endregion
            Pathfinding p = new Pathfinding(huan, antoan);

            Tile next = p.GetNextMove();
            Assert.AreEqual(jose, next);
            p = new Pathfinding(next, antoan);
            next = p.GetNextMove();
            Assert.AreEqual(antoan, next);
        }

        [TestMethod()]
        public void GetFullPathTest()
        {
            #region INITIAL SETUP
            List<Tile> list = new List<Tile>();
            Tile huan = new Tile(null, 0, null, null, null, null, Color.Pink, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, null, null, Color.Red, 1, 0);
            Tile antoan = new Tile(null, 0, jose, null, null, null, Color.Blue, 2, 0);
            huan.down = jose;
            jose.down = antoan;
            Tile[,] grid = new Tile[3, 1];  //Pattern is like
            grid[0, 0] = huan;              //      X - start
            grid[1, 0] = jose;              //      X
            grid[2, 0] = antoan;            //      X - goal

            foreach (Tile t in grid)
            {
                t.walkable = true;
                t.onFire = false;
                t.AddNeighbours();
            }
            #endregion

            Pathfinding path = new Pathfinding(huan, antoan);
            list = path.GetFullPath(); //list is in reverse (from END to START)
            Assert.AreEqual(list[0], antoan);
            Assert.AreEqual(list[1], jose);
            Assert.AreEqual(list[2], huan);
        }

        [TestMethod()]
        public void CalculatePathTest()
        {
            #region INITIAL SETUP
            List<Tile> fireList = new List<Tile>();
            List<Tile> noFireList = new List<Tile>();
            Tile huan = new Tile(null, 0, null, null, null, null, Color.Pink, 0, 1);
            Tile jose = new Tile(null, 0, huan, null, null, null, Color.Red, 1, 1);
            Tile antoan = new Tile(null, 0, jose, null, null, null, Color.Blue, 2, 1);
            Tile esmeraldo = new Tile(null, 0, null, null, null, huan, Color.Pink, 0, 0);
            Tile javier = new Tile(null, 0, esmeraldo, null, null, jose, Color.Red, 1, 0);
            Tile julio = new Tile(null, 0, javier, null, null, antoan, Color.Blue, 2, 0);
            huan.down = jose;
            jose.down = antoan;
            esmeraldo.down = javier;
            javier.down = julio;
            huan.left = esmeraldo;
            jose.left = javier;
            antoan.left = julio;
            Tile[,] grid = new Tile[3, 2];  //Pattern is like
            grid[0, 1] = huan;              //    X  X - start
            grid[1, 1] = jose;              //    X  X - on fire (Whoosh checks through the fire)
            grid[2, 1] = antoan;            //    X  X - goal
            grid[0, 0] = esmeraldo;
            grid[1, 0] = javier;
            grid[2, 0] = julio;

            foreach (Tile t in grid)
            {
                t.walkable = true;
                t.onFire = false;
                t.AddNeighbours();
            }

            jose.onFire = true;
            #endregion

            Pathfinding p = new Pathfinding(huan, antoan);

            fireList = p.CalculatePath(false); //calculate without checking if it is on fire, list is reverse (END - START)
            Assert.AreEqual(3, fireList.Count);
            Assert.AreEqual(fireList[0], antoan);
            Assert.AreEqual(fireList[1], jose);
            Assert.AreEqual(fireList[2], huan);

            p = new Pathfinding(huan, antoan);
            noFireList = p.CalculatePath(true); //calculate with check if it is on fire
            Assert.AreEqual(5,noFireList.Count);
            Assert.AreEqual(noFireList[0], antoan);
            Assert.AreEqual(noFireList[1], julio);
            Assert.AreEqual(noFireList[2], javier);
            Assert.AreEqual(noFireList[3], esmeraldo);
            Assert.AreEqual(noFireList[4], huan);
        }

        [TestMethod()]
        public void WhooshTest()
        {
            #region INITIAL SETUP
            List<Tile> list = new List<Tile>();
            Tile huan = new Tile(null, 0, null, null, null, null, Color.Pink, 0, 0);
            Tile jose = new Tile(null, 0, huan, null, null, null, Color.Red, 1, 0);
            Tile antoan = new Tile(null, 0, jose, null, null, null, Color.Blue, 2, 0);
            huan.down = jose;
            jose.down = antoan;
            Tile[,] grid = new Tile[3, 1];  //Pattern is like
            grid[0, 0] = huan;              //      X - start
            grid[1, 0] = jose;              //      X - on fire (Whoosh checks through the fire)
            grid[2, 0] = antoan;            //      X - goal

            foreach (Tile t in grid)
            {
                t.walkable = true;
                t.onFire = false;
                t.AddNeighbours();
            }

            jose.onFire = true;
            #endregion

            Pathfinding p = new Pathfinding(huan, antoan);
            list = p.Whoosh(); //list is from START to END
            Assert.AreEqual(list[0], huan);
            Assert.AreEqual(list[1], jose);
            Assert.AreEqual(list[2], antoan);
            Assert.IsTrue(jose.onFire);
        }
    }
}