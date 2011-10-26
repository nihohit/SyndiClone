using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    interface Shooter
    {

        Weapon weapon();

        HitFunction hitFunc();

        Sight sight();

    }
}
