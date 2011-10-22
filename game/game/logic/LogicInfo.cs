
namespace game.logic
{
    class LogicInfo
    {
        public delegate bool EntityChecker(Entity ent);
        public delegate Point HitFunction(Point target);
        public delegate void Effect(Entity ent);
        public delegate bool wasBlocked(Entity ent);

    }
}
