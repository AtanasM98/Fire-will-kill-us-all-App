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
    public class DijkstraTests
    {
        [TestMethod()]
        public void DijkstraTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0);
            List<Tile> grid = new List<Tile>();
            grid.Add(huan);
            Dijkstra d = new Dijkstra(huan, grid);
            Assert.IsInstanceOfType(d, typeof(Dijkstra));
        }

        [TestMethod()]
        public void CalculatePathTest()
        {
            #region INITIAL SETUP
            Tile huan = new Tile(null, 0, null, null, null, null, Color.White, 0, 0); huan.onFire = false; huan.walkable = true; 
            Tile jose = new Tile(null, 0, null, null, huan, null, Color.Black, 0, 1); jose.onFire = false; jose.walkable = true; 
            Tile antoan = new Tile(null, 0, null, huan, null, null, Color.Red, 1, 0); antoan.onFire = false; antoan.walkable = true;
            Tile esmeraldo = new Tile(null, 0, null, jose, antoan, null, Color.Pink, 1, 1); esmeraldo.onFire = true; esmeraldo.walkable = true;
            List<Tile> grid = new List<Tile>();

            huan.right = jose;              //Pattern is like
            jose.down = esmeraldo;          //  start - X  X
            antoan.left = esmeraldo;        //          X  X - end

            grid.Add(huan);
            grid.Add(jose);
            grid.Add(antoan);
            grid.Add(esmeraldo);

            foreach(Tile t in grid)
            {
                t.AddNeighbours();
            }
            #endregion

            List<Tile> finalPath;
            Dijkstra d = new Dijkstra(huan, grid);
            finalPath = d.CalculatePath(); //the path from the END to START, including the onfire tile

            Assert.AreEqual(3, finalPath.Count);
            Assert.AreEqual(finalPath[0], esmeraldo);
            Assert.AreEqual(finalPath[2], huan);
            Assert.IsTrue(esmeraldo.onFire);
        }
    }
}