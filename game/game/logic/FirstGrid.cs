using System.Collections.Generic;

namespace game.logic
{
    class FirstGrid : Grid
    {
        Dictionary<Entity, Point> locations;
        Entity[,] gameGrid;

        public FirstGrid(int x, int y)
        {
            locations = new Dictionary<Entity, Point>();
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
            // TODO
            return ans;
        }

        private Entity getEntityInPoint(Point point)
        {
            return this.gameGrid[point.getX(), point.getY()];
        }

        //This function checks if any entity in the radius around the point answers the conditions in checker
        //TODO - ensure that LoS is blocked by relevant entities. After that, copy the function for area of effect. 
        public Entity checkFor(Entity ent, int radius, LogicInfo.EntityChecker checker)
        {
            //Get all the relevant variables
            Entity ans = null;
            Entity temp = null;
            Point location = this.locations[ent];
            int x = location.getX();
            int y = location.getY();

            //the main loop - checks for each length
            for (int i = 1; i < radius; i++)
            {
                //the secondary loop - checks for each variety of x,y in given length
                for (int j = 0; j < i; j++)
                {
                    //the check - for positive & negative values
                    if (((x + j) < gameGrid.GetLength(0)) & ((y + (i - j)) < gameGrid.GetLength(1)))
                    {
                        temp = this.getEntityInPoint(new Point(x + j, y + (i - j)));
                        if (checker(temp))
                        {
                            ans = temp;
                            break;
                        }
                    }

                    if (((x - j) >= 0) & ((y + (i - j)) < gameGrid.GetLength(1)))
                    {
                        temp = this.getEntityInPoint(new Point(x - j, y + (i - j)));
                        if (checker(temp))
                        {
                            ans = temp;
                            break;
                        }
                    }

                    if (((x + j) < gameGrid.GetLength(0)) & ((y - (i - j)) >= 0))
                    {
                        temp = this.getEntityInPoint(new Point(x + j, y - (i - j)));
                        if (checker(temp))
                        {
                            ans = temp;
                            break;
                        }
                    }


                    if (((x - j) >= 0 ) & ((y - (i - j)) >= 0))
                    {
                        temp = this.getEntityInPoint(new Point(x - j, y - (i - j)));
                        if (checker(temp))
                        {
                            ans = temp;
                            break;
                        }
                    }

                    temp = null;
                }

                if (ans!=null){
                    break;
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
        public void affectTarget(Entity shooter, Entity target, LogicInfo.HitFunction func, ShotType shot)
        {
            //get all relevant variables
            Point currentTarget = func(this.locations[target]);
            Point exit = this.locations[shooter];
            LogicInfo.wasBlocked blocked = shot.getBlocked();
            LogicInfo.Effect effect = shot.getEffect();

            Entity temp = null;

            //get the path the bullet is going through, and affect targets
            LinkedList<Point> path = this.getSimplePath(exit, currentTarget);
            //the first check is whether the shot penetrated
            
            while (!blocked(temp) & path.Count > 0)
            {

                temp = this.getEntityInPoint(path.First.Value);
                effect(temp);
                path.RemoveFirst();

            }

            if (shot.getBlast() != null){

                if (temp != null)
                {
                    currentTarget = this.locations[temp];
                }

                this.areaEffect(currentTarget, shot.getBlast());

            }
        }

    }
}
