using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireWillKillUsAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll.Tests
{
    [TestClass()]
    public class ExtinguisherTests
    {
        [TestMethod()]
        public void ExtinguisherTest()
        {
            Extinguisher ex = new Extinguisher();
            Assert.IsNotNull(ex.Fuel);
        }

        [TestMethod()]
        public void ExtinguishTest()
        {
            Tile huan = new Tile(null, 0, null, null, null, null, System.Drawing.Color.White, 0, 0);
            Extinguisher ex = new Extinguisher();
            Fire f = new Fire(huan);
            bool noMalfunction;
            noMalfunction = ex.Extinguish(f); //if there is a malfunction, it will return false

            if (noMalfunction)
            {
                Assert.IsTrue(ex.Extinguish(f)); //if there is no malfunction it should return true
            }
            else
            {
                Assert.IsFalse(ex.Extinguish(f)); //else it should return false
            }
        }
    }
}