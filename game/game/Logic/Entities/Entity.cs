

namespace Game.Logic.Entities
{
    internal abstract class Entity

    {
        private int _health;
        private readonly Vector _size;
        private readonly entityType _type;
        private Reaction _reaction;
        private Affiliation _loyalty;
        private readonly Sight _sight;
        private Visibility _visible;

        internal Visibility Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        protected Entity(int health,entityType type, Vector size, Affiliation loyalty, Sight sight, Visibility visibility)
        {
            this._health = health;
            this._size = size;
            this._type = type;
            this._loyalty = loyalty;
            this._sight = sight;
            this._reaction = new Reaction (null, Action.IGNORE);
            this._visible = visibility;
        }

        internal Reaction Reaction
        {
            get { return _reaction; }
            set { _reaction = value; }
        }

        internal Sight Sight
        {
            get { return _sight; }
        } 

        internal entityType Type
        {
            get { return _type; }
        }

        internal Vector Size
        {
            get { return _size; }
        }

        internal bool hit(int damage)
        {
            this._health -= damage;
            return (this._health <= 0);
        }

        internal abstract bool blocksVision();

    }
}
