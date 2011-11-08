using Game.Logic.Entities;
using Game.Buffers;
using System.Collections.Generic;

namespace Game.Logic
{

    class GameLogic
    {
        private readonly UniqueList<Entity> activeEntities;
        private readonly UniqueList<MovingEntity> movers;
        private readonly UniqueList<Entity> playerUnits;
        private readonly UniqueList<Shooter> shooters;
        private readonly UniqueList<Entity> alwaysActive;

        private readonly Grid _grid;
        private readonly DisplayBuffer displayBuffer;
        private readonly InputBuffer inputBuffer;
        private readonly SoundBuffer soundBuffer;

        public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.City city)
        {
            this.activeEntities = new UniqueList<Entity>(); 
            this.movers = new UniqueList<MovingEntity>();
            this.alwaysActive = new UniqueList<Entity>(); 
            this.playerUnits = new UniqueList<Entity>();
            this.shooters = new UniqueList<Shooter>();
            this.displayBuffer = disp;
            this.inputBuffer = input;
            this.soundBuffer = sound;
            City _city = new City(city);
            //TODO - create the grid;
        }

        public void loop()
        {
            this.handleInput();
            this.listAdd();
            this.resolveOrders();
            this.handleMovement();
            this.handleShooting();
            this.handleBuilding();
            this.updateOutput();
            this.clearData();
        }

        private void handleBuilding()
        {
            //TODO - missing function
        }

        private void clearData()
        {
            this.shooters.Clear();
            this.movers.Clear();
            this.activeEntities.Clear();
        }

        private void updateOutput()
        {
            List<BufferEvent> actions = _grid.returnActions();
            foreach (BufferEvent action in actions)
            {
                if (action.type() == BufferType.DESTROY)
                {
                    this.alwaysActive.Remove(((DestroyEvent)action).Ent);
                    this.playerUnits.Remove(((DestroyEvent)action).Ent);
                }
            }
            //TODO - missing function
        }

        private void handleInput()
        {
            //TODO - missing function
        }

        private void resolveOrders()
        {
            foreach (Entity ent in activeEntities)
            {
                if (ent.doesReact())
                {
                    ent.resolveOrders();
                }
                Reaction react = ent.Reaction;
                Action action = react.ActionChosen;

                if (action == Action.FIRE_AT || action == Action.MOVE_WHILE_SHOOT)
                {
                    this.shooters.Add((Shooter)ent);
                }

                if ( action == Action.MOVE_TOWARDS || action == Action.MOVE_WHILE_SHOOT || action == Action.RUN_AWAY_FROM || 
                    (action == Action.IGNORE && ent.Type != entityType.BUILDING))
                {
                    this.movers.Add((MovingEntity)ent);
                }

                if (action == Action.CREATE_ENTITY)
                {
                    //TODO - missing function
                }
            }
        }

        private void handleMovement()
        {
            foreach (MovingEntity ent in movers)
            {
                _grid.resolveMove(ent);
            }
        }

        private void handleShooting()
        {
            foreach (Shooter ent in shooters)
            {
                _grid.resolveShoot(ent, ent.target());
            }
        }

        private void listAdd()
        {
            activeEntities.listAdd(alwaysActive);
            foreach (Entity t in playerUnits)
            {
                activeEntities.uniqueAdd(t);
                t.WhatSees = _grid.whatSees(t);
                foreach (Entity temp in t.WhatSees)
                {
                    activeEntities.uniqueAdd(temp);
                }
            }
        }

        //TODO - add blocks, add the whole palyer logic, add research


    }
}
