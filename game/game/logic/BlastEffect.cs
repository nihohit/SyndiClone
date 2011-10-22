
namespace Game.Logic
{
    class BlastEffect
    {
        private readonly int _radius;
        private readonly LogicInfo.Effect _effect;
        private readonly bool _blocked;

        public BlastEffect(int radius, LogicInfo.Effect effect, bool blocked)
        {
            this._blocked = blocked;
            this._effect = effect;
            this._radius = radius;
        }

        public int getRadius()
        {
            return this._radius;
        }

        public LogicInfo.Effect getEffect()
        {
            return this._effect;
        }

        public bool getBlocked()
        {
            return this._blocked;
        }

    }
}
