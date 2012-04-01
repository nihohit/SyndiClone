
using System.Collections.Generic;
using Game.Logic;

namespace Game.Logic.Entities
{
    class Shot
    {

        /******************
        class statics
        ****************/
        static Dictionary<ShotType, Shot> _shots = new Dictionary<ShotType, Shot>();

        static bool bulletBlocked(Entity ent)
        {
            if (ent == null) return false;
            return true;
        }

        static void pistolBulletEffect(Entity ent)
        {
            ent.hit(PISTOL_BULLET_DAMAGE);
        }

        /************
         * class consts
         *************/

        const int PISTOL_BULLET_DAMAGE = 3;

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
                    case(ShotType.SIGHT):
                        break;
                    case(ShotType.PISTOL_BULLET):
                        _shots.Add(type, new Shot(null, pistolBulletEffect, bulletBlocked, type));
                        break;
                    //TODO - missing types

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
