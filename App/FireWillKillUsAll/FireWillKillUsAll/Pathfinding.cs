using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Pathfinding
    {
        //****************************************************************************************************************************
        //
        //-------------------------------------- THE CLASS CONTAINING THE A* ALGORITHM -----------------------------------------------
        //
        //****************************************************************************************************************************
        

        private List<Tile> openSet;  //Tiles to consider
        private List<Tile> closedSet;  //Tiles already checked
        private Tile current; //store the current tile
        private Tile goal; //end tile
        private Dictionary<Tile, int> gCost; //distance from start
        private Dictionary<Tile, int> fCost; //distance to finish
        private Dictionary<Tile, Tile> cameFrom; //holds ech previous tile from the current tile


        //initialize the open set with only the start point in the beginning
        public Pathfinding(Tile start, Tile goal)
        {
            this.closedSet = new List<Tile>();
            this.openSet = new List<Tile>();
            this.openSet.Add(start); //add the start point to the open set as the first tile for consideration
            this.current = start;
            this.goal = goal;

            this.gCost = new Dictionary<Tile, int>();
            this.fCost = new Dictionary<Tile, int>();
            this.cameFrom = new Dictionary<Tile, Tile>();

            //gCost from start to start is 0
            gCost[start] = 0;
            //fCost from start is completely heuristical
            fCost[start] = EstimateHCost(start, goal);
        }

        //After checking the whole path, it will return the next tile
        //the person needs to move to.
        public Tile GetNextMove()
        {
            List<Tile> finalPath = this.CalculatePath(true); //call the method that actually performs the calculations, do the check
            if (finalPath != null && finalPath.Count>0) //if list is null or empty
            {
                finalPath.Reverse(); //reverse the path so that it becomes from START to FINISH
                if (finalPath[1].personOnTile)
                    return finalPath[0]; //return the current location if there is a person on the next tile, to avoid 2 people on same tile
                else
                    return finalPath[1]; //return the second position of the successful path, so the person can move to it
            }
            else
            {
                return null;
            }
        }

        public List<Tile> GetFullPath()
        {
            List<Tile> finalPath = this.CalculatePath(false);
            if (finalPath != null && finalPath.Count > 0) //if list is null or empty
            {
                return finalPath; //return the path from END to START
            }
            else
            {
                return null;
            }
        }

        //---------------------MAIN METHOD PERFORMING THE CALCULATIONS-------------------------------
        public List<Tile> CalculatePath(bool check)
        {
            int minFCost; //keep track of the lowest Fcost to determine next tile
            int tentativeGCost; //keep the distance between the start and the neighbouring node
            List<Tile> finalPath = new List<Tile>();
            while (this.openSet.Count > 0)
            {
                minFCost = 999; //reset the lowest Fcost for the next iteration

                foreach(Tile t in openSet) //loop through the tiles in the set for consideration
                {
                    t.AddNeighbours();
                    if (fCost[t] < minFCost)
                    {
                        current = t; //add the tile with the lowest Fcost
                        minFCost = fCost[current]; //update the lowest Fcost
                    }
                }

                //if the tile ends up being the goal tile
                if(current == goal)
                {
                    finalPath.Add(current);
                    while(cameFrom.Keys.Contains<Tile>(current)) //while there exists a previous tile (start tile has no previous)
                    {
                        current = cameFrom[current];
                        finalPath.Add(current);
                    }

                    return finalPath; //get the final path starting from END POINT and finsishing with START POINT
                }

                //add the tile to the closed set and remove it from the open one
                openSet.Remove(current);
                closedSet.Add(current);

                current.AddNeighbours();

                //loop through all the neighbours
                foreach(Tile neighbour in current.neighbours)
                {
                    neighbour.AddNeighbours();
                    if (closedSet.Contains(neighbour))
                    {
                        continue; //ignore if neighbour tile is already checked
                    }

                    tentativeGCost = gCost[current] + 1; //Gscore of the current + distance between current and neighbour tile (always 1)

                    if (!openSet.Contains(neighbour)) //check if neighbour is in the open set
                    {
                        if (neighbour.walkable) //if neighbour is walkable
                        {
                            if (check) //if a person calls the method, check if it is walkable
                            {
                                if (!neighbour.onFire) //if neighbour is NOT on fire, then add it
                                {
                                    openSet.Add(neighbour); //add it for consideration
                                }
                            }
                            else //if air calls the method, then no need to check
                            {
                                openSet.Add(neighbour); //add it for consideration
                            }
                        }
                        
                    }
                    else if (tentativeGCost >= this.gCost[neighbour]) //if it is, check the gScore
                    {
                        continue; //if the gScore is greater or the same, ignore this tile
                    }

                    //if all tests pass, this is the best path so far, keep track of it
                    cameFrom[neighbour] = current; //the new tile came from the current one
                    this.gCost[neighbour] = tentativeGCost; // the Gcost of the new tile
                    this.fCost[neighbour] = EstimateHCost(neighbour, goal) + tentativeGCost; //the Fcost of the new Tile (estimated to the end + distance from start)
                }
            }

            //
            return finalPath; //placeholder to satisfy the compailer
        }
        //---------------------------------------------------------------------------------


        public List<Tile> Whoosh() //Method for the wind to successfully find its path. No need to check if tile is on fire since it is wind
        {
            List<Tile> finalPath = this.CalculatePath(false); //call the method that actually performs the calculations, don't check
            if (finalPath != null && finalPath.Count > 0) //if list is null or empty
            {
                finalPath.Reverse(); //reverse the path so that it becomes from START to FINISH
                return finalPath; //return the whole list, since it doesn't care if it is on fire or not, so no need to be dynamic
            }
            else
            {
                return null;
            }
        }



        //Estimates the hCost (distance from current node to the end)
        private int EstimateHCost(Tile currentTile, Tile endTile)
        {
            if (endTile != null)
                //get the sum of the absolute difference between the tiles' X positions and Y positions
                return Math.Abs(endTile.positionX - currentTile.positionX) + Math.Abs(endTile.positionY - currentTile.positionY);
            else
                return 0;
        }


        //****************************************************************************************************************************
        //****************************************************************************************************************************
    }
}
