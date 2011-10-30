using System.Collections.Generic;
namespace Game.Logic.Entities
{
    abstract class MovingEntity : Entity
    {
        const int AMOUNT_OF_MOVES_FOR_STEP = 100;
        private readonly int _speed;
        private int _steps = 0;
        private LinkedList<Point> _path;

        protected MovingEntity(reactionFunction reaction, int health, entityType type, Vector size, Affiliation loyalty, Visibility visibility, Sight sight, int speed, LinkedList<Point> path)
            : base(reaction, health, type, size, loyalty, sight, visibility)
        {
            this._speed = speed;
            this._path = path;
        }

        internal int Speed
        {
            get { return _speed; }
        } 

        internal LinkedList<Point> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        internal virtual Point tryMove()
        {
            _steps += _speed;
            Point ans;
            if (_steps > AMOUNT_OF_MOVES_FOR_STEP)
            {
                ans = Path.First.Next.Value;
            }
            else
            {
                ans = Path.First.Value;
            }
            return ans;
        }

        internal virtual void moveResult(bool result)
        {
            if (result)
            {
                _steps -= AMOUNT_OF_MOVES_FOR_STEP;
                Path.RemoveFirst();
            }
            else
            {
                _steps -= _speed;
            }

        }
    }
}
