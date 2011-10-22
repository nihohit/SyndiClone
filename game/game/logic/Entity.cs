

namespace Game.Logic
{
    abstract class Entity
        //TODO: question - "Entity" is civilians, soldiers and buildings? anything else?
    {
        private int _health;
        private readonly Sight _sight;

        public bool hit(int damage)
        {
            this._health -= damage;
            return (this._health <= 0);
        }

        public Sight getSight()
        {
            return this._sight;
        }

        public abstract bool blocksVision();

    }
}
