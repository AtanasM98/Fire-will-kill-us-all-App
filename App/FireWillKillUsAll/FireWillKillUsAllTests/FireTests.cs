using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace FireWillKillUsAll.Tests
{
    [TestClass()]
    public class FireTests
    {

        [TestMethod]
        public void ExtinguishTest()
        {
            Tile TestTile = new Tile(new Vector(0, 0), 40, null, null, null, null, Form1.nullColor, 1, 1);
            Fire TestFire = new Fire(TestTile);
            TestFire.Extinguish(2);
            Assert.IsFalse(TestTile.isExtinguished);
            TestFire.Extinguish(232);
            Assert.IsTrue(TestTile.isExtinguished);
        }
        [TestMethod]
        public void SpreadFireTest()
        {
            Tile Up = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 1);
            Tile Down = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 1);
            Tile Left = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 3);
            Tile Right = new Tile(new Vector(0, 0), 40, null, null, null, null, Color.FromArgb(255, 255, 255, 255), 1, 4);

            Tile TestTile = new Tile(new Vector(0, 0), 40, Up, Down, Left, Right, Form1.nullColor, 1, 1);
            Fire TestFire = new Fire(TestTile);


           
           Assert.AreEqual(Up, TestFire.SpreadFire(0));
           Assert.AreEqual(Right, TestFire.SpreadFire(1));
           Assert.AreEqual(Down, TestFire.SpreadFire(2));
           Assert.AreEqual(Left, TestFire.SpreadFire(3));



        }

    }
}