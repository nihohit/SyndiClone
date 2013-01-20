﻿using System;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
   class Cop : Person, IShooter
   {

       #region static members

       //TODO - change back to threatTargeterHigh
       static private Reaction copReact(List<Entity> list)
       {
           Entity temp = Targeters.SimpleTestTargeter(list, Affiliation.INDEPENDENT);
           if (temp == null)
           {
               return new IgnoreReaction();
               
           }
           return new ShootReaction(temp);

       }
       //TODO - change back to threatTargeterHigh
       static Entity copTargeter(List<Entity> list)
       {
           return Targeters.ThreatTargeterLow(list, Affiliation.INDEPENDENT);
       }

       private static Weapons copWeapon = Weapons.Instance(WeaponType.PISTOL);

       #endregion

       #region consts
    
       private const int COP_SHOOT_TIME = 2000;

       #endregion

       #region fields 

       private int m_timeBeforeShot;
       private readonly PoliceStation m_station;

       #endregion

       #region constructors

       public Cop(PoliceStation station) :
           base(copReact, Affiliation.INDEPENDENT, new List<Direction>(), station.Exit.VectorToDirection())
       {
           m_station = station;
           m_timeBeforeShot = 0;
           List<Upgrades> list = new List<Upgrades>();
           list.Add(Upgrades.BULLETPROOF_VEST);
           base.Upgrade(list);
       }

       public Cop(PoliceStation station, List<Direction> path) :
           base(copReact, Affiliation.INDEPENDENT, new List<Direction>(path), station.Exit.VectorToDirection())
       {
           m_station = station;
           m_timeBeforeShot = 0;
           List<Upgrades> list = new List<Upgrades>();
           list.Add(Upgrades.BULLETPROOF_VEST);
           base.Upgrade(list);
       }

       #endregion

       #region IShooter

       public Weapons Weapon()
       {
           return copWeapon;
       }

       public bool ReadyToShoot()
       {
           bool result = ReachAffect(COP_SHOOT_TIME, m_timeBeforeShot, copWeapon.RateOfFire);
           if (result)
           {
               m_timeBeforeShot -= COP_SHOOT_TIME;
           }
           else
           {
               m_timeBeforeShot += copWeapon.RateOfFire;
           }
           return result;
       }

       public Entity Target()
       {
           return ((ShootReaction)Reaction).Focus;
       }

       public targetChooser Targeter()
       {
           return copTargeter;
       }

       #endregion

       public override void Destroy()
        {
 	       base.Destroy();
           m_station.PolicemanDestroyed();
        }

       public override string ToString()
       {
           return "Cop, " + base.ToString();
       }
    }
}
