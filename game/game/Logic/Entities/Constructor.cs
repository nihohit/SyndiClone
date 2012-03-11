using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    
    interface Constructor
    {
           

        MovingEntity getConstruct();

        Vector exitPoint();

        bool readyToConstruct();

    }
}
