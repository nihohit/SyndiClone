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

        private readonly Grid grid;
        private readonly DisplayBuffer _disp;
        private readonly InputBuffer _input;
        private readonly SoundBuffer _sound;

        public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.City city)
        {
            this.activeEntities = new UniqueList<Entity>(); 
            this.movers = new UniqueList<MovingEntity>();
            this.alwaysActive = new UniqueList<Entity>(); 
            this.playerUnits = new UniqueList<Entity>();
            this.shooters = new UniqueList<Shooter>();
            this._disp = disp;
            this._input = input;
            this._sound = sound;
            City _city = new City(city);
            //TODO - create the grid;
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
            this.clearData();
        }

        private void clearData()
        {
            this.shooters.Clear();
            this.movers.Clear();
            this.activeEntities.Clear();
        }

        private void updateOutput()
        {
            List<BufferEvent> actions = grid.receiveActions();
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
                    //TODO
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

        //TODO - add blocks, add the whole palyer logic, add research


    }
}
