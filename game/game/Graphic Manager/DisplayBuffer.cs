using SFML.Graphics;
using SFML.Window;
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
        const int amountOfReapeatingSpritesInAnimation = 1;
        const int amountOfDecals = 10;

        /******************
        Class members
        ****************/
        
        private HashSet<Sprite> displaySprites;
        private HashSet<Sprite> removedSprites;
        private LinkedList<Decal> decals;
        private List<Decal> doneDecals = new List<Decal>();
        private HashSet<Game.Graphic_Manager.Animation> newAnimations;
        private TextureFinder finder;
        private HashSet<BufferEvent> actions;
        private List<ExternalEntity> visibleEntities;
        private bool updated;

        /******************
        Constructors
        ****************/

        public DisplayBuffer()
        {
            this.removedSprites = new HashSet<Sprite>();
            this.displaySprites = new HashSet<Sprite>();
            this.decals = new LinkedList<Decal>();
            this.newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
            this.finder = new SpriteFinder();
            this.actions = new HashSet<BufferEvent>();
            this.visibleEntities = new List<ExternalEntity>();
            this.updated = false;
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
                temp.Position = new Vector2f(x0, y0);
                for (int i = 0; i < amountOfReapeatingSpritesInAnimation; i++)
                {
                    ans.AddLast(temp);
                }
                rightLeft = false;
                upDown = false;
            }
            return new Animation(ans);
        }

        public bool Updated
        {
            get { return updated; }
            set { updated = value; }
        }

        /************
         * input methods
         ************/

        internal void receiveActions(List<BufferEvent> _actions)
        {
            this.actions.UnionWith(_actions);
        }

        private void addDecal(Decal decal)
        {
            this.decals.AddLast(decal);
            if (decals.Count > amountOfDecals)
            {
                Decal removed = decals.First.Value;
                decals.RemoveFirst();
                this.removedSprites.Add(removed.getDecal());
            }
        }

        internal void receiveVisibleEntities(List<ExternalEntity> visibleExternalEntityList)
        {
            this.visibleEntities = visibleExternalEntityList;
        }

        private void displayDecals()
        {
            foreach (Decal decal in decals)
            {
                if (decal.isDone())
                {
                    doneDecals.Add(decal);
                    this.removedSprites.Add(decal.getDecal());
                }
                else
                    this.displaySprites.Add(decal.getDecal());
            }
            foreach (Decal decal in doneDecals)
                decals.Remove(decal);
            doneDecals.Clear();
        }

        /************
         * output methods
         ************/

        internal void analyzeData()
        {
            clearInfo();
            analyzeEntities();
            analyzeActions();
            displayDecals();
            removeSprites();
        }

        private void removeSprites()
        {
            foreach (Sprite temp in this.removedSprites)
                this.displaySprites.Remove(temp);
        }

        private void analyzeActions()
        {
            foreach (BufferEvent action in actions)
            {
                switch (action.type())
                {
                    case BufferType.DESTROY:
                        ExternalEntity ent = ((DestroyEvent)action).Ent;
                        Sprite temp = this.finder.remove(ent);
                        this.removedSprites.Add(temp);
                        if (ent.Type != entityType.PERSON)
                            this.newAnimations.Add(this.finder.generateDestoryResults(((DestroyEvent)action).Area, ent.Type));
                        Decal decal = new Decal();
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

                        decal.setLocation(ent.Position);
                        this.addDecal(decal);
                        break;
                    case BufferType.MOVE:
                        ExternalEntity mover = ((MoveEvent)action).Mover;
                        Sprite movement = this.finder.getSprite(mover);
                        movement.Rotation = ((MoveEvent)action).Rotation;
                        Vector2f position = new Vector2f(mover.Position.X, mover.Position.Y);
                        movement.Position = position;
                        //TODO - generate turning animation
                        break;
                    case BufferType.CREATE:
                        //TODO
                        break;
                    case BufferType.SHOT:
                        this.newAnimations.Add(this.createNewShot(((ShotEvent)action).Shot, ((ShotEvent)action).Exit, ((ShotEvent)action).Target));
                        break;
                }
            }
            actions.Clear();
        }

        private void analyzeEntities()
        {
            foreach (ExternalEntity ent in visibleEntities)
            {
                Sprite temp = finder.getSprite(ent);
                temp.Position = new Vector2f(ent.Position.X, ent.Position.Y);
                displaySprites.Add(temp);
            }
            visibleEntities.Clear();
        }


        internal HashSet<Sprite> newSpritesToDisplay()
        {
            return displaySprites;
        }

        internal HashSet<Sprite> spritesToRemove()
        {
            return this.removedSprites;
        }

        internal IEnumerable<Animation> getAnimations()
        {
            return newAnimations; 
        }

        internal void clearInfo()
        {
            newAnimations.Clear();
            this.removedSprites.Clear();
            this.displaySprites.Clear();
        }

    }


}
