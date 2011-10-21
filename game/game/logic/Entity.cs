

namespace game.logic
{
    class Entity
    {
        private int health;

        public bool hit(int damage)
        {
            this.health -= damage;
            return (this.health <= 0);
        }

    }
}
