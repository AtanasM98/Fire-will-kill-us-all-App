using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class StatisticsManager
    {
        public int deadCount { get; set; }
        public int escapedCount { get; set; }
        public int spawnCount { get; set; }
        public int fireSpawnCount { get; set; }
        public int fireSpreadCount { get; set; }
        public int fireExtinguisedCount { get; set; }

        public StatisticsManager()
        {
            deadCount = -1;
            escapedCount = 0;
            spawnCount = 0;
            fireSpreadCount = 0;
            fireSpreadCount = 0;
            fireExtinguisedCount = 0;
        }

        public int IncreaseDeadcount()
        {
            return ++deadCount;
        }
        public int IncreaseEscapeCount()
        {
            return ++escapedCount;
        }
        public int IncreaseSpawnCount()
        {
            return ++spawnCount;
        }
        public int IncreaseFireSpawnCount()
        {
            return ++fireSpawnCount;
        }
        public int IncreaseFireSpreadCount()
        {
            return ++fireSpreadCount;
        }
        public int IncreaseFireExtinguisedCount()
        {
            return ++fireExtinguisedCount;
        }

        public void simulationFinished()
        {
            //Send object
            clearData();
        }

        public void clearData()
        {
            deadCount = 0;
            escapedCount = 0;
            spawnCount = 0;
            fireSpawnCount = 0;
            fireSpreadCount = 0;
            fireExtinguisedCount = 0;
        }
    }
}
