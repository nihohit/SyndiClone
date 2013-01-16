using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{

    interface IConstructor
    {           

        MovingEntity GetConstruct();

        Vector ExitPoint();

        bool ReadyToConstruct();

    }
}
