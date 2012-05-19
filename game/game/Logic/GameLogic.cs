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
        const int FRAMES_PER_SECOND = 120; //determines the amount of repeats the system can have in a second
        const int MIN_MILLISECONDS_PER_FRAME = 1000 / FRAMES_PER_SECOND;

        private List<Entity> activeEntities = new List<Entity>(); //TODO - readonly uniquelist?
        private readonly UniqueList<MovingEntity> movers = new UniqueList<MovingEntity>();
        private readonly UniqueList<Entity> playerUnits = new UniqueList<Entity>();
        private readonly UniqueList<Shooter> shooters = new UniqueList<Shooter>();
        private readonly UniqueList<Constructor> constructors = new UniqueList<Constructor>();
        private readonly UniqueList<Entity> alwaysActive;

        private readonly int civAmountGoal;
        private int civAmount;
        private readonly Grid _grid;
        private readonly DisplayBuffer displayBuffer;
        private readonly InputBuffer inputBuffer;
        private readonly SoundBuffer soundBuffer;
        private bool active;
        private bool gameRunning;
        System.Diagnostics.Stopwatch frameTester = new System.Diagnostics.Stopwatch();

        int runs = 0;
        System.Diagnostics.Stopwatch synch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch move = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch shoot = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch construct = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch other = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch orders = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sight = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch totalWatch = new System.Diagnostics.Stopwatch();
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
            this.displayBuffer = disp;
            this.inputBuffer = input;
            this.soundBuffer = sound;
            this.civAmountGoal = _civAmount;
            this.civAmount = 0;
            this._grid = GameBoardToGameGridConverter.convert(city);
            this.alwaysActive = new UniqueList<Entity>(this._grid.getAllRealEntities());
            this.active = true;
            this.gameRunning = true;
            this.totalWatch.Start();
            this.frameTester.Start();
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
            if (!gameRunning)
            {
                this.totalWatch.Stop();
                Console.Out.WriteLine("synch was " + synch.Elapsed);
                Console.Out.WriteLine("move was " + other.Elapsed);
                Console.Out.WriteLine("shoot was " + shoot.Elapsed);
                Console.Out.WriteLine("construct was " + construct.Elapsed);
                Console.Out.WriteLine("orders was " + orders.Elapsed);
                Console.Out.WriteLine("sight was " + sight.Elapsed);
                Console.Out.WriteLine("amount of graphic loops: " + runs + " average milliseconds per frame: " + this.totalWatch.ElapsedMilliseconds / runs);
            }
            if (active)
            {
                this.other.Start();

                    
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

                this.clearData();
            }
            this.frameLimit();
            runs++;
        }

        private void frameLimit()
        {
            while (frameTester.ElapsedMilliseconds < MIN_MILLISECONDS_PER_FRAME) { }
            this.frameTester.Restart();
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
                //List<ExternalEntity> newPath = this._grid.getVisibleEntities();
                List<ExternalEntity> newList = new List<ExternalEntity>(this._grid.getAllEntities());
                displayBuffer.receiveVisibleEntities(newList);
                if (actions.Count > 0)
                {
                    List<BufferEvent> actionList = new List<BufferEvent>(actions);
                    displayBuffer.receiveActions(actionList);
                }
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
                                this._grid.select(((BufferMouseSelectEvent)action).Coords.toPoint());
                                break;
                            case BufferType.DESELECT:
                                this._grid.deselect();
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
                    if (ent.WhatSees.Count == 0)
                    {
                        this.orders.Stop();
                        this.sight.Start();
                        this._grid.whatSees(ent);
                        this.sight.Stop();
                    }
                    this.orders.Start();
                    ent.resolveOrders();
                    ent.WhatSees.Clear();
                }
                Reaction react = ent.Reaction;
                Action action = react.action();

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

                if (action == Action.CONSTRUCT_ENTITY)
                {
                    Constructor temp = (Constructor)ent;
                    bool check = temp.readyToConstruct();
                    if (check) //TODO - for structures, to make sure they update their building order. other solution?
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
