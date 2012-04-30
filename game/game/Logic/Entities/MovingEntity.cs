using System.Collections.Generic;
namespace Game.Logic.Entities
{
    abstract class MovingEntity : Entity
    {
        /******************
        class consts
        ****************/
        
        const int AMOUNT_OF_MOVES_FOR_STEP = 100;

        /******************
        class members
        ****************/

        private readonly int _speed;
        private int _steps = 0;
        private LinkedList<Direction> _path;
        private Direction _headed;
        private MovementType _wayToMove = MovementType.GROUND;

        /******************
        constructors
        ****************/

        protected MovingEntity(int reactionTime, reactionFunction reaction, int health, entityType type, Vector size, Affiliation loyalty, Visibility visibility, Sight sight, int speed, LinkedList<Direction> path)
            : base(reactionTime, reaction, health, type, size, loyalty)
        {
            this._speed = speed;
            this._path = path;
            this._headed = Direction.DOWN;
        }

        /******************
        Getters & setters
        ****************/

        internal int Speed
        {
            get { return _speed; }
        }

        internal LinkedList<Direction> Path
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
            if (this.Path.Count > 0 ) return this.Path.First.Value;
            return this._headed;
        }

        internal virtual void moveResult(bool result)
        {
            if (result)
            {
                _steps -= AMOUNT_OF_MOVES_FOR_STEP;
                this._headed = this.getDirection();
                Path.RemoveFirst();
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
