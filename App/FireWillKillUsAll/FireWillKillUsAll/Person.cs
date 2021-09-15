using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireWillKillUsAll
{
    public class Person
    { 
        //ID
        public int Id { get; set; }

        //personality variable
        private PERSONALITY personality;

        //get the personality for the form class
        //which will call the corresponding algorithm
        public PERSONALITY Personality { get { return personality; } }

        //percentage generator
        Random rand;

        //Event to redraw a wall when the extinguisher is removed
        public event RedrawExtinguisher RedrawHandler;

        //necessary tiles for start, end, previous and current location
        Tile startLocation;
        Tile currentLocation;
        Tile previousLocation;
        Tile endLocation;

        public Color exitDoorColor { get; private set; }

        //store the grid for the Dijkstra algorithm
        List<Tile> grid;

        //lists to keep track of all escapes and extinguishers
        List<Tile> fireEscapes;
        List<Tile> fireExtinguishers;
        List<Tile> sharedExtinguishers; //used to check wether another hero has taken the extinguisher before

        //list with the tiles which the person can see
        List<Tile> lineOfSight;

        //list with tiles of the path to the destination, without knowing if it is blocked
        List<Tile> blindPath;
        
        //properties used by the form to draw
        //Used when moving through doors - if true will leave door open else will close it
        private bool leaveDoorOpen;
        public bool LeaveDoorOpen { get { return leaveDoorOpen; } }

        public Tile CurrentLocation { get { return currentLocation; } }
        public Tile PreviousLocation { get { return previousLocation; } }
        public Tile EndLocation { get { return endLocation; } }

        //A* algorithm object
        Pathfinding A_STAR; //assigned in the SetDestination method, because of the different goals for each personality

        //Dijkstra algorithm object
        Dijkstra DIJKSTRA; //used after getting an extinguisher to find the nearest fire

        private Color nextTileColor; //store the color of the upcoming Tile
        private Color previousTileColor; //used to paint the same object as the one that was before the person moved to it

        public Color PreviousTileColor { get { return previousTileColor; } }

        //dead, saved and path blocked variables
        private bool isDead;
        private bool isSaved;
        private bool exitBlocked; //used to determine when the selfish runs for extinguisher
        private bool extinguisherBlocked; //used to determine when path to extinguisher is blocked for selfish and hero
        private bool selfishChanged; //only used for the selfish to change his end destination only once
        private bool heroChanged; //only used for the hero to change his behaviour to run for the exit

        public bool IsSaved { get { return isSaved; } }
        public bool IsDead { get { return isDead; } }

        //extinguisher object variable
        private Extinguisher extinguisher = null;

        //counter to make Shaggy run in circles
        //Note: only use it for the rare Shaggy easter egg
        private int ShaggyCounter = 0;

        //events for when someone dies or escapes
        public event DeadHandler SomeoneDies;
        public event EscapedHandler SomeoneEscapes;

        //Constructor
        public Person(Tile start, List<Tile> escapes, List<Tile> extinguishers, Tile[,] grid)
        {
            rand = new Random((int)DateTime.Now.Ticks);
            //assign a pesonality trait based on the generated percentage
            switch (rand.Next(0, 101))
            {
                //0 or 1 percent
                case int n when (n >= 0 && n < 2):
                    personality = PERSONALITY.SHAGGY_FROM_SCOOBY_DOO; //run randomly
                    break;

                //70-89 percent
                case int n when (n >= 70 && n < 90):
                    personality = PERSONALITY.SELFISH_PRICK; //run for exit, if blocked, run for extinguisher
                    break;

                //90-100 percent
                case int n when (n >= 90 && n <= 100):
                    personality = PERSONALITY.HERO_OF_MANKIND; //run for extinguisher
                    break;

                //2-69 percent
                default:
                    personality = PERSONALITY.PUSSY; //run for evacuation
                    break;
            }

            //Random check if going to leave doors open or not
            if(rand.Next(0, 100) < 40)
            {
                leaveDoorOpen = true;
            }
            else
            {
                leaveDoorOpen = false;
            }

            //set id
            this.Id = Grid.personIdCounter;
            Grid.personIdCounter++;
            //Initialize starting point and current location
            this.startLocation = this.currentLocation = this.previousLocation = start;

            //Initialize dead, saved and path blocked to false in beginning
            this.isDead = this.isSaved = exitBlocked = extinguisherBlocked = false;
            

            //the destination change is not yet made for the selfish prick and the hero
            this.selfishChanged = this.heroChanged = false;

            //keep track of the colour of the next Tile
            this.nextTileColor = start.colorInBitmap;

            //create the lists for exits and extinguishers and the grid
            fireEscapes = new List<Tile>();
            fireExtinguishers = new List<Tile>();
            this.grid = new List<Tile>();

            //set the shared extinguishers
            this.sharedExtinguishers = extinguishers;

            //set the available exits and extinguishers
            foreach(Tile t in escapes)
            {
                this.fireEscapes.Add(t);
            }

            foreach(Tile t in extinguishers)
            {
                this.fireExtinguishers.Add(t);
            }
            
            //set the end location
            this.SetInitialEndPoint(start);

            //set the line of sight
            this.lineOfSight = new List<Tile>();
            this.UpdateLineOfSight();
            this.blindPath = new List<Tile>();

            //store the grid for Dijkstra and another copy for local use
            foreach(Tile t in grid)
            {
                this.grid.Add(t);
            }

            currentLocation.personOnTile = true;
            
        }

        //Constructor with option to create a person with specified personality
        public Person(Tile start, List<Tile> escapes, List<Tile> extinguishers, Tile[,] grid, PERSONALITY personality)
        {
            rand = new Random((int)DateTime.Now.Ticks);

            this.personality = personality; //Assign a custom personality

            //Random check if going to leave doors open or not
            if (rand.Next(0, 100) < 40)
            {
                leaveDoorOpen = true;
            }
            else
            {
                leaveDoorOpen = false;
            }

            //set id
            this.Id = Grid.personIdCounter;
            Grid.personIdCounter++;
            //Initialize starting point and current location
            this.startLocation = this.currentLocation = this.previousLocation = start;

            //Initialize dead, saved and path blocked to false in beginning
            this.isDead = this.isSaved = exitBlocked = extinguisherBlocked = false;


            //the destination change is not yet made for the selfish prick and the hero
            this.selfishChanged = this.heroChanged = false;

            //keep track of the colour of the next Tile
            this.nextTileColor = start.colorInBitmap;

            //create the lists for exits and extinguishers and the grid
            fireEscapes = new List<Tile>();
            fireExtinguishers = new List<Tile>();
            this.grid = new List<Tile>();

            //set the shared extinguishers
            this.sharedExtinguishers = extinguishers;

            //set the available exits and extinguishers
            foreach (Tile t in escapes)
            {
                this.fireEscapes.Add(t);
            }

            foreach (Tile t in extinguishers)
            {
                this.fireExtinguishers.Add(t);
            }

            //set the end location
            this.SetInitialEndPoint(start);

            //set the line of sight
            this.lineOfSight = new List<Tile>();
            this.UpdateLineOfSight();
            this.blindPath = new List<Tile>();

            //store the grid for Dijkstra and another copy for local use
            foreach (Tile t in grid)
            {
                this.grid.Add(t);
            }

            currentLocation.personOnTile = true;
        }


        //Set the end destination for the person from the Form so that the personality is already assigned
        //When the destination changes (for the "selfish prick"), this method will create a new algorithm for the extinguisher as the end point
        public void SetDestination(Tile endPoint)
        {
            if (endPoint != null)
            {
                this.endLocation = endPoint;
                exitDoorColor = endLocation.colorInBitmap;
            }
            else
            {
                this.personality = PERSONALITY.SHAGGY_FROM_SCOOBY_DOO; //run in circles if no end goal
            }
        }



        //Methods for moving the person
        //If the tile is walkable and not on fire, then move
        //Current location will have person on tile set to true to avoid 2 people in 1 tile
        //After the move, the line of sight is updated
        public void MoveUp()
        {
            previousLocation = currentLocation;

            //if the up tile exists, is NOT on fire, is walkable AND there is NO person on it, then move
            if (currentLocation.up != null && !currentLocation.up.onFire && currentLocation.up.walkable && !currentLocation.up.personOnTile)
            {
                previousTileColor = nextTileColor;
                nextTileColor = currentLocation.up.colorInBitmap; //store previous and next colors for the form
                currentLocation = currentLocation.up;
                this.currentLocation.personOnTile = true;
                this.previousLocation.personOnTile = false;

                this.currentLocation.SetPerson(this);
                this.previousLocation.SetPerson(null);

                this.UpdateLineOfSight(); //update the new line of sight
               // Buffer.AddMove(Id, currentLocation.positionX, currentLocation.positionY); // to buffer for unity
                //MessageBox.Show("Move added" + $"{Buffer.movements.Count}");
            }
        }

        public void MoveDown()
        {
            previousLocation = currentLocation;

            //if the down tile exists, is NOT on fire, is walkable AND there is NO person on it, then move
            if (currentLocation.down != null && !currentLocation.down.onFire && currentLocation.down.walkable && !currentLocation.down.personOnTile)
            {
                previousTileColor = nextTileColor;
                nextTileColor = currentLocation.down.colorInBitmap; //store previous and next colors for the form
                currentLocation = currentLocation.down;
                this.currentLocation.personOnTile = true;
                this.previousLocation.personOnTile = false;

                this.currentLocation.SetPerson(this);
                this.previousLocation.SetPerson(null);
                this.UpdateLineOfSight(); //update the new line of sight
               // Buffer.AddMove(Id, currentLocation.positionX, currentLocation.positionY); // to buffer for unity
               // MessageBox.Show("Move added" + $"{Buffer.movements.Count}");
            }
        }

        public void MoveLeft()
        {
            previousLocation = currentLocation;

            //if the left tile exists, is NOT on fire, is walkable AND there is NO person on it, then move
            if (currentLocation.left != null && !currentLocation.left.onFire && currentLocation.left.walkable && !currentLocation.left.personOnTile)
            {
                previousTileColor = nextTileColor;
                nextTileColor = currentLocation.left.colorInBitmap; //store previous and next colors for the form
                currentLocation = currentLocation.left;
                this.currentLocation.personOnTile = true;
                this.previousLocation.personOnTile = false;

                this.currentLocation.SetPerson(this);
                this.previousLocation.SetPerson(null);
                this.UpdateLineOfSight(); //update the new line of sight
              //  Buffer.AddMove(Id, currentLocation.positionX, currentLocation.positionY); // to buffer for unity
               // MessageBox.Show("Move added" + $"{Buffer.movements.Count}");
            }
        }

        public void MoveRight()
        {
            previousLocation = currentLocation;

            //if right tile exists, is NOT on fire, is walkable AND there is NO person on it, then move
            if (currentLocation.right != null && !currentLocation.right.onFire && currentLocation.right.walkable && !currentLocation.right.personOnTile)
            {
                previousTileColor = nextTileColor;
                nextTileColor = currentLocation.right.colorInBitmap; //store previous and next colors for the form
                currentLocation = currentLocation.right;
                this.currentLocation.personOnTile = true;
                this.previousLocation.personOnTile = false;

                this.currentLocation.SetPerson(this);
                this.previousLocation.SetPerson(null);
                this.UpdateLineOfSight(); //update the new line of sight
               // Buffer.AddMove(Id, currentLocation.positionX, currentLocation.positionY); // to buffer for unity
              // MessageBox.Show("Move added" + $"{Buffer.movements.Count}");
            }
        }


        //------------PATHFINDING ALGORITHMS----------------
        //
        //The appropriate method will be called on each timer tick
        //The method calculates the next move and calls the corresponding
        //method for moving from the ones above
        //(Up, Down, Left or Right)

        //IF PERSONALITY IS "PUSSY"
        public void RunForTheExit()
        {
            this.NewAlgorithm(); //create a new a* algorithm

            if (currentLocation == endLocation)
            {
                this.Save(); //person is saved
                return;
            }
            else if (currentLocation.onFire)
            {
                this.Die(); //person dies
                return;
            }
            if (blindPath != null)
            {
            if (this.blindPath.Count == 0) //if the blind path is empty
            {
                blindPath = this.A_STAR.GetFullPath(); //get a new path
                if(blindPath != null)
                {
                    this.blindPath.RemoveAt(blindPath.Count - 1); //remove the start position
                    this.RunForTheExit(); //call the method with a path
                    return;
                }
            }
            else
            {
                //run algorithm to determine where to move
                Tile nextLocation;
                nextLocation = this.blindPath[blindPath.Count - 1]; //get the next move

                //check the line of sight, because it removes the tiles on fire
                if (nextLocation.onFire || !lineOfSight.Contains(nextLocation)) //if the next location is on fire OR if it is not in the line of sight
                {
                    //find a way to go around it if possible
                    //get the first tile which is not on fire
                    for (int i = blindPath.Count - 1; i >= 0; i--) //go through the list
                    {
                        if (!blindPath[i].onFire || i == 0) //find the first tile in the list that is not on fire
                        {
                            nextLocation = this.CheckAvailablePath(blindPath[i]); //the next move should be in regards to going around the fire
                            break;
                        }
                        else
                        {
                            blindPath.RemoveAt(i); //remove the tile on fire
                        }
                    }
                }

                //if there is no way to go around the fire
                if (nextLocation == null)
                {
                    if (this.fireEscapes.Count > 0) //if there are other escapes
                    {
                        this.SetDestination(GetClosestFireEscape(currentLocation)); //change to the closest one available
                        this.blindPath.Clear(); //empty the blind path to set a new one
                        this.RunForTheExit(); //enter recursion to avoid staying in one place for too long
                        return; //exit out of the recursion
                    }
                    else //if no exits are available
                    {
                        this.exitBlocked = true; //if person is a shelfish prick and the path to exit is blocked, he will run for extinguisher

                        if (this.personality != PERSONALITY.SELFISH_PRICK) //check in case the person is not selfish (if hero or pussy)
                            this.personality = PERSONALITY.SHAGGY_FROM_SCOOBY_DOO; //person starts to move in circles
                    }
                }
                else
                {
                    //move to the appropriate location
                    if (nextLocation == currentLocation.up)
                        MoveUp();
                    else if (nextLocation == currentLocation.left)
                        MoveLeft();
                    else if (nextLocation == currentLocation.down)
                        MoveDown();
                    else if (nextLocation == currentLocation.right)
                        MoveRight();
                    else { } //if next location is the current one, stay here

                    if (blindPath.Contains(nextLocation)) //if the next location is part of the blind path after all this
                    {
                        for (int i = blindPath.Count - 1; i >= 0; i--) //loop through the list in reverse (from current to end)
                        {
                            if (blindPath[i] != nextLocation) //if there are tiles before the next move
                            {
                                blindPath.RemoveAt(i); //remove them (this will put the person back on track)
                            }
                            else
                                break; //once it reaches the nextLocation, break the loop
                        }

                        //finally, remove the next location from the blind Path
                        blindPath.Remove(nextLocation);
                    }
                }
            }

            }
        }

        //IF PERSONALITY IS "SELFISH"
        public void ExitOrExtinguisher() //
        {
            this.NewAlgorithm();
            
            if (!this.exitBlocked)
            {
                this.RunForTheExit(); // run for exit until path is blocked
            }
            else if(!this.extinguisherBlocked)
            {
                if (!selfishChanged) //if the new goal (extinguisher) has not been changed, change it
                {
                    this.SetDestination(GetClosestFireExtinguisher(currentLocation));
                    this.selfishChanged = true;
                }

                this.RunForExtinguisher(); //run for extinguisher if extinguisher is not blocked
            }
            else
            {
                this.personality = PERSONALITY.SHAGGY_FROM_SCOOBY_DOO; //run in circles
            }
            
        }

        //IF PERSONALITY IS "HERO"
        public void RunForExtinguisher()
        {
            List<Tile> pathToFire; //store the path to the closest fire

            if (currentLocation.onFire)
            {
                this.Die(); //person dies
                return;
            }

            if (this.extinguisher != null) //if there is an extinguisher
            {
                if (this.extinguisher.Fuel <= 0)//if there is no fuel
                {
                    this.RemoveExtinguisher();
                    return; //exit the recursion created by the other method
                }
                this.DIJKSTRA = new Dijkstra(currentLocation, grid);
                bool assigned = false;
                pathToFire = new List<Tile>();

                foreach (Tile n in currentLocation.neighbours)
                {
                    if (n.onFire && lineOfSight.Contains(n))
                    {
                        assigned = true;
                        pathToFire.Add(n);
                    }
                }

                if (!assigned)
                {
                    pathToFire = this.DIJKSTRA.CalculatePath();
                }


                if (pathToFire.Count > 2) //if there is more than 1 move (final tile is on fire)
                {

                    if (pathToFire[pathToFire.Count-2] == currentLocation.up) //move up
                    {
                        MoveUp();
                    }
                    else if (pathToFire[pathToFire.Count - 2] == currentLocation.down) //move down
                    {
                        MoveDown();
                    }
                    else if (pathToFire[pathToFire.Count - 2] == currentLocation.left) //move left
                    {
                        MoveLeft();
                    }
                    else if (pathToFire[pathToFire.Count - 2] == currentLocation.right) //move right
                    {
                        MoveRight();
                    }
                }
                else //if there are no moves, then person is at the end location
                {
                    pathToFire[0].available = false; //the fire is currently being extinguished by a person, so no other person can have it set as a destination

                    //do the extinguishy stuff
                    if (this.extinguisher.Extinguish(pathToFire[0].GetFire())) //if there is no malfunction
                    {
                        if (pathToFire[0].GetFire().isExtinguished) //if the fire is extinguished
                        {
                            pathToFire[0].available = true; //make the tile available again
                        }
                    }
                    else //there is a malfunction
                    {
                        this.RemoveExtinguisher();
                        pathToFire[0].available = true; //make the tile available for another person to extinguish
                        return; //exit the recursion created by the other method
                    }
                    
                }
            }
            else
            {
                if (this.heroChanged) //if his destination is changed to the exit, always go there
                {
                    this.RunForTheExit(); // start running for the exit
                    return; //don't continue down
                }

                if (currentLocation == endLocation && !extinguisherBlocked) //AND extinguisher is not blocked because he is stuck in one place if there is no ext. on his position
                {
                    this.GetExtinguisher();
                }
                else
                {
                    this.NewAlgorithm(); //create a new a* algorithm

                    if (this.blindPath.Count == 0 && !extinguisherBlocked) //if the blind path is empty AND ext. is not blocked (otherwise causes stackOverflowException)
                    {
                        blindPath = this.A_STAR.GetFullPath(); //get a new path
                        this.blindPath.RemoveAt(blindPath.Count - 1); //remove the start position
                        this.RunForExtinguisher(); //call the method with a path
                        return;
                    }
                    else
                    {
                        if (!extinguisherBlocked) //if path to extinguisher is not blocked
                        {
                            Tile nextLocation;
                            nextLocation = this.blindPath[blindPath.Count - 1]; //get the next move

                            //check the line of sight, because it removes the tiles on fire
                            if (nextLocation.onFire || !lineOfSight.Contains(nextLocation)) //if the next location is on fire OR if it is not in the line of sight
                            {
                                //find a way to go around it if possible
                                //get the first tile which is not on fire
                                for (int i = blindPath.Count - 1; i >= 0; i--) //go through the list
                                {
                                    if (!blindPath[i].onFire || i==0) //find the first tile in the list that is not on fire
                                    {
                                        nextLocation = this.CheckAvailablePath(blindPath[i]); //the next move should be in regards to going around the fire
                                        break;
                                    }
                                    else
                                    {
                                        blindPath.RemoveAt(i); //remove the tile on fire
                                    }
                                }
                            }
                            
                            if(nextLocation == null) //if there is no way around the fire
                            {
                                if (this.NewExtinguisher()) //if there is an extinguisher (if not, the extinguisherBlocked is set to true)
                                {
                                    this.blindPath.Clear(); //empty the blind path to set a new one
                                }

                                this.RunForExtinguisher(); //enter recursion to not stay in one place too long
                                return;
                            }
                            else
                            {
                                //move to the appropriate location
                                if (nextLocation == currentLocation.up)
                                    MoveUp();
                                else if (nextLocation == currentLocation.left)
                                    MoveLeft();
                                else if (nextLocation == currentLocation.down)
                                    MoveDown();
                                else if (nextLocation == currentLocation.right)
                                    MoveRight();
                                else { } //if next location is the current one, stay here

                                if (blindPath.Contains(nextLocation)) //if the next location is part of the blind path after all this
                                {
                                    for (int i = blindPath.Count - 1; i >= 0; i--) //loop through the list in reverse (from current to end)
                                    {
                                        if (blindPath[i] != nextLocation) //if there are tiles before the next move
                                        {
                                            blindPath.RemoveAt(i); //remove them (this will put the person back on track)
                                        }
                                        else
                                            break; //once it reaches the nextLocation, break the loop
                                    }

                                    //finally, remove the next location from the blind Path
                                    blindPath.Remove(nextLocation);
                                }
                            }
                            
                        }
                        else //if all extinguishers are blocked
                        {
                            this.heroChanged = true;
                            this.SetDestination(GetClosestFireEscape(currentLocation)); //set exit as new destination
                            this.blindPath.Clear(); //clear the path to set a new one to the exit
                            this.RunForExtinguisher(); //call the method again with the updated destination
                            return; //exit recursion
                        }
                    }
                }
            }
        }

        //IF PERSONALITY IS "SHAGGY"
        //Note: this is an easter egg,
        //Shaggy will only run in a circle
        public void BeShaggy()
        {
            switch (ShaggyCounter)
            {
                //First move Up
                case 0:
                    this.MoveUp();
                    ShaggyCounter++;
                    break;
                //Second move right
                case 1:
                    this.MoveRight();
                    ShaggyCounter++;
                    break;
                //Third move down
                case 2:
                    this.MoveDown();
                    ShaggyCounter++;
                    break;
                //Finally, move left and reset counter to repeat
                default:
                    this.MoveLeft();
                    ShaggyCounter = 0;
                    break;
            }
        }

        //-------------------------------------------------------------

        //when person dies
        public void Die()
        {
            this.isDead = true;
            this.currentLocation.personOnTile = false;

            foreach(Tile n in currentLocation.neighbours)
            {
                n.available = true;
            }

            Buffer.AddDeadPerson(Id); // To buffer for unity
            //invoke event for statistics if someone dies
            if (SomeoneDies != null)
            {
                SomeoneDies.Invoke(this);
            }
        }

        //when person is saved
        private void Save()
        {
            this.isSaved = true;
            this.currentLocation.personOnTile = false;

            //invoke event for statistics if someone is saved
            if (SomeoneEscapes != null)
            {
                SomeoneEscapes.Invoke();
            }
        }

        //get a new extinguisher object
        private void GetExtinguisher()
        {
            foreach(Tile t in lineOfSight) //up, down, left and right
            {
                if (sharedExtinguishers.Contains(t)) //if it is still in the shared extinguishers list (it has not been taken yet)
                {
                    this.extinguisher = new Extinguisher();
                    this.sharedExtinguishers.Remove(t); //remove it from the shared list
                    this.DIJKSTRA = new Dijkstra(currentLocation, grid); //create the dijsktra object
                    this.NewExtinguisher(); //create a new extinguisher
                    
                    if(RedrawHandler != null)
                    {
                        RedrawHandler.Invoke(t); //Call the form's redraw method to redraw a wall
                    }

                    return; //spend one tick to get the extinguisher
                }
            }

            this.NewExtinguisher(); //set a new extinguisher
            RunForExtinguisher(); //avoid wasting time in one place when no extinguisher at the current place
        }

        //create a new algorithm with the current position as start point
        private void NewAlgorithm()
        {
            this.A_STAR = new Pathfinding(currentLocation, endLocation);
        }
        

        //Moved the methods from the Form class to here
        //They get the closest fire escape or extinguisher to the person at the moment
        //Once it locates the closest to them, it removes them from the list
        private Tile GetClosestFireEscape(Tile spawn) //will retrieve the closest fire escape
        {
            if (fireEscapes.Count <= 0)
            {
                return null;
            }

            Tile closest = null;
            int minDistance = 999;
            int currentDistance;
            Pathfinding closestEsc;
            foreach (Tile t in this.fireEscapes)
            {
                closestEsc = new Pathfinding(currentLocation, t); //find the actual traversable path to the closest escape
                currentDistance = closestEsc.CalculatePath(true).Count;
                if (currentDistance < minDistance)
                {
                    closest = t;
                    minDistance = currentDistance;
                }
            }

            fireEscapes.Remove(closest);
            return closest;
        }

        private Tile GetClosestFireExtinguisher(Tile spawn) //will retrieve the closest extinguisher
        {
            if (fireExtinguishers.Count <= 0)
            {
                return null;
            }

            Tile closest = null;
            Tile t1 = null; //the tile infront of the extinguisher
            int minDistance = 999;
            int currentDistance;
            Pathfinding closestExt;

            foreach (Tile t in this.fireExtinguishers)
            {
                //Check the extinguisher and get the tile infront of it for the pathfinding
                //Otherwise, the extingiusher is not walkable and the algorithm breaks
                if (t.colorInBitmap == Color.FromArgb(255, 150, 0, 150))
                    t1 = t.up;
                else if (t.colorInBitmap == Color.FromArgb(255, 50, 200, 50))
                    t1 = t.left;
                else if (t.colorInBitmap == Color.FromArgb(255, 255, 0, 0))
                    t1 = t.right;
                else if (t.colorInBitmap == Color.FromArgb(255, 255, 0, 50))
                    t1 = t.down;

                closestExt = new Pathfinding(currentLocation, t1); //find the actual traversable path to the tile infront of the extinguisher
                currentDistance = closestExt.CalculatePath(true).Count;
                if (currentDistance < minDistance)
                {
                    closest = t;
                    minDistance = currentDistance;
                }
            }

            fireExtinguishers.Remove(closest);

            //Check which extinguisher it is, and return the walkable tile infront of it
            if (closest.colorInBitmap == Color.FromArgb(255, 150, 0, 150))
                closest = closest.up;
            else if (closest.colorInBitmap == Color.FromArgb(255, 50, 200, 50))
                closest = closest.left;
            else if (closest.colorInBitmap == Color.FromArgb(255, 255, 0, 0))
                closest = closest.right;
            else if (closest.colorInBitmap == Color.FromArgb(255, 255, 0, 50))
                closest = closest.down;

            return closest;
        }



        //Set the initial end location
        private void SetInitialEndPoint(Tile start)
        {
            Tile endLocation;

            if (personality == PERSONALITY.PUSSY || personality == PERSONALITY.SELFISH_PRICK) //set closest fire escape
            {
                endLocation = GetClosestFireEscape(start);
                if (endLocation == null) //If there is no fire exit at all on the map
                {
                    if (personality == PERSONALITY.SELFISH_PRICK) //if person is selfish, find nearest extinguisher
                    {
                        SetDestination(GetClosestFireExtinguisher(start));
                    }
                    else
                    {
                        SetDestination(endLocation); //set closest fire exit (when null, person becomes Shaggy)
                    }
                }
                else
                {
                    SetDestination(endLocation); //set closest fire exit
                }
            }
            else //set closest extinguisher
            {
                endLocation = GetClosestFireExtinguisher(start);
                if (endLocation == null) //if no extinguishers on the map
                {
                    SetDestination(GetClosestFireEscape(start)); //look for closest exit
                }
                else
                {
                    SetDestination(endLocation); //set null end locaion (will become Shaggy)
                }
            }
        }


        //Removes the extinguisher when no fuel or malfunction and calls the RunForExtinguisher method without an extinguisher
        private void RemoveExtinguisher()
        {
            this.extinguisher = null; //remove the extinguisher
            RunForExtinguisher(); //run the method without extinguisher
        }

        //Checks if there are other extinguishers available, and if so, set the closest one as an end location
        private bool NewExtinguisher()
        {
            if (this.fireExtinguishers.Count > 0) //check if other extinguishers are available
            {
                this.SetDestination(GetClosestFireExtinguisher(currentLocation)); //set the new extinguisher
                return true;
            }
            else //if no available extinguishers
            {
                extinguisherBlocked = true; //block path to extinguisher
                return false;
            }
        }

        //Updates the line of sight
        private void UpdateLineOfSight()
        {
            this.lineOfSight.Clear();

            //Only up, down, left and right to use when moving
            if (currentLocation.up != null)
                this.lineOfSight.Add(currentLocation.up); //up
            if (currentLocation.left != null)
                this.lineOfSight.Add(currentLocation.left); //left
            if (currentLocation.right != null)
                this.lineOfSight.Add(currentLocation.right); //right
            if (currentLocation.down != null)
                this.lineOfSight.Add(currentLocation.down); //down

            //if(currentLocation.up.left != null)
            //    this.lineOfSight.Add(currentLocation.up.left); //left upper corner
            //if (currentLocation.up.right != null)
            //    this.lineOfSight.Add(currentLocation.up.right); //right upper corner
            //if (currentLocation.left.down != null)
            //    this.lineOfSight.Add(currentLocation.left.down); //lower left corner
            //if (currentLocation.right.down != null)
            //    this.lineOfSight.Add(currentLocation.right.down); //lower right corner

        }

        //Checks if there is a possible path to the specified end at all
        private Tile CheckAvailablePath(Tile end)
        {
            Pathfinding temp = new Pathfinding(currentLocation, end); //create a pathfinder from current position to the specified location
            return temp.GetNextMove();
        }
        

        //THE FOLLOWING METHOD IS ONLY FOR PERFORMING UNIT TESTS
        //NOT USED ANYWHERE ELSE
        public void SWITCHPERSONALITYFORTESTING(PERSONALITY newPers)
        {
            if (this.personality != PERSONALITY.HERO_OF_MANKIND && newPers == PERSONALITY.HERO_OF_MANKIND)
                this.SetDestination(GetClosestFireExtinguisher(this.currentLocation));
            else if(this.personality != PERSONALITY.PUSSY && newPers == PERSONALITY.PUSSY || personality != PERSONALITY.SELFISH_PRICK && newPers == PERSONALITY.SELFISH_PRICK)
            {
                this.SetDestination(GetClosestFireEscape(this.currentLocation));
            }

            this.personality = newPers;
        }

        public List<Coordinates> GetPathForUnity()
        {
            List<Coordinates> l = new List<Coordinates>();

            
            for (int i = blindPath.Count - 1; i >= 0; i--) // revese the order of the list
            {
                Coordinates c = new Coordinates(blindPath[i].positionX, blindPath[i].positionY);
                l.Add(c);
            }
            return l;
        }
    }

    //enumeration for personality
    public enum PERSONALITY
    {
        PUSSY,
        SELFISH_PRICK,
        HERO_OF_MANKIND,
        SHAGGY_FROM_SCOOBY_DOO
    };


   
}
