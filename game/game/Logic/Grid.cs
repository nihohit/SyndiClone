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

        const int CIV_FLEE_RANGE = 100;
        delegate Effect CurriedEntityListAdd(UniqueList<Entity> list); //These functions curry a list to an effect which will enter entities into the list
        delegate Effect CurriedDirectionListAdd(List<Direction> list); //These functions curry directions to an effect which will put them in lists
        delegate bool boolPointOperator(Point point); //These functions as a binary question on a Point
        delegate boolPointOperator CurriedPointOperator(Entity entity); //These are the curried version, which curry and entity for the question

        /******************
        Getters & setters
        ****************/
        
        
        private readonly Dictionary<Entity, Area> locations;
        private readonly Entity[,] gameGrid;
        private readonly List<BufferEvent> actionsDone;
        //TODO - add corporations?

        /******************
        Constructors
        ****************/     

        internal Grid(Entity[,] grid)
        {
            this.gameGrid = grid;
            this.actionsDone = new List<BufferEvent>();
            this.locations = new Dictionary<Entity, Area>(); 
        }

        /******************
        Methods
        ****************/ 

        //////////COMMUNICATION LOGIC////////

        internal Vector getPosition(Entity ent)
        {
            return new Vector(this.locations[ent].Entry);
        }


        /*
         * This function returns the list of actions performed in the current round.
         */
        internal List<BufferEvent> returnActions()
        {
            List<BufferEvent> ans = new List<BufferEvent>(this.actionsDone);
            this.actionsDone.Clear();
            return ans;
        }

        /*
         * This function finds the central point of an entity - and entity is a grid, and the central point is that which the
         * shots/sights are coming from, and where other units will aim at.
         */
        private Point convertToCentralPoint(Entity ent)
        {
            Point[,] grid = this.locations[ent].getPointArea();
            int x, y;
            if (grid.GetLength(0) % 2 == 0) x = grid.GetLength(0) / 2; else x = (grid.GetLength(0) + 1) / 2;
            if (grid.GetLength(1) % 2 == 0) y = grid.GetLength(1) / 2; else y = (grid.GetLength(1) + 1) / 2;
            Point ans = grid[x, y];
            return ans;
        } 

        /*
         * this function adds an entity to the board, from another entity. Basically, a unit constructed from a building.
         */
        internal void addEntity(Entity ent, Entity from, Vector displacement)
        {
            //TODO - convert data & call the next addEntity
        }


        /*
         * This function adds an entity to a certain area
         */
        internal void addEntity(Entity ent, Area area)
        {
            //TODO - missing function
            Point[,] loc = new Point[ent.Size.X, ent.Size.Y];
            //TODO - if (gameGrid[loc.getX, loc.Y] != null) throw new LocationFullException(loc.ToString() + " " + gameGrid[loc.getX, loc.getY].ToString());
            //else
            {

            }
        }

        /*
         * This function adds events to be reported to future buffers
         */
        private void addEvent(BufferEvent action)
        {
            this.actionsDone.Add(action);
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        internal UniqueList<Entity> whatSees(Entity ent)
        {
            //TODO - look into Sight simply having a list and the given blast, instead of craeating a new list & blast for every iteration
            //Get all the relevant variables
            UniqueList<Entity> ans = new UniqueList<Entity>();
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

            Effect effect = listAdd(ans);

            BlastEffect blast = BlastEffect.instance(radius, effect, blocked, ShotType.SIGHT);

            return ans;
        }

        //////////MOVEMENT LOGIC//////////

        /*
         * this function resolves a move command from the game logic, based on the reaction set by the entity
         */
        internal void resolveMove(MovingEntity ent)
        {
            if (ent.needToMove(ent.Speed))
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
                }
            }

            //TODO - missing function
        }

        /*
         * This is the basic moving function
         */
        private void move(MovingEntity ent)
        {
            Direction dir = ent.getDirection();
            bool result = canMove(ent, dir);
            if (result)
            {
                doMove(ent, dir);
            }
            ent.moveResult(result);
        }


        /*
         * This function is used mainly to generate the simple walking path for civilians. 
         * Will probably need to make it better in future, it'll probably serve other functions.
         */
        private LinkedList<Direction> getSimplePath(Point entry, Point target)
        {
            LinkedList<Direction> ans = new LinkedList<Direction>();
            ans = processWalkingPath(entry, target);
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
                if (ent.Type == entityType.BUILDING) break;
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
            }
            return ans;
        }

        private Entity getEntityInPoint(Point point)
        {
            return this.gameGrid[point.X, point.Y];
        }

        /*
         * Checks if a ceratin entity can mone, and whether it needs to flip its axis in order to move in the given direction
         */
        private bool canMove(MovingEntity ent, Direction direction)
        {
            Area areaToCheck = new Area();
            Area location = this.locations[ent];
            if(ent.needFlip())
            {
                location.flip();
            }

            areaToCheck = new Area(location, this.directionToVector(direction));

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
                    return entity == this.getEntityInPoint(point);
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
                location.flip();
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
            this.addEvent(new MoveEvent(toSwitch, new ExternalEntity(ent, new Vector(location.Entry)), rotation));
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
            vector.normalise();
            if (vector.Equals(new Vector(-1, 0))) return Direction.UP;
            if (vector.Equals(new Vector(1, 0))) return Direction.DOWN;
            if (vector.Equals(new Vector(0, -1))) return Direction.LEFT;
            if (vector.Equals(new Vector(0, 1))) return Direction.RIGHT;
            throw new Exception("not valid direction found");
        }

        private Vector directionToVector(Direction direction)
        {

            switch (direction)
            {
                case(Direction.UP):
                    return new Vector(-1, 0);

                case(Direction.DOWN):
                    return new Vector(1, 0);

                case(Direction.LEFT):
                    return new Vector(0, -1);

                case(Direction.RIGHT):
                    return new Vector(0, 1);
                
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
                    //TODO - check if this is the correct way to do AND
                }

            }
            return ans;
        }

        /////////SHOOTING LOGIC//////////


        internal void resolveShoot(Shooter shooter, Entity target)
        {
            if (shooter.readyToShoot())
            {
                shoot(shooter, target);
            }
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
            for (int i = 0; i < radius; i++)
            {
                newX = Math.Min(x+i, gameGrid.GetLength(0));
                newY = Math.Min(y+radius-i,gameGrid.GetLength(1));
                processPath(location, new Point(newX,newY), blast);

                newY = newY = Math.Max(y-radius+i,0);
                processPath(location, new Point(newX,newY), blast);

                newX = Math.Max(x-i, 0);
                processPath(location, new Point(newX,newY), blast);

                newY = Math.Min(y+radius-i,gameGrid.GetLength(1));
                processPath(location, new Point(newX,newY), blast);
            }
        }

        //This function simulates a single shot
        internal void shoot(Shooter shooter, Entity target)
        {
            //get all relevant variables
            Shot shot = shooter.weapon().Shot;
            Point currentTargetLocation = shooter.hitFunc()(this.convertToCentralPoint(target));
            //TODO - If there's a target that I see only parts of it, how do I aim at the visible parts?
            Point exit = this.convertToCentralPoint((Entity)shooter);


            //get the path the bullet is going through, and affect targets
            Point endPoint = this.processPath(exit, currentTargetLocation, shot);

            if (shot.Blast != null){

                this.areaEffect(endPoint, shot.Blast);

            }
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
            while (!(x0 == x1 & y0 == y1))
            {
                Entity ent = gameGrid[x0, y0];
                effect(ent);
                if (ent.destroyed())
                {
                    this.destroy(ent);
                }
                if (blocked(ent.Visible)) break;
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
            }
            Point res = new Point(x0, y0);

            if (!(shot.Type == ShotType.SIGHT))
            {
                this.addEvent(new ShotEvent(shot.Type, exit, res));
            }

            return res;
        }

        private void destroy(Entity ent)
        {
            this.addEvent(new DestroyEvent(this.locations[ent], ent));
            locations.Remove(ent);
            this.removeFromLocation(ent);
        }

    }
}