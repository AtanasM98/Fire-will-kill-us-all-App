using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FireWillKillUsAll
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UnityService1" in both code and config file together.
    
    public class UnityService1 : IUnityService1
    {
      

        public UpdatesFlush FlushUpdates()
        {
            return Buffer.Flush();
        }

        public string[] GetMap()
        {
            return Dump.bitmapArray;
        }
    }
}
