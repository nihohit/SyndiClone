using System.Collections.Generic;
using Game.Logic.Entities;

namespace Game.Logic
{


    class Grid
    {
        private readonly Dictionary<Entity, Point> locations;
        private readonly Entity[,] gameGrid;
        delegate Effect CurriedListAdd(List<Entity> list);

        public Grid(int x, int y)
        {
            locations = new Dictionary<Entity, Point>(); //TODO question: just "entity" is enough? not, maybe, "background+entity" ? 
            gameGrid = new Entity[x,y];
            for (int i = 0 ; i < x; i++){
                for (int j =0 ; j < y ; j++){
                    gameGrid[i,j] = null;
                }
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
            return this.gameGrid[point.getX(), point.getY()];
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        public List<Entity> whatSees(Entity ent)
        {
            
            //Get all the relevant variables
            List<Entity> ans = new List<Entity>();
            Point location = this.locations[ent];
            Sight sight = ent.getSight();
            int radius = sight.getRange();
            wasBlocked blocked = sight.getBlocked();

            CurriedListAdd listAdd = delegate(List<Entity> list){
                return delegate(Entity entity){
                    if (entity != null) { list.Add(entity); }
                };
            };

            Effect effect = listAdd(ans);
            for (int i = 1; i < radius; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    processPath(location, new Point(j, i - j), blocked, effect);
                }
            }

            return ans;
        }

        public void areaEffect(Entity ent, BlastEffect blast)
        {
            this.areaEffect(this.locations[ent], blast);
        }

        private void areaEffect(Point point, BlastEffect blast)
        {
            //TODO
        }

        //This function simulates a single shot
        public void solveShot(Shooter shooter, Entity target)
        {
            //get all relevant variables
            ShotType shot = shooter.Weapon.Shot;
            Point currentTarget = shooter.HitFunc(this.locations[target]);
            Point exit = this.locations[shooter];
            wasBlocked blocked = shot.getBlocked();
            Effect effect = shot.getEffect();

            //get the path the bullet is going through, and affect targets
            Point endPoint = this.processPath(exit, currentTarget, blocked, effect);
           

            if (shot.getBlast() != null){

                this.areaEffect(endPoint, shot.getBlast());

            }
        }


        private Point processPath(Point exit, Point target, wasBlocked blocked, Effect effect)
        {
            int x0 = exit.getX();
            int y0 = exit.getY();
            int x1 = target.getX();
            int y1 = target.getY();
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
                if (blocked(ent)) break;
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
