using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace game.logic
{
    class Sight
    {
        private readonly int _range;
        private readonly LogicInfo.wasBlocked _blocked;

        public Sight(int range, LogicInfo.wasBlocked blocked)
        {
            this._blocked = blocked;
            this._range = range;
        }

        public int getRange()
        {
            return this._range;
        }

        public LogicInfo.wasBlocked getBlocked()
        {
            return this._blocked;
        }
    }
}
