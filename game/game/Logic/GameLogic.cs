using Game.Logic.Entities;
using Game.Buffers;
using System.Collections.Generic;
using System;

namespace Game.Logic
{

    class GameLogic
    {

        /******************
        Class members
        ****************/

        private List<Entity> activeEntities; //TODO - readonly uniquelist?
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
        private bool active;
        private bool gameRunning;

        System.Diagnostics.Stopwatch synch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch move = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch shoot = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch construct = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch other = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch orders = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sight = new System.Diagnostics.Stopwatch();
        /******************
        Constructors
        ****************/

        public void run()
        {
            while (gameRunning)
            {
                //Console.Out.WriteLine("logic loop");
                loop();
            }
        }

        public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.GameBoard city, int _civAmount)
        {
            this.activeEntities = new UniqueList<Entity>(); 
            this.movers = new UniqueList<MovingEntity>();
            
            this.playerUnits = new UniqueList<Entity>();
            this.shooters = new UniqueList<Shooter>();
            this.constructors = new UniqueList<Constructor>();
            this.displayBuffer = disp;
            this.inputBuffer = input;
            this.soundBuffer = sound;
            this.civAmountGoal = _civAmount;
            this.civAmount = 0;
            this._grid = GameBoardToGameGridConverter.convert(city);
            this.alwaysActive = new UniqueList<Entity>(this._grid.getAllRealEntities());
            this.active = true;
            this.gameRunning = true;
        }

        /******************
        Methods
        ****************/

        /*
         * This is the main loop of the logic
         */
        public void loop()
        {
            this.synch.Start();
            this.handleInput();
            this.synch.Stop();
            if (!gameRunning) Console.Out.WriteLine("other was " + other.Elapsed + " orders time was " + orders.Elapsed + " sight time was " + sight.Elapsed);
            if (active)
            {
                this.other.Start();
                this.clearData();
                this.populateActionLists();
                this.resolveOrders();
                this.other.Stop();
                this.move.Start();
                this.handleMovement();
                this.move.Stop();
                this.shoot.Start();
                this.handleShooting();
                this.shoot.Stop();
                this.construct.Start();
                this.handleUnitCreation();
                this.construct.Stop();
                this.synch.Start();
                this.updateOutput();
                this.synch.Stop();
            }
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
            this._grid.clear();
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
                        if ((((DestroyEvent)action).Ent.Type == entityType.PERSON) && ((DestroyEvent)action).Ent.Loyalty == Affiliation.CIVILIAN)
                        {
                            this.civAmount--;
                        }
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
                //List<ExternalEntity> newList = this._grid.getVisibleEntities();
                List<ExternalEntity> newList = new List<ExternalEntity>(this._grid.getAllEntities());
                List<BufferEvent> actionList = new List<BufferEvent>(actions);
                displayBuffer.receiveVisibleEntities(newList);
                displayBuffer.receiveActions(actionList);
                displayBuffer.Updated = true;
            }
            
        }

        private void handleInput()
        {
            lock (inputBuffer)
            {
                if (inputBuffer.LogicInput)
                {
                    List<BufferEvent> events = inputBuffer.logicEvents();
                    foreach (BufferEvent action in events)
                    {
                        switch (action.type())
                        {
                            case BufferType.PAUSE:
                                this.active = false;
                                break;
                            case BufferType.UNPAUSE:
                                this.active = true;
                                break;
                            case BufferType.ENDGAME:
                                this.active = false;
                                this.gameRunning = false;
                                break;
                            case BufferType.SELECT:
                                Entity temp = this._grid.getEntityInPoint(((BufferMouseSelectEvent)action).Coords.toPoint());
                                if (temp != null)
                                    Console.Out.WriteLine(temp.ToString());
                                break;
                        }
                    }
                }
            }
            
        }

        /*
         * This function iterates over every active entity, checks if they need to act, and if they do, finds their new reaction.
         */
        private void resolveOrders()
        {
            foreach (Entity ent in activeEntities)
            {
                this.orders.Start();
                if (ent.doesReact())
                {
                    if (ent.WhatSees == null)
                    {
                        this.orders.Stop();
                        this.sight.Start();
                        this._grid.whatSees(ent);
                        this.sight.Stop();
                    }
                    this.orders.Start();
                    ent.resolveOrders();
                    ent.WhatSees = null;
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
                    if (temp.readyToConstruct()) //TODO - for structures, to make sure they update their building order. other solution?
                    {
                        this.constructors.Add(temp);
                    }
                }
                this.orders.Stop();
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
        private void populateActionLists()
        {
            /*
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
             */
            activeEntities = this._grid.getAllRealEntities();
            
        }

        //TODO - add blocks, add the whole player logic, add research

    }
}
