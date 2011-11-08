using System.Collections.Generic;
using Game.Logic.Entities;
using Game;
using System;

namespace Game.Logic
{
    class Grid
    {
        //TODO - what do we need to do in order to make the grid threadsafe?
        private readonly Dictionary<Entity, Area> locations;
        private readonly Entity[,] gameGrid;
        private readonly List<BufferEvent> actionsDone;
        //TODO - add corporations?
        const int CIV_FLEE_RANGE = 100;
        
        delegate Effect CurriedEntityListAdd(UniqueList<Entity> list);
        delegate Effect CurriedDirectionListAdd(List<Direction> list);
        delegate bool boolPointOperator(Point point);
        delegate boolPointOperator CurriedPointOperator(Entity entity);
        

        internal Grid(Entity[,] grid)
        {
            this.gameGrid = grid;
            this.actionsDone = new List<BufferEvent>();
            this.locations = new Dictionary<Entity, Area>(); 
        }

        //////////COMMUNICATION LOGIC////////

        internal List<BufferEvent> returnActions()
        {
            List<BufferEvent> ans = new List<BufferEvent>(this.actionsDone);
            this.actionsDone.Clear();
            return ans;
        }

        private Point convertToCentralPoint(Entity ent)
        {
            Point[,] grid = this.locations[ent].getPointArea();
            int x, y;
            if (grid.GetLength(0) % 2 == 0) x = grid.GetLength(0) / 2; else x = (grid.GetLength(0) + 1) / 2;
            if (grid.GetLength(1) % 2 == 0) y = grid.GetLength(1) / 2; else y = (grid.GetLength(1) + 1) / 2;
            Point ans = grid[x, y];
            return ans;
        } 

        internal void addEntity(Entity ent, Entity from, Vector displacement)
        {
            //TODO - convert data & call the next addEntity
        }

        internal void addEntity(Entity ent, Area area)
        {
            //TODO - missing function
            Point[,] loc = new Point[ent.Size.X, ent.Size.Y];
            //TODO - if (gameGrid[loc.getX, loc.Y] != null) throw new LocationFullException(loc.ToString() + " " + gameGrid[loc.getX, loc.getY].ToString());
            //else
            {

            }
        }

        private void addEvent(BufferEvent action)
        {
            this.actionsDone.Add(action);
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        internal UniqueList<Entity> whatSees(Entity ent)
        {

            //Get all the relevant variables
            UniqueList<Entity> ans = new UniqueList<Entity>();
            Point location = this.convertToCentralPoint(ent);
            Sight sight = ent.Sight;
            int radius = sight.Range;
            wasBlocked blocked = sight.Blocked;

            CurriedEntityListAdd listAdd = delegate(UniqueList<Entity> list)
            {
                return delegate(Entity entity)
                {
                    if (entity != null) { list.uniqueAdd(entity); }
                };
            };

            Effect effect = listAdd(ans);

            BlastEffect blast = new BlastEffect(radius, effect, blocked, TypesOfShot.SIGHT);

            return ans;
        }

        //////////MOVEMENT LOGIC//////////

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

        private LinkedList<Direction> getSimplePath(Point entry, Point target)
        {
            LinkedList<Direction> ans = new LinkedList<Direction>();
            ans = processWalkingPath(entry, target);
            if (!(ans.Count == entry.getDiffVector(target).length())){
                //TODO - probably continue from there.
            }
            return ans;
        }

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

        private Direction vectorToDirection(Vector vector)
        {
            vector.normalise();
            if (vector.Equals(new Vector(-1, 0))) return Direction.UP;
            if (vector.Equals(new Vector(1, 0))) return Direction.DOWN;
            if (vector.Equals(new Vector(0, -1))) return Direction.LEFT;
            if (vector.Equals(new Vector(0, 1))) return Direction.RIGHT;
            throw new Exception("not valid direction found");
        }

        private Entity getEntityInPoint(Point point)
        {
            return this.gameGrid[point.X, point.Y];
        }

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

        private bool canMove(Entity ent, Area area)
        {
            CurriedPointOperator checkEntityInArea = delegate(Entity entity)
            {
                return delegate(Point point)
                {
                    return entity == this.getEntityInPoint(point);
                };
            };
            return iterateOverArea(area, checkEntityInArea(ent));
        }

        internal void resolveMove(MovingEntity ent)
        {
            Action action = ent.Reaction.ActionChosen;
            switch (action)
            {
                case Action.IGNORE:
                    if (ent.needToMove(ent.Speed))
                    {
                        this.move(ent);  
                    }
                    break;

                case Action.RUN_AWAY_FROM:
                    Point from = this.convertToCentralPoint(ent.Reaction.Focus);
                    Point currentLocation = this.convertToCentralPoint(ent);
                    Point runTo = getOppositePoint(from, currentLocation, CIV_FLEE_RANGE);
                    ent.Path = getSimplePath(currentLocation, runTo);
                    this.move(ent);
                    break;
            }

            //TODO - missing function
        }

        private Point getOppositePoint(Point from, Point center, int distance)
        {
            Vector dist = center.getDiffVector(from);
            dist.completeToDistance(distance);
            return new Point(from, dist);
            //TODO - check
        }

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

        private void doMove(MovingEntity ent, Direction dir)
        {
            removeFromLocation(ent);
            moveToNewLocation(ent, dir);
        }

        private void moveToNewLocation(MovingEntity ent, Direction dir)
        {
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
            this.addEvent(new MoveEvent(this.locations[ent], toSwitch, ent));
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

        /////////SHOOTING LOGIC//////////


        internal void resolveShoot(Shooter shooter, Entity target)
        {
            if (shooter.readyToShoot())
            {
                solveShot(shooter, target);
            }
        }

        internal void areaEffect(Entity ent, BlastEffect blast)
        {
            this.areaEffect(this.convertToCentralPoint(ent), blast);
        }

        private void areaEffect(Point location, BlastEffect blast)
        {
            int radius = blast.Radius;
            int x = location.X;
            int y = location.Y;
            int newX, newY;
            //checks in each of the four cardinal directions;
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
        internal void solveShot(Shooter shooter, Entity target)
        {
            //get all relevant variables
            ShotType shot = shooter.weapon().Shot;
            Point currentTargetLocation = shooter.hitFunc()(this.convertToCentralPoint(target));
            //TODO - If there's a target that I see only parts of it, how do I aim at the visible parts?
            Point exit = this.convertToCentralPoint((Entity)shooter);


            //get the path the bullet is going through, and affect targets
            Point endPoint = this.processPath(exit, currentTargetLocation, shot);

            if (shot.Blast != null){

                this.areaEffect(endPoint, shot.Blast);

            }
        }

        private Point processPath(Point exit, Point target, ShotType shot)
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

            if (!(shot.Type == TypesOfShot.SIGHT))
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
