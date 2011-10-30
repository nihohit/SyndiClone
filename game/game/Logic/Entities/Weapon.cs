
namespace Game.Logic.Entities
{
    class Weapon
    {
        const int timeToNextShot = 100;
        private readonly int _range;
        private readonly ShotType _shot;
        private readonly int _rateOfFire;
        private readonly int _threat;

        public int Threat
        {
            get { return _threat; }
        } 

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

        internal Weapon(int range, int ROF, float acc, ShotType shot, int threat)
        {
            this._accuracy = acc;
            this._range = range;
            this._rateOfFire = ROF;
            this._shot = shot;
            this._threat = threat;
        }
    }
}