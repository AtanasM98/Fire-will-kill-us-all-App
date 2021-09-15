using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWillKillUsAll
{
    public class Extinguisher
    {
        //extinguisher fuel, max 100
        private int fuel;
        private Random rand = new Random((int)DateTime.Now.Ticks);

        private int fuelDecrease = 5; //how much fuel is used on each extinguish, change here if needed

        //chance to malfunction; if true, it will not work
        private bool malfunction;

        public int Fuel { get { return fuel; } }

        public Extinguisher()
        {
            //get random fuel
            this.fuel = rand.Next(80, 100);
            
            //10% chance the extinguisher will malfunction
            switch (rand.Next(0, 100))
            {
                case int n when (n >= 0 && n <= 10):
                    this.malfunction = true;
                    break;
                default:
                    this.malfunction = false;
                    break;
            }
        }

        public bool Extinguish(Fire f)
        {
            //if no malfunction, then extinguish
            if (!malfunction)
            {
                fuel -= fuelDecrease; //decrease the fuel
                if (fuel <= 0)
                {
                    //do nothing
                }
                else
                {
                    f.ExtinguishWithExtinguisher(fuelDecrease*3); //use three times the fuel decrease on the fire
                }

                return true; //extinguish has been performed
            }
            else
            {
                return false; //there is a malfunction
            }
        }
    }
}
