using System.Collections.Generic;


namespace Game.Logic
{
        internal delegate bool EntityChecker(Entity ent);
        internal delegate Point HitFunction(Point target);
        internal delegate void Effect(Entity ent);
        internal delegate bool wasBlocked(Entity ent);
        internal delegate Entity targetChooser(List<Entity> targets);

}
