using Game.Logic.Entities;
using Game.Buffers;
using System.Collections.Generic;
using System;

namespace Game.Logic
{

    class GameLogic
    {

        /******************
        Class Fields
        ****************/

        private readonly UniqueList<Entity> activeEntities;
        private readonly UniqueList<MovingEntity> movers;
        private readonly UniqueList<Entity> playerUnits;
        private readonly UniqueList<Shooter> shooters;
        private readonly UniqueList<Constructor> constructors;
        private readonly UniqueList<Entity> alwaysActive;

        private readonly int civAmountGoal;
        private int civAmount;
        private readonly Grid _grid;
        private readonly DisplayBuffer displayBuffer;
        private readonly InputBuffer inputBuffer;
        private readonly SoundBuffer soundBuffer;

        /******************
        Constructors
        ****************/

        public void run()
        {
            while (true)
            {
                Console.Out.WriteLine("logic loop");
                loop();
            }
        }

        public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.GameBoard city, int _civAmount)
        {
            this.activeEntities = new UniqueList<Entity>(); 
            this.movers = new UniqueList<MovingEntity>();
            this.alwaysActive = new UniqueList<Entity>(); 
            this.playerUnits = new UniqueList<Entity>();
            this.shooters = new UniqueList<Shooter>();
            this.constructors = new UniqueList<Constructor>();
            this.displayBuffer = disp;
            this.inputBuffer = input;
            this.soundBuffer = sound;
            this.civAmountGoal = _civAmount;
            this.civAmount = 0;
            this._grid = GameBoardToGameGridConverter.convert(city);
        }

        /******************
        Methods
        ****************/

        /*
         * This is the main loop of the logic
         */
        public void loop()
        {
            this.handleInput();
            this.listAdd();
            this.resolveOrders();
            this.handleMovement();
            this.handleShooting();
            this.handleUnitCreation();
            this.updateOutput();
            this.clearData();
        }

        private void handleUnitCreation()
        {
            int civAmountToCreate = this.civAmountGoal - this.civAmount;
            foreach(Constructor constructor in constructors)
            {
                switch (((Entity)constructor).Loyalty)
                {
                    case (Affiliation.CIVILIAN):
                        if (civAmountToCreate > 0)
                        {
                            this._grid.resolveConstruction(constructor, constructor.getConstruct());
                            civAmountToCreate--;
                            this.civAmount++;
                        }
                        break;
                    default:
                        this._grid.resolveConstruction(constructor, constructor.getConstruct());
                        break;

                }
            }
        }

        private void clearData()
        {
            this.shooters.Clear();
            this.movers.Clear();
            this.constructors.Clear();
            this.activeEntities.Clear();
            this._grid.clearVisibleEntities();
        }

        private void updateOutput()
        {
            List<BufferEvent> actions = _grid.returnActions();
            foreach (BufferEvent action in actions)
            {
                switch(action.type())
                {
                    case(BufferType.DESTROY):
                        this.alwaysActive.Remove(((DestroyEvent)action).Ent.Ent);
                        this.playerUnits.Remove(((DestroyEvent)action).Ent.Ent);
                        break;
                    case(BufferType.CREATE):
                        //TODO - recreate
                        this.alwaysActive.Add(((CreateEvent)action).Mover.Ent);
                        break;
                    //TODO - missing cases?
                }


            }
            //TODO - try smarter threading, with waiting only a limited time on entering. 
            lock (displayBuffer)
            {
                List<ExternalEntity> newList = this._grid.getVisibleEntities();
                displayBuffer.receiveVisibleEntities(this._grid.getAllEntities());
                displayBuffer.receiveActions(actions);
                //TODO - missing function
            }
            soundBuffer.getLock();

            soundBuffer.releaseLock();
            
        }

        private void handleInput()
        {
            lock (inputBuffer)
            {
                //TODO - missing function
            }
        }

        /*
         * This function iterates over every active entity, checks if they need to act, and if they do, finds their new reaction.
         */
        private void resolveOrders()
        {
            foreach (Entity ent in activeEntities)
            {
                bool currentlyActed = false;
                if (ent.doesReact())
                {
                    currentlyActed = true;
                    if (ent.WhatSees == null)
                    {
                        this._grid.whatSees(ent);
                    }
                    ent.resolveOrders();
                }
                Reaction react = ent.Reaction;
                Action action = react.ActionChosen;

                if (action == Action.FIRE_AT || action == Action.MOVE_WHILE_SHOOT)
                {
                    Shooter temp = (Shooter)ent;
                    if (temp.readyToShoot())
                    {
                        this.shooters.Add(temp);
                    } 
                }

                if (action == Action.MOVE_TOWARDS || action == Action.MOVE_WHILE_SHOOT || action == Action.RUN_AWAY_FROM || 
                    (action == Action.IGNORE && ent.Type != entityType.BUILDING))
                {
                    MovingEntity temp = (MovingEntity)ent;
                    if (temp.ReadyToMove(temp.Speed))
                    {
                        this.movers.Add(temp);
                    }
                }

                if (action == Action.CREATE_ENTITY)
                {
                    Constructor temp = (Constructor)ent;
                    if (temp.readyToConstruct() && currentlyActed) //TODO - for structures, to make sure they update their building order. other solution?
                    {
                        this.constructors.Add(temp);
                    }
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

        /*
         * This function populates the active entities for this round, by the logic of - all player units, and every entity they see 
         */
        private void listAdd()
        {
            activeEntities.listAdd(alwaysActive);
            foreach (Entity t in playerUnits)
            {
                activeEntities.uniqueAdd(t);
                _grid.whatSees(t);
                foreach (Entity temp in t.WhatSees)
                {
                    activeEntities.uniqueAdd(temp);
                }
            }
        }

        //TODO - add blocks, add the whole player logic, add research

    }
}
