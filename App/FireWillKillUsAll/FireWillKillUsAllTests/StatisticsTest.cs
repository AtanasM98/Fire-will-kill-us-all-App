using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireWillKillUsAll;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FireWillKillUsAllTests
{
    [TestClass()]
    public class StatisticsTest
    {
        [TestMethod()]
        public void TestDeadCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseDeadcount();
            Assert.AreEqual(1, sm.deadCount);
        }
        [TestMethod()]
        public void TestEscapeCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseEscapeCount();
            Assert.AreEqual(1, sm.escapedCount);
        }
        [TestMethod()]
        public void TestSpawnCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseSpawnCount();
            Assert.AreEqual(1, sm.spawnCount);
        }
        [TestMethod()]
        public void TestFireSpawnCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseFireSpawnCount();
            Assert.AreEqual(1, sm.fireSpawnCount);
        }
        [TestMethod()]
        public void TestFireSpreadCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseFireSpreadCount();
            Assert.AreEqual(1, sm.fireSpreadCount);
        }
        [TestMethod()]
        public void TestFireExtinguishCount()
        {
            StatisticsManager sm = new StatisticsManager();
            sm.IncreaseFireExtinguisedCount();
            Assert.AreEqual(1, sm.fireExtinguisedCount);
        }
    }
}
