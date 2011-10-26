
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    class Sight
    {
        static Dictionary<SightType, Sight> _sights = new Dictionary<SightType, Sight>();
        const int CIV_RANGE = 50, COP_RANGE = 50;
        const bool CIV_CLOAKED = false, COP_CLOAKED = false;
        static wasBlocked CIV_BLOCKED = civBlocked;
        
        static internal bool civBlocked(Visibility ent)
        {
            switch (ent)
            {
                case Visibility.SOLID:
                    return true;
                default:
                    return false;
            }
        }

        private readonly int _range;
        private readonly wasBlocked _blocked;
        private readonly bool _seesCloaked;

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

                    case (SightType.CIV_SIGHT):
                        _sights.Add(SightType.CIV_SIGHT, new Sight(CIV_RANGE, CIV_BLOCKED, CIV_CLOAKED));
                        break;

                }
            }

            return _sights[type];
        }




    }
}
