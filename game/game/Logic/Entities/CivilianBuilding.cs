using Game.Logic;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    internal class CivilianBuilding :  Building, Constructor
    {
        /******************
        class consts
        ****************/

        const int BASE_BUILD_REACTION_TIME = 200;
        const int BASE_BUILD_HEALTH = 10;


        /******************
        class fields
        ****************/

        Vector _exitPoint;
        int sizeModifier;

        /***********
         * constructor
         **********/
        internal CivilianBuilding(Game.Vector realSize, int sizeModifier, Vector exit)
            : base(BASE_BUILD_REACTION_TIME / sizeModifier, civBuildReact, BASE_BUILD_HEALTH * sizeModifier, realSize, Affiliation.CIVILIAN, Sight.instance(SightType.CIV_SIGHT))
        {
            this._exitPoint = exit;
        }

        /******************
        Methods
        ****************/

        /*
         * This function is just the basic reaction function for the basic civic buildings.
         */
        public static Reaction civBuildReact(List<Entity> ent)
        {
            return new Reaction(new Civilian(), Action.CREATE_ENTITY);
        }

        public MovingEntity getConstruct()
        {
            return (MovingEntity)this.Reaction.Focus;
        }

        Vector Constructor.exitPoint()
        {
            return this._exitPoint;
        }

        bool Constructor.readyToConstruct()
        {
            //TODO - problem?
            return true;
        }
    }

        
}
