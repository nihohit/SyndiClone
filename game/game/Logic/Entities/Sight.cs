
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    /*This class is supposed to represent the different kinds of sight mechanisms we'll have. 
     *The two important things to understand about sight - 
     *a. not everything that can be seen blocks the sight from continuing.  
     *b. Sights are created as instances, so as not to repeat the creation of different objects.  
     */

    class Sight
    {
        /******************
        class consts
        ****************/

        static Dictionary<SightType, Sight> _sights = new Dictionary<SightType, Sight>();
        const int DEFAULT_RANGE = 25;
        const bool DEFAULT_CLOAKED = false;
        static wasBlocked DEFAULT_BLOCKED = defaultBlockedSight;
        
        static internal bool defaultBlockedSight(Entity ent)
        {
            if (ent == null) return false;
            switch (ent.Visible)
            {
                case Visibility.SOLID:
                    return true;
                default:
                    return false;
            }
        }

        /******************
        class members
        ****************/

        private readonly int _range;
        private readonly wasBlocked _blocked;
        private readonly bool _seesCloaked;

        /******************
        constructors
        ****************/


        private Sight(int range, wasBlocked blocked, bool cloak)
        {
            this._blocked = blocked;
            this._range = range;
            this._seesCloaked = cloak;
        }

        internal static Sight instance(SightType type)
        {
            if (!_sights.ContainsKey(type))
            {
                switch (type)
                {

                    case (SightType.DEFAULT_SIGHT):
                        _sights.Add(type, new Sight(DEFAULT_RANGE, DEFAULT_BLOCKED, DEFAULT_CLOAKED));
                        break;
                    case(SightType.BLIND):
                        _sights.Add(type, new Sight(0, DEFAULT_BLOCKED, false));
                        break;

                    //TODO - missing types

                }
            }

            return _sights[type];
        }

        /******************
        Getters & setters
        ****************/


        internal bool SeesCloaked
        {
            get { return _seesCloaked; }
        }

        internal int Range
        {
            get { return _range; }
        }

        internal wasBlocked Blocked
        {
            get { return _blocked; }
        } 




    }
}
