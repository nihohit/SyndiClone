
namespace Game.Logic.Entities
{
    abstract class Person : MovingEntity
    {

        public Person()
        {
            this._size = new Vector(1, 1);
        }

        public override bool blocksVision()
        {
            return false;
        }
    }
}
