using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    abstract class Shooter : Entity
    {
        private readonly Weapon weapon;

        protected Weapon Weapon
        {
            get { return weapon; }
        }


        private readonly HitFunction hitFunc;

        protected HitFunction HitFunc
        {
            get { return hitFunc; }
        } 

    }
}
