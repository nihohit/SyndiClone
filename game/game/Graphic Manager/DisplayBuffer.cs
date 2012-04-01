using SFML.Graphics;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Entities;
using Game.Graphic_Manager;


namespace Game.Buffers
{
    public class DisplayBuffer : Game.Buffers.Buffer
    {

        /******************
        Class consts
        ****************/
        const int amountOfReapeatingSpritesInAnimation = 100;

        /******************
        Class Fields
        ****************/
        
        private HashSet<Sprite> displaySprites;
        private HashSet<Sprite> removedSprites;
        private HashSet<Sprite> decals;
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
            this.decals = new HashSet<Sprite>();
            this.newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
            this.finder = new SpriteFinder();
        }

        /******************
        Methods
        ****************/

        /**********
         * internal methods
         *********/
        private Animation createNewShot(ShotType shot, Point exit, Point target)
        {
            LinkedList<Sprite> ans = new LinkedList<Sprite>();
            int x0 = exit.X;
            int y0 = exit.Y;
            int x1 = target.X;
            int y1 = target.Y;
            int dx = Vector.abs(x1 - x0);
            int dy = Vector.abs(y1 - y0);
            int sx, sy, e2;
            if (x0 < x1) sx = 1; else sx = -1;
            if (y0 < y1) sy = 1; else sy = -1;
            int err = dx - dy;
            bool rightLeft = false;
            bool upDown = false;
            Sprite temp = null;
            while (!(x0 == x1 & y0 == y1))
            {
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                    rightLeft = true;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                    upDown = true;
                }

                temp = this.finder.getShot(shot, (upDown & rightLeft));
                //TODO - rotate the shot
                temp.Center = new Vector2(x0, y0);
                for (int i = 0; i < amountOfReapeatingSpritesInAnimation; i++)
                {
                    ans.AddLast(temp);
                }
                rightLeft = false;
                upDown = false;
            }
            return new Animation(ans);
        }

        private Graphic_Manager.Animation generateDestoryResults(Area area, entityType type)
        {
            //TODO - decide if & when do decals go away. 
            //TODO - missing function
            return new Graphic_Manager.Animation(area, type);
        }

        /************
         * input methods
         ************/

        internal void receiveActions(List<BufferEvent> actions)
        {
            foreach (BufferEvent action in actions)
            {
                switch (action.type())
                {
                    case BufferType.DESTROY:
                        ExternalEntity ent = ((DestroyEvent)action).Ent;
                        this.spriteLoopFinder.Remove(ent);

                        if(ent.Type != entityType.PERSON)
                            this.newAnimations.Add(this.generateDestoryResults(((DestroyEvent)action).Area, ent.Type));

                        Decal decal;
                        switch (ent.Type)
                        {
                            case (entityType.BUILDING):
                                decal = new Decal(DecalType.RUBBLE);
                                break;
                            case (entityType.VEHICLE):
                                decal = new Decal(DecalType.WRECKAGE);
                                break;
                            case (entityType.PERSON):
                                decal = new Decal(DecalType.BLOOD);
                                break;

                        }
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
                        this.newAnimations.Add(this.createNewShot(((ShotEvent)action).Shot, ((ShotEvent)action).Exit, ((ShotEvent)action).Target));
                        break;
                }
            }
        }

        internal void receiveVisibleEntities(List<ExternalEntity> visibleExternalEntityList)
        {
            displaySprites.Clear();
            foreach (ExternalEntity ent in visibleExternalEntityList)
            {
                Sprite temp = finder.getSprite(ent);
                temp.Position = new Vector2(ent.Position.X, ent.Position.Y);
                //TODO - find a way to make sure that the sprite's position is correct.
                displaySprites.Add(temp);
            }
        }

        /************
         * output methods
         ************/

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

        internal IEnumerable<Animation> getAnimations()
        {
            HashSet<Animation> ans = new HashSet<Animation>(newAnimations);
            newAnimations.Clear();
            return ans; 
        }
    }


}
