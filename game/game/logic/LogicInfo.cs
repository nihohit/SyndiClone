using System.Collections.Generic;
using Game.Logic.Entities;


namespace Game.Logic
{
    internal delegate bool EntityChecker(Entity ent);
    internal delegate Point HitFunction(Point target);
    internal delegate void Effect(Entity ent);
    internal delegate bool wasBlocked(Visibility ent);
    internal delegate Entity targetChooser(List<Entity> targets);
    internal delegate Reaction reactionFunction(Entity ent, Event evnt);
    internal enum Action { FIRE_AT, IGNORE, RUN_AWAY }
    internal enum entityType { PERSON, VEHICLE, BUILDING}
    internal enum Visibility { CLOAKED, MASKED, REVEALED, SOLID }
    internal enum Event { SHOT_FIRED, COMBAT_VEHICLE_APPEARED, PERSON_DIED, BUILDING_CRASHED }
    internal enum Affiliation { INDEPENDENT, CORP1, CORP2, CORP3, CORP4 }
    internal enum SightType { CIV_SIGHT }
    internal enum WeaponType { PISTOL, ASSAULT, BAZOOKA, SNIPER, RAILGUN }

    internal class Reaction{

        private readonly Entity _focus;
        private readonly Action _actionChosen;
        private readonly Point _runTo;

        internal Point RunTo
        {
            get { return _runTo; }
        } 

        internal Action ActionChosen
        {
            get { return _actionChosen; }
        }

        internal Entity Focus
        {
            get { return _focus; }
        } 

        internal Reaction(Entity ent, Action action)
        {
            this._actionChosen = action;
            this._focus = ent;
        }


    }

}
