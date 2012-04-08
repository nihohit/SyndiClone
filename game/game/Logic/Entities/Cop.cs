using System;
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
    
       private const int COP_ACC = 5;
       private const int COP_REACTION_TIME = 10;
       private const int COP_HEALTH = 10;
       private const int COP_SPEED = 10;
       private const int COP_SHOOT_TIME = 2000;

       /************
         * class fields
         ***********/
       private int timeBeforeShot;
       private readonly PoliceStation station;

       /***********
        * constructor
        ***********/

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
    }
}
