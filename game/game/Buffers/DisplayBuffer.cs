using SFML.Graphics;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Entities;


namespace Game.Buffers
{
    class DisplayBuffer : Game.Buffers.Buffer
    {

        /******************
        Class Fields
        ****************/
        
        private readonly Dictionary<Entity, SpriteLoop> spriteFinder;
        private HashSet<Sprite> displaySprites;
        private HashSet<Sprite> removedSprites;
        private HashSet<Sprite> newDecalsSprites;
        private HashSet<Game.Graphic_Manager.Animation> newAnimations;

        /******************
        Constructors
        ****************/

        DisplayBuffer()
        {
            this.spriteFinder = new Dictionary<Entity, SpriteLoop>();
            this.removedSprites = new HashSet<Sprite>();
            this.displaySprites = new HashSet<Sprite>();
            this.newDecalsSprites = new HashSet<Sprite>();
            this.newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
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
            displaySprites.Clear();
            return ans;
        }

        internal void receiveActions(List<BufferEvent> actions)
        {
            foreach (BufferEvent action in actions)
            {
                switch (action.type())
                {
                    case BufferType.DESTROY:
                        Entity ent = ((DestroyEvent)action).Ent;
                        this.spriteFinder.Remove(ent);
                        this.newAnimations.Add(this.generateExplosion(((DestroyEvent)action).Area, ent.Type));
                        break;
                    case BufferType.MOVE:
                        MovingEntity mover = ((MoveEvent)action).Mover;
                        Sprite toRemove = this.spriteFinder[mover].getSprite();
                        this.removedSprites.Add(toRemove);
                        Sprite movement = this.spriteFinder[mover].Next();
                        movement.Rotation = ((MoveEvent)action).Rotation;
                        Point pos = ((MoveEvent)action).Exit.Entry; /*The reason for the double calling is 
                                                                     * that we call the area to which the animation leads, 
                                                                     * and then the entry point to that area
                                                                     */
                        Vector2 position = new Vector2(pos.X, pos.Y);
                        movement.Position = position;
                        //TODO - generate turning animation
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

        internal void receiveVisibleEntities(List<Entity> visibleEntityList)
        {
            displaySprites.Clear();
            foreach (Entity ent in visibleEntityList)
            {
                Sprite temp = this.spriteFinder[ent].getSprite();
                displaySprites.Add(temp);
            }
        }
    }
}
