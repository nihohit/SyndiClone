using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    class Agent : Person, Shooter
    {
        enum AGENT_SIGHTS { };
        enum AGENT_WEAPONS { };



        Weapon weapon;
        HitFunction hitFunc;

        Weapon Shooter.weapon()
        {
            throw new NotImplementedException();
        }

        HitFunction Shooter.hitFunc()
        {
            throw new NotImplementedException();
        }
    }
}
