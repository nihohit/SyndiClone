

namespace game.logic
{
    abstract class Entity
    {
        private int health;

        public bool hit(int damage)
        {
            this.health -= damage;
            return (this.health <= 0);
        }

        public abstract bool blocksVision();

    }
}
