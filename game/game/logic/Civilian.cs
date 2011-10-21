using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.logic
{
    class Civilian : Entity, Person
    {
        Point location;

        public Point getLocation()
        {
            return this.location;
        }
    }
}
