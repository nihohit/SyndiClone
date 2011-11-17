
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    class Weapon
    {
        /******************
        class consts
        ****************/
        static Dictionary<WeaponType, Weapon> _weapons = new Dictionary<WeaponType, Weapon>();
        const int timeToNextShot = 100;

        /******************
        class fields
        ****************/

        private readonly int _range;
        private readonly Shot _shot; //This property represents the shot type of the weapon. notice that different weapons can have the same kind of shot, but with different ranges & rates of fire
        private readonly int _rateOfFire;
        private readonly int _threat;

        /******************
        constructors
        ****************/

        private Weapon(int range, int ROF, float acc, Shot shot, int threat)
        {
            this._accuracy = acc;
            this._range = range;
            this._rateOfFire = ROF;
            this._shot = shot;
            this._threat = threat;
        }

        internal static Weapon instance(WeaponType type)
        {
            if (!_weapons.ContainsKey(type))
            {
                switch (type)
                {

                    //missing types

                }
            }

            return _weapons[type];
        }



        /******************
        Getters & setters
        ****************/

        public int Threat
        {
            get { return _threat; }
        } 

        internal int Range
        {
          get { return _range; }  
        } 

        internal Shot Shot
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
    }
}