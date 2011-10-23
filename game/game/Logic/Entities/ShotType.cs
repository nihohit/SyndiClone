using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    class ShotType
    {
        //TODO question - I think we need to talk about the shooting mechanism, also, we should do that later - after we'll have people running on our map.
        private readonly BlastEffect _blast;
        private readonly Effect _effect;
        private readonly wasBlocked _blocked;

        public ShotType(BlastEffect blast, Effect effect, wasBlocked blocked)
        {
            this._blast = blast;
            this._blocked = blocked;
            this._effect = effect;
        }

        public BlastEffect getBlast()
        {
            return this._blast;
        }

        public Effect getEffect()
        {
            return this._effect;
        }

        public wasBlocked getBlocked()
        {
            return this._blocked;
        }

    }
}
