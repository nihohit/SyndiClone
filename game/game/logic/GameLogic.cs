using Game.Logic.Entities;
using System.Collections.Generic;

namespace Game.Logic
{
    class UniqueList<T> : List<T>
    {
        public UniqueList() : base()
        {
        }

        public void UniqueAdd(T obj)
        {
            if (!base.Contains(obj)) base.Add(obj);
        }
    }

    class GameLogic
    {
        private readonly UniqueList<Entity> activeEntities;
        private readonly UniqueList<MovingEntity> movers;
        private readonly UniqueList<Entity> playerUnits;
        private readonly UniqueList<Entity> alwaysActive;
        private readonly Grid grid;

        public GameLogic()
        {
            activeEntities = new UniqueList<Entity>(); 
            movers = new UniqueList<MovingEntity>();
            alwaysActive = new UniqueList<Entity>(); 
            playerUnits = new UniqueList<Entity>(); 
        }

        public void loop()
        {
            this.listAdd();


            activeEntities.Clear();
        }

        private void listAdd()
        {
            foreach (Entity t in playerUnits)
            {
                activeEntities.UniqueAdd(t);
                foreach (Entity temp in grid.whatSees(t))
                {
                    activeEntities.UniqueAdd(temp);
                }
            }

            foreach (Entity t in activeEntities)
            {
                if (t is MovingEntity) movers.UniqueAdd((MovingEntity)t);
            }
        }




    }
}
