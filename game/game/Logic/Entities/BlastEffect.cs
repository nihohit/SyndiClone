
namespace Game.Logic.Entities
{
    class BlastEffect : ShotType
    {
        private readonly int _radius;

        internal int Radius
        {
            get { return _radius; }
        }

        internal BlastEffect(int radius, Effect effect, wasBlocked blocked, TypesOfShot type) : base(null, effect, blocked, type)
        {
            this._radius = radius;
        }

        internal BlastEffect(BlastEffect blast,int radius, Effect effect, wasBlocked blocked, TypesOfShot type)
            : base(blast, effect, blocked, type)
        {
            this._radius = radius;
        }

    }
}
