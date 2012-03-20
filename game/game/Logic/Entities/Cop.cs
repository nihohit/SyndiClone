﻿using System;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
   class Cop : Person, Shooter
    //TODO
    {

       /*********
        * class statics
        *********/
       static Random generator = new Random();
       static Point copHitFunc(Point target, int range, double weaponAcc)
       {

           int rangeOfNumbers = Convert.ToInt32(range * weaponAcc / COP_ACC);
           int randX = generator.Next(-rangeOfNumbers, rangeOfNumbers);
           int randY = generator.Next(-rangeOfNumbers, rangeOfNumbers);
           return new Point(target.X + randX, target.Y + randY);
       }

       static private Reaction copReact(List<Entity> list)
       {
           Entity temp = Targeters.threatTargeterHigh(list, Affiliation.INDEPENDENT);

           if (temp == null)
           {
               return new Reaction(null, Action.IGNORE);
           }
           return new Reaction(temp, Action.FIRE_AT);

       }

       static Entity copTargeter(List<Entity> list)
       {
           return Targeters.threatTargeterHigh(list, Affiliation.INDEPENDENT);
       }

       private static Weapon copWeapon = Weapon.instance(WeaponType.PISTOL);

       /**********
        * class consts
        **********/
    
       private const int COP_ACC = 5;
       private const int COP_REACTION_TIME = 10;
       private const int COP_HEALTH = 10;
       private const int COP_SPEED = 10;
       private const int COP_SHOOTING_RATE = 10;
       private const int COP_SHOOT_TIME = 200;

       /************
         * class fields
         ***********/
       private int timeBeforeShot;

       /***********
        * constructor
        ***********/

       private readonly PoliceStation station;

       public Cop(PoliceStation station) :
           base(COP_REACTION_TIME, copReact, COP_HEALTH, Affiliation.INDEPENDENT, Sight.instance(SightType.POLICE_SIGHT), COP_SPEED, new LinkedList<Direction>())
       {
           // TODO: Complete member initialization
           this.station = station;
           this.timeBeforeShot = 0;
       }

       /***********
        * methods
        ********/

        Weapon Shooter.weapon()
        {
            return copWeapon;
        }

        HitFunction Shooter.hitFunc()
        {
            return copHitFunc;
        }

       internal override void destroy()
        {
 	       base.destroy();
           station.policemanDestroyed();
        }

       bool Shooter.readyToShoot()
       {
           bool result = reachAffect(COP_SHOOT_TIME, this.timeBeforeShot, COP_SHOOTING_RATE);
           if (result)
           {
               this.timeBeforeShot -= COP_SHOOT_TIME;
           }
           else
           {
               this.timeBeforeShot += COP_SHOOTING_RATE;
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
    }
}
