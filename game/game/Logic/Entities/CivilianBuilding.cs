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
            : base(sizeModifier, Entity.reactionPlaceHolder, sizeModifier, realSize, Affiliation.CIVILIAN, Sight.instance(SightType.CIV_SIGHT))
        {
            base.ExitPoint = exit;
            this._sizeModifier = sizeModifier;
            reactionFunction react = delegate(List<Entity> ent)
            {
                Civilian temp = new Civilian();
                this._readyToBuild = true;
                return new Reaction(temp, Action.CREATE_ENTITY);
            };
            this.ReactionFunction = react;
        }

        /******************
        Methods
        ****************/

        /*
         * This function is just the basic reaction function for the basic civic buildings.
         */
        public Reaction civBuildReact(List<Entity> ent)
        {
            Civilian temp = new Civilian();
            this._readyToBuild = true;
            return new Reaction(temp, Action.CREATE_ENTITY);
        }

        public MovingEntity getConstruct()
        {
            return (MovingEntity)this.Reaction.Focus;
        }

        bool Constructor.readyToConstruct()
        {
            return base.readyToConstruct();
        }

        public Vector exitPoint()
        {
            return base.ExitPoint;
        }
    }

        
}
