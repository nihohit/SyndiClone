
namespace Game.Logic.Entities
{
    class BlastEffect
    {
        private readonly int _radius;
        private readonly Effect _effect;
        private readonly bool _blocked;

        public BlastEffect(int radius, Effect effect, bool blocked)
        {
            this._blocked = blocked;
            this._effect = effect;
            this._radius = radius;
        }

        public int getRadius()
        {
            return this._radius;
        }

        public Effect getEffect()
        {
            return this._effect;
        }

        public bool getBlocked()
        {
            return this._blocked;
        }

    }
}
