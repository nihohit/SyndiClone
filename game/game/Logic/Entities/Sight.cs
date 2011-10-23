
namespace Game.Logic.Entities
{
    class Sight
    {
        private readonly int _range;
        private readonly wasBlocked _blocked;

        public Sight(int range, wasBlocked blocked)
        {
            this._blocked = blocked;
            this._range = range;
        }

        public int getRange()
        {
            return this._range;
        }

        public wasBlocked getBlocked()
        {
            return this._blocked;
        }
    }
}
