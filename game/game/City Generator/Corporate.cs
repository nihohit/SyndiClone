using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * a Corporate is a collection of buildings.
 * */
namespace Game.City_Generator
{
    enum CorporateNames { ZOMBIE_LTD,BRAINS,UPGRADES,MINING,ARMORY,RND,DEFENCE,DIGGING}; //TODO: figure out what kinds of corporates do we want.
    class Corporate
    {
        static int counter = 0;
        static Random rand = new Random();

        /********************************members***************************************/
        private List<Building> _buildings;
        private int _id;
        private CorporateNames _type;
      

        /********************************Constructor***************************************/        
        internal Corporate() {

           _type = (CorporateNames)rand.Next(Enum.GetValues(typeof(CorporateNames)).Length);
            
            _id = counter;
            counter++;
            _buildings = new List<Building>();
        }


        /********************************Simple Methods***************************************/

        public int Id
        {
            get { return _id; }
        }
        
        internal void addBuilding (Building b){
            _buildings.Add(b);
        }

        internal void removeBuilding(Building b) {
            _buildings.Remove(b);
        }

        internal bool CanBuild(Building b) { 
            if (b.Corp != this) return false;
            foreach (Building other in _buildings)
                if (b.Owner != other.Owner)
                    return false;
            return true;
        }
      /*  internal static void print() {
            Console.Out.WriteLine("count: " + counter);
        }*/
        internal List<Building> Buildings{
            get { return _buildings; }
        }


        /**
         * This method merges the "other" corporate into the current one.
         * after this method is done, other corporate will still exist, but will be empty (so it's better to remove him)
         * */
        internal void takeover(Corporate other) {
            if (other == this)
                return;
         //   Console.Out.WriteLine("merging!");
          //  counter--;
            while (other.Buildings.Count>0)
                other.Buildings.First().joinCorp(this);
        }

        

    }
}
