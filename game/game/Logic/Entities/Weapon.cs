
namespace Game.Logic.Entities
{
    class Weapon
    {
        private readonly int _range;
        private readonly ShotType _shot;
        private readonly int _rateOfFire;

        internal int Range
        {
          get { return _range; }  
        } 

        internal ShotType Shot
        {
          get { return _shot; }  
        } 

        public int RateOfFire
        {
          get { return _rateOfFire; }  
        } 

        private readonly float _accuracy;

        internal float acc
        {
          get { return _accuracy; }  
        }

        internal Weapon(int range, int ROF, float acc, ShotType shot)
        {
            this._accuracy = acc;
            this._range = range;
            this._rateOfFire = ROF;
            this._shot = shot;
        }
    }
}