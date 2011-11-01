using Game.Logic.Entities;
using System.Collections.Generic;

namespace Game.Logic
{
    class City
    {
        private readonly List<Entity> _buildings;

        private readonly Point[,] _grid;
        

        public City (Game.City_Generator.City city)
        {
            //TODO - missing function
        }

        public Point[,] Grid
        {
            get { return _grid; }
        }

        internal List<Entity> Buildings
        {
            get { return _buildings; }
        } 
    }
}
