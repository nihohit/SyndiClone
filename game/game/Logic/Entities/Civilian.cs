
namespace Game.Logic.Entities
{
    class Civilian : Person
    {

        const int CIV_HEALTH = 3;
        const int CIV_SPEED = 10;
        const int CIV_SIGHT_RANGE = 20;

        public bool CIV_SIGHT(Entity ent)
        {
            if (ent == null) return false;
            else return true;
        }


    }
}
