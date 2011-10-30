
namespace Game.Logic.Entities
{
    class ShotType
    {
        /*HACK (amit) 
         * question - I think we need to talk about the shooting mechanism, also, we should do that later - after we'll have people running on our map.
         * */
         
        private readonly BlastEffect _blast;
        private readonly Effect _effect;
        private readonly wasBlocked _blocked;
        private readonly TypesOfShot _type;

        internal TypesOfShot Type
        {
            get { return _type; }
        } 

  
        internal BlastEffect Blast
        {
            get { return _blast; }
        }

        internal Effect Effect
        {
            get { return _effect; }
        }
 
        internal wasBlocked Blocked
        {
            get { return _blocked; }
        } 

        public ShotType(BlastEffect blast, Effect effect, wasBlocked blocked, TypesOfShot type)
        {
            this._blast = blast;
            this._blocked = blocked;
            this._effect = effect;
            this._type = type;
        }


    }
}

