using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Entities
{
    class Building : Entity
    {
        private readonly SFML.Graphics.Sprite image;
        
        public Building(int reactionTime, reactionFunction reaction, int health,  
            Vector size, Affiliation loyalty, Sight sight, SFML.Graphics.Sprite img)
            : base(reactionTime, reaction, health, entityType.BUILDING, size, loyalty, sight, Visibility.SOLID)
        {
            image = img;
        }

        public SFML.Graphics.Sprite Image
        {
            get { return image; }
        }

    }
}
