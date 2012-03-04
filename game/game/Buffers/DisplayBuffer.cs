using SFML.Graphics;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Entities;


namespace Game.Buffers
{
        public class DisplayBuffer : Game.Buffers.Buffer
    {

        /******************
        Class Fields
        ****************/
        
        private HashSet<Sprite> displaySprites;
        private HashSet<Sprite> removedSprites;
        private HashSet<Sprite> newDecalsSprites;
        private HashSet<Game.Graphic_Manager.Animation> newAnimations;
        private ImageFinder finder;
        private Dictionary<ExternalEntity, SpriteLoop> spriteLoopFinder;

        /******************
        Constructors
        ****************/

        public DisplayBuffer()
        {
            this.spriteLoopFinder = new Dictionary<ExternalEntity, SpriteLoop>();
            this.removedSprites = new HashSet<Sprite>();
            this.displaySprites = new HashSet<Sprite>();
            this.newDecalsSprites = new HashSet<Sprite>();
            this.newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
            this.finder = new SpriteFinder();
        }

        /******************
        Methods
        ****************/

        internal HashSet<Sprite> newSpritesToDisplay()
        {
            HashSet<Sprite> ans = new HashSet<Sprite>(displaySprites);
            displaySprites.Clear();
            return ans;
        }

        internal List<Sprite> spritesToRemove()
        {
            List<Sprite> ans = new List<Sprite>(removedSprites);
            removedSprites.Clear();
            return ans;
        }

        internal void receiveActions(List<BufferEvent> actions)
        {
            foreach (BufferEvent action in actions)
            {
                switch (action.type())
                {
                    case BufferType.DESTROY:
                        ExternalEntity ent = ((DestroyEvent)action).Ent;
                        this.spriteLoopFinder.Remove(ent);
                        this.newAnimations.Add(this.generateExplosion(((DestroyEvent)action).Area, ent.Type));
                        break;
                    case BufferType.MOVE:
                        ExternalEntity mover = ((MoveEvent)action).Mover;
                        Sprite movement = null;
                        if (mover.Type == Logic.entityType.VEHICLE)
                        {
                            Sprite toRemove = this.spriteLoopFinder[mover].getSprite();
                            this.removedSprites.Add(toRemove);
                            movement = this.spriteLoopFinder[mover].Next();
                        }
                        else
                        {
                            movement = this.finder.getSprite(mover);
                        }
                        movement.Rotation = ((MoveEvent)action).Rotation;
                        Vector2 position = new Vector2(mover.Position.X, mover.Position.Y);
                        movement.Position = position;
                        //TODO - generate turning animation
                        this.displaySprites.Add(movement);
                        break;
                    case BufferType.CREATE:
                        //TODO
                        break;
                    case BufferType.SHOT:
                        //TODO
                        break;
                }
            }
        }

        private Graphic_Manager.Animation generateExplosion(Area area, entityType type)
        {
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        internal void receiveVisibleEntities(List<ExternalEntity> visibleExternalEntityList)
        {
            displaySprites.Clear();
            foreach (ExternalEntity ent in visibleExternalEntityList)
            {
                Sprite temp = finder.getSprite(ent);
                temp.Position = new Vector2(ent.Position.Y, ent.Position.X);
                //TODO - find a way to make sure that the sprite's position is correct.
                displaySprites.Add(temp);
            }
        }

        
    }


}
