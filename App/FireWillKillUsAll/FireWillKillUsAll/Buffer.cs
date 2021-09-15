using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireWillKillUsAll
{
    public static class Buffer
    {

        public static List<UnityPath> paths = new List<UnityPath>();


        public static List<Coordinates> fireSpreads = new List<Coordinates>();

        private static List<int> deadPeople = new List<int>();

        public static void AddMove(int personId,Coordinates[] moves)
        {
            paths.Add(new UnityPath(personId, moves));
        }
        public static void GetAllMoves(List<Person> people)
        {
            List<UnityPath> pa = new List<UnityPath>();
            foreach (Person p in people)
            {
                Coordinates[] c = p.GetPathForUnity().ToArray();
               /* foreach (Coordinates co in c)
                {
                    MessageBox.Show($"{co.X}, {co.Y}");
                }*/
                UnityPath u = new UnityPath(p.Id, c);
                pa.Add(u);
            }
            paths = pa;
        }
        public static void  AddFireSpread(int x, int y)
        {
            fireSpreads.Add(new Coordinates(x, y));
        }
        public static void AddDeadPerson(int id)
        {
            deadPeople.Add(id);
        }

        public static UpdatesFlush Flush()
        {
            UpdatesFlush temp = new UpdatesFlush(paths.ToArray(), fireSpreads.ToArray(), deadPeople.ToArray());
           // MessageBox.Show($"{temp.movements.Length},{temp.fireSpreads.Length},{temp.deadPeople.Length}");
            paths.Clear();
            fireSpreads.Clear();
            return temp;
        }
    }
}
