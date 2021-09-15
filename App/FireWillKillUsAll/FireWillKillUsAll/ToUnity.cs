using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    [DataContract]
    public class UpdatesFlush
    {/*
        public ToUnity(string test, MyColor[] bitmapArray, int x, int y)
        {
            Test = test;
            this.bitmapArray = bitmapArray;
            X = x;
            Y = y;
        }
        */

        public UpdatesFlush(UnityPath[] movements, Coordinates[] fireSpreads,int[] dP)
        {
            this.movements = movements;
            this.fireSpreads = fireSpreads;
            this.deadPeople = dP;
        }
        [DataMember]
        public UnityPath[] movements { get; set; }

        [DataMember]
        public Coordinates[] fireSpreads { get; set; }
        [DataMember]
        public int[] deadPeople { get; set; }
    }
    [DataContract]
    public class UnityPath
    {
       
        public UnityPath(int personId, Coordinates[] m)
        {
            PersonId = personId;
            this.moves = m;
        }
        [DataMember]
        public int PersonId{get;set; }
        [DataMember]
        public Coordinates[] moves { get; set; }
        
    }
    [DataContract]
    public class Coordinates
    {
       
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
    }
   
}
