using System.Collections.Generic;
using Game.Logic.Entities;
using Game;
using System;

namespace Game.Logic
{
    class Grid //TODO - what do we need to do in order to make the grid threadsafe?
    {

        /******************
        Consts
        ****************/
        const int randomPathLength = 1000;
        const int CIV_FLEE_RANGE = 1000;
        delegate Effect CurriedEntityListAdd(UniqueList<Entity> list); //These functions curry a list to an effect which will enter entities into the list
        delegate Effect CurriedDirectionListAdd(List<Direction> list); //These functions curry directions to an effect which will put them in lists
        delegate bool boolPointOperator(Point point); //These functions as a binary question on a Point
        delegate boolPointOperator CurriedPointOperator(Entity entity); //These are the curried version, which curry and entity for the question

        /******************
        members
        ****************/

        private readonly Dictionary<Entity, ExternalEntity> entities;
        private readonly Dictionary<Entity, Area> locations;
        private readonly Entity[,] gameGrid;
        private readonly List<Buffers.BufferEvent> actionsDone;
        private readonly UniqueList<ExternalEntity> visible;
        private readonly UniqueList<Entity> destroyed;
        private readonly byte[,] pathFindingGrid;
        //TODO - add corporations?

        private Pathfinding.PathFinderFast pathfinder;

        /******************
        Constructors
        ****************/     

        internal Grid(Entity[,] grid)
        {
            this.gameGrid = grid;
            this.actionsDone = new List<Buffers.BufferEvent>();
            this.locations = new Dictionary<Entity, Area>();
            this.entities = new Dictionary<Entity, ExternalEntity>();
            this.visible = new UniqueList<ExternalEntity>();
            this.destroyed = new UniqueList<Entity>();
            this.pathFindingGrid = new byte[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if(grid[i,j] == null) this.pathFindingGrid[i,j] =1;
                    else this.pathFindingGrid[i, j] = Convert.ToByte(Math.Min(grid[i,j].Health+1, byte.MaxValue));
                }
            }
            pathfinder = new Pathfinding.PathFinderFast(this.pathFindingGrid);
        }


        internal void initialiseExitPoints()
        {
            foreach (Entity ent in entities.Keys)
            {
                ((Building)ent).ExitPoint = this.getExitPoint(ent);
            }
        }

        private Vector getExitPoint(Entity ent)
        {
            Point center = this.convertToCentralPoint(ent);
            int x = center.X;
            int y = center.Y;
            int i = 1;
            while (true)
            {
                if (x + i < this.gameGrid.GetLength(0) && (this.gameGrid[x + i, y] == null))
                    return new Vector(i, 0);
                if (x - i >= 0 && (this.gameGrid[x - i, y] == null))
                    return new Vector(-i, 0);
                if (y + i < this.gameGrid.GetLength(1) && (this.gameGrid[x, y + i] == null))
                    return new Vector(0, i);
                if (y - i >= 0 && (this.gameGrid[x, y - i] == null))
                    return new Vector(0, -i);
                i++;

            }
        }

        /******************
        Methods
        ****************/ 

        //////////COMMUNICATION LOGIC////////

        internal List<ExternalEntity> getVisibleEntities()
        {
            return this.visible;
        }

        internal List<Entity> getAllRealEntities()
        {
            return new List<Entity>(this.entities.Keys);
        }

        internal void clear()
        {
            this.visible.Clear();
            this.actionsDone.Clear();
            destroyed.Clear();
        }

        internal List<ExternalEntity> getAllEntities()
        {
            return new List<ExternalEntity>(this.entities.Values);
        }

        /*
         * This function returns the list of actions performed in the current round.
         */
        internal List<Buffers.BufferEvent> returnActions()
        {
            foreach (Entity ent in destroyed)
                this.destroy(ent);
            return this.actionsDone;
        }

        /*
         * This function finds the central point of an entity - and entity is a grid, and the central point is that which the
         * shots/sights are coming from, and where other units will aim at.
         */
        private Point convertToCentralPoint(Entity ent)
        {
            Point[,] grid = this.locations[ent].getPointArea();
            int x, y;
            if (grid.GetLength(0) % 2 == 0) x = grid.GetLength(0) / 2; else x = (grid.GetLength(0) - 1) / 2;
            if (grid.GetLength(1) % 2 == 0) y = grid.GetLength(1) / 2; else y = (grid.GetLength(1) - 1) / 2;
            Point ans = grid[x, y];
            return ans;
        } 

        /*
         * This function adds events to be reported to future buffers
         */
        private void addEvent(Buffers.BufferEvent action)
        {
            this.actionsDone.Add(action);
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        internal void whatSees(Entity ent)
        {
            //TODO - look into Sight simply having a list and the given blast, instead of craeating a new list & blast for every iteration
            //Get all the relevant variables
            Point location = this.convertToCentralPoint(ent);
            Sight sight = ent.Sight;
            int radius = sight.Range;
            wasBlocked blocked = sight.Blocked;
            //curries the list of entities to an effect
            CurriedEntityListAdd listAdd = delegate(UniqueList<Entity> list)
            {
                return delegate(Entity entity)
                {
                    if (entity != null) { list.uniqueAdd(entity); }
                };
            };

            Effect effect = listAdd(ent.WhatSees);

            BlastEffect blast = BlastEffect.instance(radius, effect, blocked, ShotType.SIGHT);

            this.areaEffect(ent, blast);

            foreach (Entity temp in ent.WhatSees)
            {
                //TODO - this is wrong. it should only return what the player units see. 
                this.visible.Add(this.entities[temp]);
            }
        }

        //////////MOVEMENT LOGIC//////////

        /*
         * this function resolves a move command from the game logic, based on the reaction set by the entity
         */
        internal void resolveMove(MovingEntity ent)
        {

            Action action = ent.Reaction.ActionChosen;
            switch (action)
            {
                case Action.IGNORE:
                    this.move(ent);
                    break;

                case Action.RUN_AWAY_FROM:
                    Point from = this.convertToCentralPoint(ent.Reaction.Focus);
                    Point currentLocation = this.convertToCentralPoint(ent);
                    Point runTo = getOppositePoint(from, currentLocation, CIV_FLEE_RANGE);
                    ent.Path = getSimplePath(currentLocation, runTo);
                    ((Civilian)ent).runningAway();
                    this.move(ent);
                    break;
                //TODO - missing cases
            }
            
            
        }

        /*
         * This is the basic moving function
         */
        private void move(MovingEntity ent)
        {
            int tries = 0;
            while (ent.Path.Count == 0)
            {
                if (tries == 5) //just a precaution
                {
                    this.destroyed.Add(ent);
                    return;
                }
                Point temp = this.convertToCentralPoint(ent);
                ent.Path = this.getSimplePath(temp, this.generateRandomPoint(temp, randomPathLength));
                tries++;
            }
            Direction dir = ent.getDirection();
            bool result = this.canMove(ent, dir);
            if (result)
            {
                doMove(ent, dir);
            }
            else
            {
                Point temp = this.convertToCentralPoint(ent);
                ent.Path = this.getSimplePath(temp, this.generateRandomPoint(temp, randomPathLength));
            }
            ent.moveResult(result);
        }

        //TODO - problematic, could lead into a building
        private Point generateRandomPoint(Point temp, int distance)
        {
            int maxX = Math.Min(temp.X + distance, this.gameGrid.GetLength(0)-1);
            int minX = Math.Max(0, temp.X-distance);
            int maxY = Math.Min(temp.Y + distance, this.gameGrid.GetLength(1)-1);
            int minY = Math.Max(0, temp.Y-distance);
            return new Point(minX, maxX, minY, maxY);
        }


        /*
         * This function is used mainly to generate the simple walking path for civilians. 
         * Will probably need to make it better in future, it'll probably serve other functions.
         */
        private LinkedList<Direction> getSimplePath(Point entry, Point target)
        {
            LinkedList<Direction> ans = new LinkedList<Direction>();

            ans = Pathfinding.PathFinding.convertToDirection(pathfinder.FindPath(entry, target));
            if (!(ans.Count == entry.getDiffVector(target).length())){
                //TODO - probably continue from there.
            }
            return ans;
        }

        /*
         * a case of Bresenham's line algorithm that returns a list of directions to go in.
         */
        private LinkedList<Direction> processWalkingPath(Point exit, Point target)
        {
            LinkedList<Direction> ans = new LinkedList<Direction>();
            int x0 = exit.X;
            int y0 = exit.Y;
            int x1 = target.X;
            int y1 = target.Y;
            int dx = Vector.abs(x1 - x0);
            int dy = Vector.abs(y1 - y0);
            int sx, sy, e2;
            if (x0 < x1) sx = 1; else sx = -1;
            if (y0 < y1) sy = 1; else sy = -1;
            int err = dx - dy;
            while (!(x0 == x1 & y0 == y1))
            {
                Entity ent = gameGrid[x0, y0];
                if (ent != null && ent.Type == entityType.BUILDING) break;
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                    if (sx > 0)
                    {
                        if (e2 < dx)
                        {
                            err = err + dx;
                            y0 = y0 + sy;
                            if (sy > 0)
                            {
                                ans.AddLast(Direction.DOWNRIGHT);
                            }
                            else
                            {
                                ans.AddLast(Direction.UPRIGHT);
                            }
                        }
                        else ans.AddLast(Direction.RIGHT);
                    }
                    else
                    {
                        if (e2 < dx)
                        {
                            err = err + dx;
                            y0 = y0 + sy;
                            if (sy > 0)
                            {
                                ans.AddLast(Direction.DOWNLEFT);
                            }
                            else
                            {
                                ans.AddLast(Direction.UPLEFT);
                            }
                        }
                        else ans.AddLast(Direction.LEFT);
                    }
                }
                else
                {
                    if (e2 < dx)
                    {
                        err = err + dx;
                        y0 = y0 + sy;
                        if (sy > 0)
                        {
                            ans.AddLast(Direction.DOWN);
                        }
                        else
                        {
                            ans.AddLast(Direction.UP);
                        }
                    }
                }

                if ((y0 < 0) ||(x0 < 0) || (x0 >= this.gameGrid.GetLength(0)) || (y0 >= this.gameGrid.GetLength(1)))
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return ans;
        }

        //TODO - return to private
        public Entity getEntityInPoint(Point point)
        {
            return this.gameGrid[point.X, point.Y];
        }

        /*
         * Checks if a ceratin entity can mone, and whether it needs to flip its axis in order to move in the given direction
         */
        private bool canMove(MovingEntity ent, Direction direction)
        {
            Area location = this.locations[ent];
            if(ent.needFlip())
            {
                location = location.flip();
            }

            Area areaToCheck = new Area(location, this.directionToVector(direction));

            return canMove(ent, areaToCheck);
        }

        /*
         * Checks if an entity can enter a given area - whether each point is free
         */
        private bool canMove(Entity ent, Area area)
        {
            //This delegate is a function that checks if the entity in the point is either null (point empty) or the same entity
            CurriedPointOperator checkEntityInArea = delegate(Entity entity)
            {
                return delegate(Point point)
                {
                    Entity temp = this.getEntityInPoint(point);
                    return (entity == temp|| temp == null);
                };
            };
            return iterateOverArea(area, checkEntityInArea(ent));
        }

        /**
         * This function serves for running away from a certain point - it finds where does the civilian run to.
         */
        private Point getOppositePoint(Point from, Point center, int distance)
        {
            Vector dist = center.getDiffVector(from);
            dist.completeToDistance(distance);
            return new Point(from, dist);
            //TODO - check
        }

        private void doMove(MovingEntity ent, Direction dir)
        {
            removeFromLocation(ent);
            moveToNewLocation(ent, dir);
        }

        /*
         * 
         */
        private void moveToNewLocation(MovingEntity ent, Direction dir)
        {
            int rotation = 0;
            Area location = this.locations[ent];
            if (ent.needFlip())
            {
                location = location.flip();
            }

            Area toSwitch = new Area(location, this.directionToVector(dir));

            CurriedPointOperator putEntityInArea = delegate(Entity entity)
            {
                return delegate(Point point)
                {
                    gameGrid[point.X, point.Y] = entity;
                    return true;
                };
            };

            iterateOverArea(toSwitch, putEntityInArea(ent));
            if (this.entities.ContainsKey(ent))
            {
                ExternalEntity newEnt = this.entities[ent];
                newEnt.Position = new Vector(location.Entry);
                this.addEvent(new Buffers.MoveEvent(toSwitch, newEnt, rotation));
            }
            else
            {
                ExternalEntity newEnt = new ExternalEntity(ent, new Vector(location.Entry));
                this.addEvent(new Buffers.MoveEvent(toSwitch, newEnt, rotation));
                this.entities.Add(ent, newEnt);

            }
            this.locations[ent] = toSwitch;
        }

        private bool removeEntityFromPoint(Point point)
        {
            gameGrid[point.X, point.Y] = null;
            return true;
        }

        private void removeFromLocation(Entity ent)
        {
            Area area = this.locations[ent];
            this.iterateOverArea(area, removeEntityFromPoint);
        }

        private Direction vectorToDirection(Vector vector)
        {
            vector = vector.normalise();
            if (vector.Equals(new Vector(0, -1))) return Direction.UP;
            if (vector.Equals(new Vector(0, 1))) return Direction.DOWN;
            if (vector.Equals(new Vector(-1, 0))) return Direction.LEFT;
            if (vector.Equals(new Vector(1, 0))) return Direction.RIGHT;
            throw new Exception("not valid direction found");
        }

        private Vector directionToVector(Direction direction)
        {

            switch (direction)
            {
                case(Direction.UP):
                    return new Vector(0, -1);

                case(Direction.DOWN):
                    return new Vector(0, 1);

                case(Direction.LEFT):
                    return new Vector(-1, 0);

                case(Direction.RIGHT):
                    return new Vector(1, 0);

                case (Direction.UPRIGHT):
                    return new Vector(1, -1);

                case (Direction.DOWNRIGHT):
                    return new Vector(1, 1);

                case (Direction.UPLEFT):
                    return new Vector(-1, -1);

                case (Direction.DOWNLEFT):
                    return new Vector(-1, 1);
                
                default:
                    throw new Exception("not valid direction found");
            }
        }

        /*
         * This function iterates an operator on every point in an area. 
         */
        private bool iterateOverArea(Area area, boolPointOperator op)
        {
            bool ans = true;
            Point entry = area.Entry;
            for (int i = 0; i < area.Size.X; i++)
            {
                for (int j = 0; j < area.Size.Y; j++)
                {
                    ans = ans & op(new Point(entry, new Vector(i, j)));
                }

            }
            return ans;
        }

        /////////SHOOTING LOGIC//////////


        internal void resolveShoot(Shooter shooter, Entity target)
        {
             shoot(shooter, target);
        }

        internal void areaEffect(Entity ent, BlastEffect blast)
        {
            this.areaEffect(this.convertToCentralPoint(ent), blast);
        }

        /*
         * this function sends a shot effect in every direction - lines to every point in the radius circumference
         */
        private void areaEffect(Point location, BlastEffect blast)
        {
            int radius = blast.Radius;
            int x = location.X;
            int y = location.Y;
            int newX, newY;
            //checks in each of the four cardinal directions;
            //TODO - make sure that entities aren't affected more then once? or just reduce the effects to account for that?
            for (int i = 1; i < radius; i++)
            { 
                newX = Math.Min(x+i, gameGrid.GetLength(0)-1);
                newY = Math.Min(y+radius-i,gameGrid.GetLength(1)-1);
                processPath(location, new Point(newX,newY), blast);
                
                newY = newY = Math.Max(y-radius+i,0);
                processPath(location, new Point(newX,newY), blast);
                
                newX = Math.Max(x-i, 0);
                processPath(location, new Point(newX,newY), blast);
                
                newY = Math.Min(y+radius-i,gameGrid.GetLength(1)-1);
                processPath(location, new Point(newX,newY), blast);
            }
        }

        //This function simulates a single shot
        internal void shoot(Shooter shooter, Entity target)
        {
            if(target.destroyed()) return;
            //get all relevant variables
            Weapon weapon = shooter.weapon();
            Shot shot = weapon.Shot;
            Point exit = this.convertToCentralPoint((Entity)shooter);
            Point currentTargetLocation = this.convertToCentralPoint(target);
            Vector direction = new Vector(exit, currentTargetLocation);
            direction = direction.normalProbability(direction.length() / weapon.Acc);
            direction = direction.completeToDistance(weapon.Range);
            //TODO - If there's a target that I see only parts of it, how do I aim at the visible parts?
            
            //get the path the bullet is going through, and affect targets
            Point endPoint = this.processPath(exit, new Point(exit, direction), shot);

            if (shot.Blast != null){

                this.areaEffect(endPoint, shot.Blast);

            }
        }

        private int getDistance(Point exit, Point target)
        {
            return Convert.ToInt32(Math.Sqrt(Math.Pow(exit.X - target.X, 2) + Math.Pow(exit.Y - target.Y, 2)));  
        }


        /*
         * A simple version of Berensham's line algorithm, that calculates the way of a bullet, and affects every entity in the way
         */
        private Point processPath(Point exit, Point target, Shot shot)
        {
            Effect effect = shot.Effect;
            wasBlocked blocked = shot.Blocked;
            int x0 = exit.X;
            int y0 = exit.Y;
            int x1 = target.X;
            int y1 = target.Y;
            int dx = Vector.abs(x1-x0);
            int dy = Vector.abs(y1-y0);
            int sx, sy, e2;
            if (x0 < x1) sx = 1; else sx = -1;
            if (y0 < y1) sy = 1; else sy = -1;
            int err = dx-dy;
            Entity ent = null;
            while ((!(x0 == x1 & y0 == y1)) && (!shot.Blocked(ent)))
            {
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
                if (x0 >= gameGrid.GetLength(0) || x0 < 0 || y0 < 0 || y0 >= gameGrid.GetLength(1)) 
                    break;
                ent = gameGrid[x0, y0];
                if (ent != null)
                {
                    effect(ent);
                    if (ent.destroyed())
                    {
                        this.destroyed.Add(ent);
                    }
                }
            }
            Point res = new Point(x0, y0);

            if (!(shot.Type == ShotType.SIGHT))
            {
                this.addEvent(new Buffers.ShotEvent(shot.Type, exit, res));

            }
            return res;
        }

        private void destroy(Entity ent)
        {

            if (ent.Type == entityType.BUILDING)
            {
                Area area = this.locations[ent];
                foreach (Point point in area.getPointArea())
                {
                    this.pathFindingGrid[point.X, point.Y] = 1;
                }
            }
            pathfinder = new Pathfinding.PathFinderFast(this.pathFindingGrid);

            this.addEvent(new Buffers.DestroyEvent(this.locations[ent], this.entities[ent]));
            this.removeFromLocation(ent);
            this.locations.Remove(ent);
            this.entities.Remove(ent);
            ent.destroy();

        }

        /**************
         construction logic
         **************/

        /*
         * This function adds an entity to a certain area
         */
        internal void addEntity(Entity ent, Area area)
        {
            //TODO - if (gameGrid[loc.getX, loc.Y] != null) throw new LocationFullException(loc.ToString() + " " + gameGrid[loc.getX, loc.getY].ToString());
            //else
            this.locations.Add(ent, area);
            foreach (Point point in area.getPointArea())
            {
                //if (this.gameGrid[point.X, point.Y] != null) throw new Exception();
                this.gameGrid[point.X, point.Y] = ent;
            }
            ExternalEntity temp = new ExternalEntity(ent, new Vector(area.Entry.X, area.Entry.Y));
            this.entities.Add(ent, temp);
            this.addEvent(new Buffers.CreateEvent(temp, area));
        }

        internal void resolveConstruction(Constructor constructor, MovingEntity entity)
        {
            this.addEntity(entity, findConstructionSpot(constructor, entity));
            //TODO - add the transition of the entity from the center of the building to outside. currently just pops out. 
        }

        private Area findConstructionSpot(Constructor constructor, Entity ent)
        {
            return new Area(new Point(this.convertToCentralPoint((Entity)constructor), constructor.exitPoint()), ent.Size);
        }

    }
}