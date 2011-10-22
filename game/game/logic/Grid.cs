using System.Collections.Generic;

namespace Game.Logic
{
    interface Grid
    {
        
        Entity checkFor(Point center, int radius, LogicInfo.EntityChecker checker, bool blocked);
        LinkedList<Point> generatePathTowards(Point exit, Point target, int step, bool blocked);
        void solveShot(Entity shooter, Entity target, LogicInfo.HitFunction func, ShotType shot);

    }
}
