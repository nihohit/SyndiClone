using System.Collections.Generic;

namespace game.logic
{
    interface Grid
    {
        
        Entity checkFor(Point center, int radius, LogicInfo.EntityChecker checker, bool blocked);
        LinkedList<Point> generatePathTowards(Point exit, Point target, int step, bool blocked);
        void affectArea(Entity shooter, Entity target, int radius, LogicInfo.HitFunction func, LogicInfo.Effect effect, bool blocked);
        void affectTarget(Entity shooter, Entity target, LogicInfo.HitFunction func, LogicInfo.Effect effect, bool blocked);

    }
}
