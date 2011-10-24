
namespace Game.Logic.Entities
{
    class Sight
    {
        private readonly int _range;

        public int Range
        {
            get { return _range; }
        }

        private readonly wasBlocked _blocked;

        internal wasBlocked Blocked
        {
            get { return _blocked; }
        } 


        public Sight(int range, wasBlocked blocked)
        {
            this._blocked = blocked;
            this._range = range;
        }


    }
}
