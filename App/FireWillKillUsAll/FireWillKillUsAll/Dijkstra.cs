using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Dijkstra
    {
        //****************************************************************************************************************************
        //
        //-------------------------------------- THE CLASS CONTAINING THE DIJKSTRA ALGORITHM -----------------------------------------
        //
        //****************************************************************************************************************************
        
        private Tile start; //start point
        private Tile current; //store the current location
        private Dictionary<Tile, int> distance; //distance from start to current point
        private Dictionary<Tile, Tile> cameFrom; //previous tile
        private List<Tile> grid; //the whole grid

        public Dijkstra(Tile start, List<Tile> grid)
        {
            this.start = this.current = start;
            distance = new Dictionary<Tile, int>();
            cameFrom = new Dictionary<Tile, Tile>();
            this.grid = grid;
            
            foreach(Tile t in this.grid)
            {
                if(t != null)
                    distance[t] = 9999; //initialize each tile with distance of infinity (realistically, 9999 will do the job)
            }

            //distance from start to start is 0
            distance[start] = 0;
        }


        //---------------------MAIN METHOD PERFORMING THE CALCULATIONS-------------------------------
        public List<Tile> CalculatePath()
        {
            List<Tile> Q = new List<Tile>(); //the list with the tiles to consider
            foreach(Tile t in this.grid)
            {
                Q.Add(t); //make a copy of the grid to not remove tiles from it
            }

            int minDist;
            List<Tile> finalPath = new List<Tile>();
            while(Q.Count > 0) //while the list is not empty
            {
                minDist = 999; //reset it everytime to be accurate

                foreach (Tile t in Q) //for each tile in the list
                {
                    if (t != null && distance[t] < minDist) //if the distance is lower than the minimum
                    {
                        current = t; //add the tile with the lowest distance
                        minDist = distance[t];
                    }
                }

                Q.Remove(current); //remove it from the list

                if (current.onFire && current.available) //if the current tile is on fire AND it is not yet being extinguished
                {
                    finalPath.Add(current); //add the tile that is on fire

                    while (cameFrom.Keys.Contains<Tile>(current) && current != null) //while there exists a previous tile (start tile has no previous)
                    {
                        current = cameFrom[current]; //get the previous tile and add it to the final path
                        finalPath.Add(current);
                    }

                    return finalPath; //return the path from END to START
                }

                foreach (Tile neighbour in current.neighbours) //for all neighbours of the current tile
                {
                    if (!Q.Contains(neighbour)) //if the neighbour is no longer in the list
                    {
                        continue; //skip it as it has been processed
                    }
                    else if (!neighbour.walkable) //if the neighbour is not walkable
                    {
                        Q.Remove(neighbour); //remove it from the list
                        continue; //skip the iteration
                    }

                    int alt = distance[current] + 1; //alternate path with length distance from start to current tile + 1

                    if (alt < distance[neighbour]) //if the alternate path is better than the current best
                    {
                        distance[neighbour] = alt;
                        cameFrom[neighbour] = current;
                    }
                }
            }

            return finalPath; //placeholder to satisfy the compiler
        }
        //---------------------------------------------------------------------------------





        //****************************************************************************************************************************
        //****************************************************************************************************************************
    }
}
