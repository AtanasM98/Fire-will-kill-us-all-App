using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsForm
{
    class StatisticsManager
    {
        private int deadCount;
        private int escapedCount;


        public int PersonDies()
        {
            deadCount++;
            return deadCount;
        }

        public int PersonEscapes()
        {
            escapedCount++;
            return escapedCount;
        }
    }
}
