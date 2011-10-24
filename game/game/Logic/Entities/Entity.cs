

namespace Game.Logic.Entities
{
    internal abstract class Entity

    {
        protected int _health;
        protected Sight _sight;
        protected Vector _size;

        internal Sight Sight
        {
            get { return _sight; }
        } 

        internal Vector Size
        {
            get { return _size; }
        }

        public bool hit(int damage)
        {
            this._health -= damage;
            return (this._health <= 0);
        }

        public abstract bool blocksVision();

    }
}
