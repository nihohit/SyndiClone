using System.Collections.Generic;
namespace Game.Logic.Entities
{
    public abstract class MovingEntity : Entity
    {
        #region consts

        const int AMOUNT_OF_MOVES_FOR_STEP = 70;

        #endregion

        #region fields

        private int m_steps = 0;
        private Direction m_headed;

        #endregion

        #region properties

        public int Speed { get; private set; }

        public List<Direction> Path { get; set; }

        public MovementType WayToMove { get; private set; }

        #endregion

        #region constructor

        protected MovingEntity(int reactionTime, reactionFunction reaction, int health, entityType type, Vector size, Affiliation loyalty, Visibility visibility, Sight sight, int speed, List<Direction> path, Direction headed)
            : base(reactionTime, reaction, health, type, size, loyalty)
        {
            Speed = speed;
            Path = path;
            m_headed = headed;
            WayToMove = MovementType.GROUND;
        }

        #endregion

        #region Entity overrides

        protected override void Upgrade(System.Collections.Generic.List<Upgrades> list)
        {
            foreach (Upgrades upgrade in list)
            {
                switch (upgrade)
                {
                    case (Upgrades.FLYER):
                        WayToMove = MovementType.FLYER;
                        break;
                    case (Upgrades.HOVER):
                        WayToMove = MovementType.HOVER;
                        break;
                    case (Upgrades.CRUSHER):
                        WayToMove = MovementType.CRUSHER;
                        break;
                }
            }
            base.Upgrade(list);
        }

        #endregion

        #region public methods

        public bool ReadyToMove(int speed)
        {
            bool ans =  ReachAffect(AMOUNT_OF_MOVES_FOR_STEP, m_steps, speed);
            if (ans)
            {
                m_steps -= AMOUNT_OF_MOVES_FOR_STEP;
            }
            else
            {
                m_steps += speed;
            }
            return ans;
        }

        //TODO - incorrect
        public virtual Direction GetDirection()
        {
            if (Path.Count > 0 ) return Path[0];
            return m_headed;
        }

        public virtual void MoveResult(bool result)
        {
            if (result)
            {
                m_steps -= AMOUNT_OF_MOVES_FOR_STEP;
                m_headed = GetDirection();
                Path.RemoveAt(0);
            }
            else
            {
                m_steps -= Speed;
            }

        }

        public bool NeedFlip()
        {
            return m_headed != GetDirection();
        }

        public void Flip()
        {
            Size = new Vector(Size.Y, Size.X);
        }

        #endregion
    }
}
