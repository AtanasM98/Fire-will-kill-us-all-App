using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Tile
    {
        private Vector startPos;
        private int size;
        public bool inUse;
        public bool outOfBound;
        public Tile up;
        public Tile down;
        public Tile left;
        public Tile right;
        public bool checkedInAlgorithm;
        public Color colorInBitmap;
        public bool onFire;
        public bool walkable;
        public bool personOnTile; //if there is a person currently on the tile
        public bool available; //used only in Dijkstra algorithm, feel free to ignore it (it means that no person is extingiushing it)
        public int positionX;
        public int positionY;
        public List<Tile> neighbours = new List<Tile>(); //used in pathfinding
        //Check if already had a fire on it, if true cant set on fire again.
        public bool isExtinguished;

        //---------------------------------------------------------------------------------
        //These vars are ONLY if there is a door on the tile - will use to close/open doors
        public bool hasDoor;
        public bool DoorOpen;
        //---------------------------------------------------------------------------------

        public bool hasVent;

        private Fire fire;
        private Person person;

        internal Vector StartPos { get => startPos; }
        public int Size { get => size; }

        public Tile(Vector start, int size,Tile u, Tile d, Tile l, Tile r, Color c, int posX, int posY)
        {
            this.startPos = start;
            this.size = size;
            inUse = false;
            outOfBound = false;
            up = u;
            down = d;
            left = l;
            right = r;
            colorInBitmap = c;
            walkable = false;
            available = true;
            this.positionX = posX;
            this.positionY = posY;

        }

        public Fire GetFire()
        {
            return fire;
        }

        public void SetFire(Fire f)
        {
            fire = f;
        }

        public Person GetPerson()
        {
            return person;
        }

        public void SetPerson(Person p)
        {
            person = p;
        }

        public void AddNeighbours()
        {
            this.neighbours.Clear(); //the method is called every timer tick, so clear the list, as eventually a tile could have infinite neighbours

            if (this.up != null)
                this.neighbours.Add(this.up);
            if (this.down != null)
                this.neighbours.Add(this.down);
            if (this.left != null)
                this.neighbours.Add(this.left);
            if (this.right != null)
                this.neighbours.Add(this.right);
        }
    }
}
