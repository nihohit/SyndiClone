
using System.Collections.Generic;
using Game.Logic;

namespace Game.Logic.Entities
{
    class Shot
    {

        /******************
        class consts
        ****************/
        static Dictionary<ShotType, Shot> _shots = new Dictionary<ShotType, Shot>();

        /******************
        class fields
        ****************/
        private readonly BlastEffect _blast;
        private readonly Effect _effect;
        private readonly wasBlocked _blocked;
        private readonly ShotType _type;



        /******************
        constructors
        ****************/
        internal static Shot instance(ShotType type)
        {
            if (!_shots.ContainsKey(type))
            {
                switch (type)
                {

                    //missing types

                }
            }

            return _shots[type];
        }

        protected Shot(BlastEffect blast, Effect effect, wasBlocked blocked, ShotType type)
        {
            this._blast = blast;
            this._blocked = blocked;
            this._effect = effect;
            this._type = type;
        }

        /******************
        Getters & setters
        ****************/

        internal ShotType Type
        {
            get { return _type; }
        }


        internal BlastEffect Blast
        {
            get { return _blast; }
        }

        internal Effect Effect
        {
            get { return _effect; }
        }

        internal wasBlocked Blocked
        {
            get { return _blocked; }
        }


    }
}
