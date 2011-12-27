using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    class Building : Entity
    {
        
        public Building(int reactionTime, reactionFunction reaction, int health,  
            Vector size, Affiliation loyalty, Sight sight)
            : base(reactionTime, reaction, health, entityType.BUILDING, size, loyalty, sight, Visibility.SOLID)
        {
        }


    }
}
