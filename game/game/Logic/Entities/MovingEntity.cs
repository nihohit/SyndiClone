using System.Collections.Generic;
namespace Game.Logic.Entities
{
    abstract class MovingEntity : Entity
    {
        protected readonly int speed;

        public int Speed
        {
            get { return speed; }
        } 

        static int update = 100;
        protected LinkedList<Point> path;

        internal LinkedList<Point> Path
        {
            get { return path; }
            set { path = value; }
        }
        

    }
}
