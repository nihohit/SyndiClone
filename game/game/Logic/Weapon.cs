﻿
namespace Game.Logic
{
    abstract class Weapon
    {
        private readonly int _range;

        internal int rng
        {
          get { return _range; }  
        } 

        readonly ShotType _shot;

        internal ShotType shot
        {
          get { return _shot; }  
        } 

        readonly int _rateOfFire;

        public int RateOfFire
        {
          get { return _rateOfFire; }  
        } 

        private readonly float _accuracy;

        internal float acc
        {
          get { return _accuracy; }  
        }


        public Weapon(int range, int ROF, float acc, ShotType shot)
        {
            this._accuracy = acc;
            this._range = range;
            this._rateOfFire = ROF;
            this._shot = shot;
        }
    }


}
