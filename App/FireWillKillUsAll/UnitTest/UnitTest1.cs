using FireWillKillUsAll;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace FireWillKillUsAll.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        [TestMethod()]
        public void CompareBitmapsFastTest()
        {
            Bitmap bm1 = new Bitmap(25, 45);

            bm1.SetPixel(23, 12, Color.GreenYellow);

            Bitmap bm2 = bm1;

            bool a = Grid.CompareBitmapsFast(bm1, bm2);

            if (!a)
            {
                Assert.Fail("Maps should be the same");
            }


            Assert.IsTrue(a, "Should be true if the maps are the same");

        }
        //----------------------------------------------------------------------------------
        [TestMethod()]
        public void FlushTest()
        {


            Buffer.AddDeadPerson(3);
            Buffer.AddDeadPerson(1);
            Buffer.AddDeadPerson(22);

            Buffer.AddFireSpread(3, 4);
            Buffer.AddFireSpread(6, 4);
            Buffer.AddFireSpread(2, 4);

            Buffer.AddMove(1, 3, 5);
            Buffer.AddMove(3, 3, 2);
            Buffer.AddMove(22, 5, 5);


            UpdatesFlush u = Buffer.Flush();

            if (u.deadPeople.Length != 3 || u.movements.Length != 3 || u.fireSpreads.Length != 3)
            {
                Assert.Fail("The object was not created currectly");
            }
            else if (u.deadPeople.Length == 3 || u.movements.Length == 3 || u.fireSpreads.Length == 3)
            {
                Assert.IsTrue(true, "Exactly the expected amount in each list");
            }
        }
        //----------------------------------------------------------------------------------
        [TestMethod()]
        public void SearchGridTest()
        {
            Vector v = new Vector(4, 5);

            Tile t = new Tile(v, 1, null, null, null, null, Color.Wheat, 5, 7);

            bool b = Grid.SearchGrid(new Vector(12, 13), out t);


            Assert.IsFalse(b, "Coordinates should be out of boundS");

        }

        [TestMethod()]
        public void GetBitmapOfActiveBrushFromColorTest()
        {
            Bitmap b = Grid.GetBitmapOfActiveBrushFromColor(Color.Tomato);

            bool a;
            if (b.Height == 1)
            {
                a = false;
            }
            else
            {
                a = true;
            }
            Assert.IsFalse(a, "There is no brush with Tomato Color, so it should return a 1,1 bitmap");
        }

        [TestMethod()]
        public void MarkOutsideBitmapTest()
        {
            Bitmap bit = new Bitmap(4, 3);

            

            Color w = Color.FromArgb(255, 255, 255, 255);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    bit.SetPixel(x, y, w);
                }
            }
            Bitmap b =Grid.MarkOutsideBitmap(bit, 3, 2);

            Color c = b.GetPixel(3, 2); //bottom right corner

            Assert.AreEqual(c, Form1.outsideColor);// all corners should allways be the outside color

            
        }
    }
}

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
