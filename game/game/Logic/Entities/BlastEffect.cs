
namespace Game.Logic.Entities
{
    class BlastEffect
    {
        private readonly int _radius;

        public int Radius
        {
            get { return _radius; }
        }

        private readonly Effect _effect;

        internal Effect Effect
        {
            get { return _effect; }
        }

        private readonly wasBlocked _blocked;

        public wasBlocked Blocked
        {
            get { return _blocked; }
        } 


        public BlastEffect(int radius, Effect effect, wasBlocked blocked)
        {
            this._blocked = blocked;
            this._effect = effect;
            this._radius = radius;
        }

    }
}
