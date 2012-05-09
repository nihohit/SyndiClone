using System.Collections.Generic;
namespace Game.Logic.Entities
{
    abstract class MovingEntity : Entity
    {
        /******************
        class consts
        ****************/
        
        const int AMOUNT_OF_MOVES_FOR_STEP = 70;

        /******************
        class members
        ****************/

        private readonly int _speed;
        private int _steps = 0;
        private List<Direction> _path;
        private Direction _headed;
        private MovementType _wayToMove = MovementType.GROUND;

        /******************
        constructors
        ****************/

        protected MovingEntity(int reactionTime, reactionFunction reaction, int health, entityType type, Vector size, Affiliation loyalty, Visibility visibility, Sight sight, int speed, List<Direction> path, Direction headed)
            : base(reactionTime, reaction, health, type, size, loyalty)
        {
            this._speed = speed;
            this._path = path;
            this._headed = headed;
        }

        /******************
        Getters & setters
        ****************/

        internal int Speed
        {
            get { return _speed; }
        }

        internal List<Direction> Path
        {
            get { return _path; }
            set { _path = value; }
        }


        /******************
        Methods
        ****************/

        internal bool ReadyToMove(int speed)
        {
            bool ans =  reachAffect(AMOUNT_OF_MOVES_FOR_STEP, _steps, speed);
            if (ans)
            {
                _steps -= AMOUNT_OF_MOVES_FOR_STEP;
            }
            else
            {
                _steps += speed;
            }
            return ans;
        }

        //TODO - incorrect
        internal virtual Direction getDirection()
        {
            if (this.Path.Count > 0 ) return this.Path[0];
            return this._headed;
        }

        internal virtual void moveResult(bool result)
        {
            if (result)
            {
                _steps -= AMOUNT_OF_MOVES_FOR_STEP;
                this._headed = this.getDirection();
                Path.RemoveAt(0);
            }
            else
            {
                _steps -= _speed;
            }

        }

        internal bool needFlip()
        {
            return this._headed != this.getDirection();
        }

        internal void flip()
        {
            this._size = new Vector(this._size.Y, this._size.X);
        }

        internal MovementType WayToMove
        {
            get { return _wayToMove; }
            set { _wayToMove = value; }
        }

    }
}
