using Game.Logic;
using System.Collections.Generic;

namespace Game.Logic.Entities
{
    internal class CivilianBuilding :  Building, Constructor
    {

        /******************
        class members
        ****************/

        private readonly int _sizeModifier;

        /***********
         * constructor
         **********/
        internal CivilianBuilding(Game.Vector realSize, int sizeModifier, Vector exit)
            : base(sizeModifier, Entity.reactionPlaceHolder, realSize, Affiliation.CIVILIAN)
        {
            base.ExitPoint = exit;
            this._sizeModifier = sizeModifier;
            reactionFunction react = delegate(List<Entity> ent)
            {
                Civilian temp = new Civilian(exit.vectorToDirection());
                this._readyToBuild = true;
                return new ConstructReaction(temp);
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
            Civilian temp = new Civilian(base.ExitPoint.vectorToDirection());
            this._readyToBuild = true;
            return new ConstructReaction(temp);
        }

        public MovingEntity getConstruct()
        {
            return ((ConstructReaction)this.Reaction).Focus;
        }

        bool Constructor.readyToConstruct()
        {
            return base.readyToConstruct();
        }

        public Vector exitPoint()
        {
            return base.ExitPoint;
        }

        public override string ToString()
        {
            return "Civilian Building, " + base.ToString();
        }
    }

        
}
