using Game.Logic.Entities;
using System.Collections.Generic;

namespace Game.Logic
{
    class UniqueList<T> : List<T>
    {
        public UniqueList() : base()
        {
        }

        public void uniqueAdd(T obj)
        {
            if (!base.Contains(obj)) base.Add(obj);
        }

        public void listAdd(UniqueList<T> list)
        {
            foreach (T t in list)
            {
                this.uniqueAdd(t);
            }
        }

    }

    class GameLogic
    {
        private readonly UniqueList<Entity> activeEntities;
        private readonly UniqueList<MovingEntity> movers;
        private readonly UniqueList<Entity> playerUnits;
        private readonly UniqueList<Shooter> shooters;
        private readonly UniqueList<Entity> alwaysActive;
        private readonly Grid grid;
        private readonly DisplayBuffer dips;
        private readonly InputBuffer input;
        private readonly SoundBuffer sound;

        public GameLogic()
        {
            activeEntities = new UniqueList<Entity>(); 
            movers = new UniqueList<MovingEntity>();
            alwaysActive = new UniqueList<Entity>(); 
            playerUnits = new UniqueList<Entity>(); 
        }

        public void loop()
        {
            this.handleInput();
            this.listAdd();
            //TODO - how do I actually resolve each entitiy's orders?
            this.resolveOrders();
            this.handleMovement();
            this.handleShooting();


            this.updateOutput();
            movers.Clear();
            activeEntities.Clear();
        }

        private void updateOutput()
        {
            //TODO
        }

        private void handleInput()
        {
            //TODO
        }

        private void resolveOrders()
        {
            foreach (Entity ent in activeEntities)
            {
                Reaction react = ent.resolveOrders();
                Action action = react.ActionChosen;
                if (action == Action.FIRE_AT || action == Action.MOVE_WHILE_SHOOT)
                {
                    this.shooters.Add((Shooter)ent);
                }
                if (action != Action.FIRE_AT && ent.Type != entityType.BUILDING)
                {
                    this.movers.Add((MovingEntity)ent);
                }
            }
        }

        private void handleMovement()
        {
            foreach (MovingEntity ent in movers)
            {
                grid.resolveMove(ent);
            }
        }

        private void handleShooting()
        {
            foreach (Shooter ent in shooters)
            {
                grid.resolveShoot(ent, ent.target());
            }
        }

        private void listAdd()
        {
            activeEntities.listAdd(alwaysActive);
            foreach (Entity t in playerUnits)
            {
                activeEntities.uniqueAdd(t);
                t.WhatSees = grid.whatSees(t);
                foreach (Entity temp in t.WhatSees)
                {
                    activeEntities.uniqueAdd(temp);
                }
            }
        }




    }
}
