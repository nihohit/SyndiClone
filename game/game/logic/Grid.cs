﻿using System.Collections.Generic;
using Game.Logic.Entities;
using Game;
using System;

namespace Game.Logic
{
    public class LocationFullException : System.ApplicationException
    {
        public LocationFullException() { }
        public LocationFullException(string message) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected LocationFullException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }


    class Grid
    {
        private readonly Dictionary<Entity, Point[,]> locations;
        private readonly Entity[,] gameGrid;
        delegate Effect CurriedListAdd(List<Entity> list);

        private Point convertToCentralPoint(Entity ent, Point[,] grid)
        {
            int x,y;
            if (grid.GetLength(0) % 2 == 0) x = grid.GetLength(0) / 2; else x = (grid.GetLength(0) + 1) / 2;
            if (grid.GetLength(1) % 2 == 0) y = grid.GetLength(1) / 2; else y = (grid.GetLength(1) + 1) / 2;
            Point ans = grid[x, y];
            return ans;
        }

        public Grid(int x, int y)
        {
            locations = new Dictionary<Entity, Point[,]>(); /*HACK ans 
                                                             * (amit): just "entity" is enough? not, maybe, "background+entity" ? 
                                                             * why?
                                                             * */
            gameGrid = new Entity[x,y];
            for (int i = 0 ; i < x; i++){
                for (int j =0 ; j < y ; j++){
                    gameGrid[i,j] = null;
                }
            }
        }

        public void addEntity(Entity ent, Entity from, Vector displacement)
        {
            Point[,] loc = new Point[ent.Size.X, ent.Size.Y];
            //TODO - if (gameGrid[loc.getX, loc.Y] != null) throw new LocationFullException(loc.ToString() + " " + gameGrid[loc.getX, loc.getY].ToString());
            //else
            {
                
            }
        }

        private LinkedList<Point> getSimplePath(Point entry, Point target)
        {
            LinkedList<Point> ans = new LinkedList<Point>();
            //TODO
            return ans;
        }

        private Entity getEntityInPoint(Point point)
        {
            return this.gameGrid[point.X, point.Y];
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        public List<Entity> whatSees(Entity ent)
        {
            
            //Get all the relevant variables
            List<Entity> ans = new List<Entity>();
            Point location = this.convertToCentralPoint(ent,this.locations[ent]);
            Sight sight = ent.Sight;
            int radius = sight.Range;
            wasBlocked blocked = sight.Blocked;

            CurriedListAdd listAdd = delegate(List<Entity> list){
                return delegate(Entity entity){
                    if (entity != null) { list.Add(entity); }
                };
            };

            Effect effect = listAdd(ans);

            BlastEffect blast = new BlastEffect(radius, effect, blocked);

            return ans;
        }

        public void areaEffect(Entity ent, BlastEffect blast)
        {
            this.areaEffect(this.convertToCentralPoint(ent,this.locations[ent]), blast);
        }


        private void areaEffect(Point location, BlastEffect blast)
        {
            int radius = blast.Radius;
            wasBlocked blocked = blast.Blocked;
            Effect effect = blast.Effect;
            int x = location.X;
            int y = location.Y;
            int newX, newY;
            //checks in each of the four cardinal directions;
            for (int i = 0; i < radius; i++)
            {
                newX = Math.Min(x+i, gameGrid.GetLength(0));
                newY = Math.Min(y+radius-i,gameGrid.GetLength(1));
                processPath(location, new Point(newX,newY), blocked, effect);

                newY = newY = Math.Max(y-radius+i,0);
                processPath(location, new Point(newX,newY), blocked, effect);

                newX = Math.Max(x-i, 0);
                processPath(location, new Point(newX,newY), blocked, effect);

                newY = Math.Min(y+radius-i,gameGrid.GetLength(1));
                processPath(location, new Point(newX,newY), blocked, effect);
            }
        }

        //This function simulates a single shot
        public void solveShot(Shooter shooter, Entity target)
        {
            //get all relevant variables
            ShotType shot = shooter.weapon().Shot;
            Point currentTarget = shooter.hitFunc()(this.convertToCentralPoint(target,this.locations[target]));
            //TODO - If there's a target that I see only parts of it, how do I aim at the visible parts?
            Point exit = this.convertToCentralPoint((Entity)shooter,this.locations[(Entity)shooter]);
            wasBlocked blocked = shot.Blocked;
            Effect effect = shot.Effect;

            //get the path the bullet is going through, and affect targets
            Point endPoint = this.processPath(exit, currentTarget, blocked, effect);

            if (shot.Blast != null){

                this.areaEffect(endPoint, shot.Blast);

            }
        }

        private Point processPath(Point exit, Point target, wasBlocked blocked, Effect effect)
        {
            int x0 = exit.X;
            int y0 = exit.Y;
            int x1 = target.X;
            int y1 = target.Y;
            int dx = abs(x1-x0);
            int dy = abs(y1-y0);
            int sx, sy, e2;
            if (x0 < x1) sx = 1; else sx = -1;
            if (y0 < y1) sy = 1; else sy = -1;
            int err = dx-dy;
            while (!(x0 == x1 & y0 == y1))
            {
                Entity ent = gameGrid[x0, y0];
                effect(ent);
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
            return new Point(x0, y0);
        }


        private int abs(int a){
            if (a > 0) return a; else return -a;
        }

    }
}
