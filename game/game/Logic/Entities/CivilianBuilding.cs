using Game.Logic;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    internal class CivilianBuilding :  Building, Constructor
    {

        /******************
        class fields
        ****************/

        private readonly int _sizeModifier;

        /***********
         * constructor
         **********/
        internal CivilianBuilding(Game.Vector realSize, int sizeModifier, Vector exit)
            : base(BASE_BUILD_REACTION_TIME / sizeModifier, civBuildReact, BASE_BUILD_HEALTH * sizeModifier, realSize, Affiliation.CIVILIAN, Sight.instance(SightType.CIV_SIGHT))
        {
            base.ExitPoint = exit;
            this._sizeModifier = sizeModifier;
        }

        /******************
        Methods
        ****************/

        /*
         * This function is just the basic reaction function for the basic civic buildings.
         */
        public static Reaction civBuildReact(List<Entity> ent)
        {
            Civilian temp = new Civilian();
            return new Reaction(temp, Action.CREATE_ENTITY);
        }

        public MovingEntity getConstruct()
        {
            return (MovingEntity)this.Reaction.Focus;
        }

        bool Constructor.readyToConstruct()
        {
            //TODO - problem?
            return true;
        }

        public Vector exitPoint()
        {
            return base.ExitPoint;
        }
    }

        
}
