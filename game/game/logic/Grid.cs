using System.Collections.Generic;

namespace Game.Logic
{
    interface Grid
    {
        

        LinkedList<Point> generatePathTowards(Point exit, Point target, int step, bool blocked);
        void solveShot(Shooter shooter, Entity target, HitFunction func, ShotType shot);

    }
}
