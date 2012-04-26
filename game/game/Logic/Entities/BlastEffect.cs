using System.Collections.Generic;
using Game.Logic;

namespace Game.Logic.Entities
{
    class BlastEffect : Shot
    {

        /******************
        class consts
        ****************/

        static Dictionary<BlastType, BlastEffect> _blasts = new Dictionary<BlastType, BlastEffect>();

        /******************
        class members
        ****************/
        private readonly int _radius;

        /******************
        constructors
        ****************/

        internal static BlastEffect instance(BlastType type)
        {
            if (!_blasts.ContainsKey(type))
            {
                switch (type)
                {

                    //missing types

                }
            }

            return _blasts[type];
        }

        internal static BlastEffect instance(int radius, Effect effect, wasBlocked blocked, ShotType type)
        {
            return new BlastEffect(radius, effect, blocked, type);
        }


        protected BlastEffect(int radius, Effect effect, wasBlocked blocked, ShotType type) : base(null, effect, blocked, type)
        {
            this._radius = radius;
        }

        protected BlastEffect(BlastEffect blast,int radius, Effect effect, wasBlocked blocked, ShotType type)
            : base(blast, effect, blocked, type)
        {
            this._radius = radius;
        }

        /******************
        Getters & setters
        ****************/

        internal int Radius
        {
            get { return _radius; }
        }

    }
}
