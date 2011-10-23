using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    abstract class Shooter : Entity
    {
        private readonly Weapon weapon;

        internal Weapon Weapon
        {
            get { return weapon; }
        }


        private readonly HitFunction hitFunc;

        internal HitFunction HitFunc
        {
            get { return hitFunc; }
        } 

    }
}
