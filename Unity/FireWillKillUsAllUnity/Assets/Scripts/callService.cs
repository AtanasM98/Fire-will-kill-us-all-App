using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ServiceModel;
using System.Drawing;
using System.IO;
using FireWillKillUsAll;

public class callService : MonoBehaviour
{
    // Start is called before the first frame update
   
    
    public GameObject gridObject;
    Grid grid;
    public GameObject chModel;
    public GameObject deadModel;
    public float TimerTick = 3f; // how often does the service pull
    private float timerReset;
    UnityService1Client client;
    float yOffset = 0.6f;

    UnityPath[] pathz;


    bool mapSet = false;

    void Start()
    {
        timerReset = TimerTick;
        client = new UnityService1Client(new BasicHttpBinding(), new EndpointAddress("http://localhost:8733/Design_Time_Addresses/FireWillKillUsAll/UnityService1/"));
        
        //proxy = new UnityService1Client(context);

        grid = gridObject.GetComponent<Grid>();

        //grid.CreateMapFromArrayCol(client.GetMap(), 25, 21);

       /* CreatePerson(2, 2, 15);
        CreatePerson(3, 16, 12);
     CreatePerson(4, 11, 5);*/
    }

    private void Update()
    {
        TimerTick -= Time.deltaTime;

        if (TimerTick<= 0)
        {
            UpdatesFlush up = client.FlushUpdates();

            //Debug.Log($"moves-{up.movements.Length},fireSpreads-{up.fireSpreads.Length},deadPeople-{up.deadPeople.Length}");


            KillPeople(up.deadPeople);       // add Death Possiton to the service
            pathz = up.movements;
            ExecuteMoves(up.movements);
            ExecuteFires(up.fireSpreads);

            if (!mapSet)
            {
                grid.CreateMapFromArrayCol(client.GetMap(), 25, 21); // getMap
                mapSet = true;
            }
            
                //Debug.Log($"{up.fireSpreads.Length}");
            
            
            
            TimerTick = timerReset;
        }
    }
    /*
    void ExecuteMoves(UnityMovement[] moves)
    {
        foreach (UnityMovement m in moves)
        {
            bool personExists = false;
            foreach (Person p in grid.people)
            {
                
                if (m.PersonId == p.Id)
                {
                    personExists = true;

                    int xFlip;
                    int yFlip;
                    grid.FlipCoordinates(m.X, m.Y, out xFlip, out yFlip);    //flip

                    p.MoveToCell(xFlip, yFlip);
                   // Debug.Log($"{p.Id}"); // test

                    break;
                }
                
            }
            if (!personExists)
            {
                int xFlip;
                int yFlip;
                grid.FlipCoordinates(m.X, m.Y, out xFlip, out yFlip);    //flip

                CreatePerson(m.PersonId, xFlip, yFlip);
               // Debug.Log($"{m.PersonId}"); // test
            }
            
        }
    }*/
    void ExecuteMoves(UnityPath[] paths)
    {
        foreach (UnityPath m in paths)
        {
            if (m.moves.Length > 0)
            {
                int xFlip;
                int yFlip;
                List<Coordinates> fliped = new List<Coordinates>();
                foreach (FireWillKillUsAll.Coordinates c in m.moves)               // flip coordinates on path 
                {
                    
                    grid.FlipCoordinates(c.X, c.Y, out xFlip, out yFlip);
                    fliped.Add(new Coordinates(xFlip, yFlip));
                }

                bool personExists = false;
                foreach (Person p in grid.people)
                {


                    if (m.PersonId == p.Id)
                    {
                        personExists = true;


                        //flip
                        if (!p.dead)
                        {
                            p.charModel.GetComponent<LerpHelper>().ResetMoves(fliped);
                            // Debug.Log($"{p.Id}"); // test
                        }


                        break;
                    }

                }
                
                    if (!personExists)
                    {
                        
                        int mo;
                        int mu;
                        mo = m.moves[0].X;
                        mu = m.moves[0].Y;

                        grid.FlipCoordinates(mo, mu, out xFlip, out yFlip);    //flip

                        Person p = CreatePerson(m.PersonId, xFlip, yFlip);
                        p.charModel.GetComponent<LerpHelper>().ResetMoves(fliped);
                        // Debug.Log($"{m.PersonId}"); // test
                    }
                
            }
            
            

        }
    }
    void ExecuteFires(FireWillKillUsAll.Coordinates[] fires)
    {
        foreach (FireWillKillUsAll.Coordinates f in fires)
        {
            // Debug.Log($"{f.X},{f.Y}"); // test

            int xFlip;
            int yFlip;
            grid.FlipCoordinates(f.X, f.Y, out xFlip, out yFlip);    //flip

            grid.SpawnFire(xFlip, yFlip);
        }
    }
    void KillPeople(int[] deads)
    {
       /* foreach (int p in deads)
        {
            Debug.Log($"{p}");
        }*/

        foreach (Person p in grid.people)
        {
            foreach (int i in deads)
            {
                Vector3 fal = new Vector3(-1, -1, -1);
                Vector3 dP = fal;
                if (p.Id == i)
                {
                    if (!p.dead)
                    {
                        Vector3 pos = p.charModel.transform.position;
                        Destroy(p.charModel);
                        foreach (UnityPath m in pathz)
                        {
                            if (m.PersonId == p.Id)
                            {
                                try
                                {
                                    int fX = 0, fY = 0;
                                    grid.FlipCoordinates(m.moves[0].X, m.moves[0].Y, out fX,out fY);
                                    dP = grid.cells[fX,fY].Object.transform.position;
                                }
                                catch (System.Exception)
                                {

                                    dP = fal;
                                }

                            }
                        }
                        if (dP != fal)
                        {
                            p.DieAtPos(Instantiate(deadModel, pos, Quaternion.identity), dP);
                        }
                        else
                        {
                            p.Die(Instantiate(deadModel, pos, Quaternion.identity));
                        }
                        
                    }
                   

                }
            }
        }


    }
   
    public Person CreatePerson(int Id,int X, int Y)
    {

        Vector3 pos = new Vector3(grid.cells[X, Y].x, grid.cells[X, Y].y + yOffset, grid.cells[X, Y].z);
        Person p = new Person(Id, X, Y,Instantiate(chModel,pos,Quaternion.identity), grid.cells);
        grid.people.Add(p);
        return p;
    }
    public void MoveToCell(int personId, int x, int y)
    {
        for (int i = 0; i < grid.people.Count; i++)
        {
            if (personId == grid.people[i].Id)
            {
                grid.people[i].MoveToCell(x,y);
            }
            else
            {
                Debug.Log("Person not found");
            }
        }
    }
    /*
    public void MovePerson(int personId, string dir)
    {
        bool set = false;
        for (int i = 0; i < grid.people.Count; i++)
        {
            if (personId == grid.people[i].Id)
            {
                if (dir == "up")
                {
                    grid.people[i].MoveUp();
                    set = true;
                    break;
                }
                else if (dir == "down")
                {
                    grid.people[i].MoveDown();
                    set = true;
                    break;
                }
                else if (dir == "left")
                {
                    grid.people[i].MoveLeft();
                    set = true;
                    break;
                }
                else if (dir == "right")
                {
                    grid.people[i].MoveRight();
                    set = true;
                    break;
                }
            }
        }
        if (!set)
        {
            Debug.Log("Failed to move!!!");
        }
    }
    */
    public void SetMap(string[] bitmapArray, int x, int y)
    {
        grid.CreateMapFromArrayCol(bitmapArray, x, y);
    }

    public void LerpPeople()
    {
        foreach (Person p in grid.people)
        {

        }
    }
}
