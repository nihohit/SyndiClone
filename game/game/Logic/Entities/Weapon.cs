
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    public class Weapons
    {
        #region consts
        private static Dictionary<WeaponType, Weapons> s_weapons = new Dictionary<WeaponType, Weapons>();
        const int PISTOL_ROF = 20;
        const int PISTOL_RANGE = 12;
        const int PISTOL_THREAT = 1;
        const double PISTOL_ACC = 8;

        #endregion

        #region constructor

        private Weapons(int range, int ROF, double acc, Shot shot, int threat)
        {
            Accuracy = acc;
            Range = range;
            RateOfFire = ROF;
            Shot = shot;
            Threat = threat;
        }

        public static Weapons Instance(WeaponType type)
        {
            if (!s_weapons.ContainsKey(type))
            {
                switch (type)
                {
                    case(WeaponType.PISTOL):
                        s_weapons.Add(type, new Weapons(PISTOL_RANGE, PISTOL_ROF, PISTOL_ACC, Shot.instance(ShotType.PISTOL_BULLET) , PISTOL_THREAT));
                        break;
                    //TODO - missing types

                }
            }
            return s_weapons[type];
        }

        #endregion

        #region properties

        public int Threat { get; set; }

        public int Range { get; set; }

        public Shot Shot { get; set; }

        public int RateOfFire { get; set; }

        public double Accuracy { get; set; }

        #endregion
    }
}