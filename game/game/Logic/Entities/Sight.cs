
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    /*This class is supposed to represent the different kinds of sight mechanisms we'll have. 
     *The two important things to understand about sight - 
     *a. not everything that can be seen blocks the sight from continuing.  
     *b. Sights are created as instances, so as not to repeat the creation of different objects.  
     */

    public class Sight
    {
        #region consts

        static Dictionary<SightType, Sight> s_sights = new Dictionary<SightType, Sight>();
        const int DEFAULT_RANGE = 25;
        const bool DEFAULT_CLOAKED = false;
        static WasBlocked DEFAULT_BLOCKED = DefaultBlockedSight;

        #endregion

        #region constructors

        private Sight(int range, WasBlocked blocked, bool cloak)
        {
            Blocked = blocked;
            Range = range;
            SeesCloaked = cloak;
        }

        public static Sight instance(SightType type)
        {
            if (!s_sights.ContainsKey(type))
            {
                switch (type)
                {

                    case (SightType.DEFAULT_SIGHT):
                        s_sights.Add(type, new Sight(DEFAULT_RANGE, DEFAULT_BLOCKED, DEFAULT_CLOAKED));
                        break;
                    case(SightType.BLIND):
                        s_sights.Add(type, new Sight(0, DEFAULT_BLOCKED, false));
                        break;

                    //TODO - missing types
                }
            }

            return s_sights[type];
        }

        #endregion

        #region properties

        public bool SeesCloaked { get; set; }

        public int Range { get; set; }

        public WasBlocked Blocked { get; set; }

        #endregion

        static public bool DefaultBlockedSight(Entity ent)
        {
            if (ent == null) return false;
            switch (ent.VisibleStatus)
            {
                case Visibility.SOLID:
                    return true;
                default:
                    return false;
            }
        }
    }
}
