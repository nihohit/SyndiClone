using Game.Logic.Entities;
using Game.Buffers;
using System.Collections.Generic;

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
        private readonly UniqueList<Entity> alwaysActive;

        private readonly Grid _grid;
        private readonly DisplayBuffer displayBuffer;
        private readonly InputBuffer inputBuffer;
        private readonly SoundBuffer soundBuffer;

        /******************
        Constructors
        ****************/

        public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.GameBoard city)
        {
            this.activeEntities = new UniqueList<Entity>(); 
            this.movers = new UniqueList<MovingEntity>();
            this.alwaysActive = new UniqueList<Entity>(); 
            this.playerUnits = new UniqueList<Entity>();
            this.shooters = new UniqueList<Shooter>();
            this.displayBuffer = disp;
            this.inputBuffer = input;
            this.soundBuffer = sound;
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
                    this.alwaysActive.Remove(((DestroyEvent)action).Ent.Ent);
                    this.playerUnits.Remove(((DestroyEvent)action).Ent.Ent);
                }
            }
            //TODO - try smarter threading, with waiting only a limited time on entering. 
            displayBuffer.getLock();
            List<ExternalEntity> newList = new List<ExternalEntity>();
            foreach(Entity ent in activeEntities)
            {
                newList.Add(new ExternalEntity(ent, this._grid.getPosition(ent)));
            }
            displayBuffer.receiveVisibleEntities(newList);
            displayBuffer.receiveActions(actions);
            
            //TODO - missing function
            displayBuffer.releaseLock();
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

        /*
         * This function populates the active entities for this round, by the logic of - all player units, and every entity they see 
         */
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
