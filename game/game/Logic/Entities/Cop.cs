﻿using System;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
   class Cop : Person, Shooter
    {

       /*********
        * class statics
        *********/
       static Random generator = new Random();

       //TODO - change back to threatTargeterHigh
       static private Reaction copReact(List<Entity> list)
       {
           Entity temp = Targeters.simpleTestTargeter(list, Affiliation.INDEPENDENT);
           if (temp == null)
           {
               return new Reaction(null, Action.IGNORE);
               
           }
           return new Reaction(temp, Action.FIRE_AT);

       }
       //TODO - change back to threatTargeterHigh
       static Entity copTargeter(List<Entity> list)
       {
           return Targeters.threatTargeterLow(list, Affiliation.INDEPENDENT);
       }

       private static Weapon copWeapon = Weapon.instance(WeaponType.PISTOL);

       /**********
        * class consts
        **********/
    
       private const int COP_SHOOT_TIME = 2000;

       /************
         * class members
         ***********/
       private int timeBeforeShot;
       private readonly PoliceStation station;

       /***********
        * constructor
        ***********/

       public Cop(PoliceStation station) :
           base(copReact, Affiliation.INDEPENDENT, new LinkedList<Direction>())
       {
           // TODO: Complete member initialization
           this.station = station;
           this.timeBeforeShot = 0;
           List<Upgrades> list = new List<Upgrades>();
           list.Add(Upgrades.BULLETPROOF_VEST);
           base.upgrade(list);
       }

       /***********
        * methods
        ********/

        Weapon Shooter.weapon()
        {
            return copWeapon;
        }

       internal override void destroy()
        {
 	       base.destroy();
           station.policemanDestroyed();
        }

       bool Shooter.readyToShoot()
       {
           bool result = reachAffect(COP_SHOOT_TIME, this.timeBeforeShot, copWeapon.RateOfFire);
           if (result)
           {
               this.timeBeforeShot -= COP_SHOOT_TIME;
           }
           else
           {
               this.timeBeforeShot += copWeapon.RateOfFire;
           }
           return result;
       }

       Entity Shooter.target()
       {
           return this.Reaction.Focus;
       }

       targetChooser Shooter.targeter()
       {
           return copTargeter;
       }

       public override string ToString()
       {
           return "Cop, " + base.ToString();
       }
    }
}
