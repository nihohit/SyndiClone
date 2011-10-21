using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.logic
{
    interface Grid
    {
        Entity findHit(Point exit,Point target);


    }
}
