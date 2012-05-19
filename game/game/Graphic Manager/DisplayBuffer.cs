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
        //TODO - debug, remove
        const int amountOfReapeatingSpritesInAnimation = 1;
        readonly uint amountOfDecals = FileHandler.getUintProperty("decal amount", FileAccessor.DISPLAY);

        /******************
        Class members
        ****************/
        private readonly Sprite selection = new Sprite(new Texture("images/UI/selection.png"));

        private HashSet<Sprite> displaySprites = new HashSet<Sprite>();
        private HashSet<Sprite> removedSprites = new HashSet<Sprite>();
        private List<Decal> decals = new List<Decal>();
        private List<Decal> doneDecals = new List<Decal>();
        private HashSet<Game.Graphic_Manager.Animation> newAnimations = new HashSet<Game.Graphic_Manager.Animation>();
        private TextureFinder finder = new SpriteFinder();
        private HashSet<BufferEvent> actions = new HashSet<BufferEvent>();
        private List<ExternalEntity> visibleEntities = new List<ExternalEntity>();
        private bool updated = false;
        private bool selected = false;
        private bool deselected = false;
        
        private ExternalEntity selectedEntity; //For UI purposes

        /******************
        Constructors
        ****************/

        public DisplayBuffer()
        {
            selection.Origin = new Vector2f(selection.Texture.Size.X/2, selection.Texture.Size.Y/2);
        }

        /******************
        Methods
        ****************/

        /************
         * output methods
         ************/

        internal void analyzeData()
        {
            clearInfo();
            analyzeEntities();
            analyzeActions();
            displayDecals();
            UIDisplay();
            removeSprites();
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

        public bool Updated
        {
            get { return updated; }
            set { updated = value; }
        }

        /************
         * input methods
         ************/

        internal void receiveVisibleEntities(List<ExternalEntity> visibleExternalEntityList)
        {
            this.visibleEntities = visibleExternalEntityList;
        }

        internal void receiveActions(List<BufferEvent> _actions)
        {
            this.actions.UnionWith(_actions);
        }

        /**********
         * internal methods
         *********/

        /*
         * This function is a repeat of the Berensham's line algorithm, this time painting a straight line. 
         */
        private Animation createNewShot(ShotType shot, Point exit, Point target)
        {
            List<Sprite> ans = new List<Sprite>();
            int x0 = exit.X;
            int y0 = exit.Y;
            int x1 = target.X;
            int y1 = target.Y;
            int dx = System.Math.Abs(x1 - x0);
            int dy = System.Math.Abs(y1 - y0);
            int sx, sy, e2;
            if (x0 < x1) sx = 1; else sx = -1;
            if (y0 < y1) sy = 1; else sy = -1;
            int err = dx - dy;
            Sprite temp = null;
            while (!(x0 == x1 & y0 == y1))
            {
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }

                temp = this.finder.getShot(shot);
                //TODO - rotate the shot
                temp.Position = new Vector2f(x0, y0);
                for (int i = 0; i < amountOfReapeatingSpritesInAnimation; i++)
                {
                    ans.Add(temp);
                }
            }
            return new Animation(ans);
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

                    case BufferType.CREATE:
                        //TODO
                        break;

                    case BufferType.SHOT:
                        this.newAnimations.Add(this.createNewShot(((ShotEvent)action).Shot, ((ShotEvent)action).Exit, ((ShotEvent)action).Target));
                        break;

                    case BufferType.UNIT_SELECT:
                        this.selection.Position = ((BufferUnitSelectEvent)action).Coords.toVector2f();
                        this.selectedEntity = ((BufferUnitSelectEvent)action).Ent;
                        this.selected = true;
                        break;

                    case BufferType.DESELECT:
                        this.selected = false;
                        this.deselected = true;
                        break;

                    case BufferType.SETPATH:
                        Sprite toRemove = finder.removePath(((BufferSetPathActionEvent)action).Ent);
                        if (toRemove != null)
                        {
                            this.removedSprites.Add(toRemove);
                        }
                        finder.setPath(((BufferSetPathActionEvent)action).Ent, ((BufferSetPathActionEvent)action).Path, ((BufferSetPathActionEvent)action).Position);
                        break;
                }
            }
            actions.Clear();
        }

        private void addDecal(Decal decal)
        {
            this.decals.Add(decal);
            if (decals.Count > amountOfDecals)
            {
                Decal removed = decals[0];
                decals.Remove(removed);
                this.removedSprites.Add(removed.getDecal());
            }
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

        private void UIDisplay()
        {
            if (this.selected)
            {
                this.displaySprites.Add(selection);
                this.displayAdditionalInfo(selectedEntity);
                return;
            }
            if (this.deselected)
            {
                this.removedSprites.Add(this.selection);
                this.removeAdditionalInfo(selectedEntity);
                this.deselected = false;
            }
        }

        private void removeAdditionalInfo(ExternalEntity selectedEntity)
        {
            if (this.finder.hasPath(selectedEntity))
            {
                this.removedSprites.Add(this.finder.getPath(selectedEntity));
            }
        }

        private void displayAdditionalInfo(ExternalEntity selectedEntity)
        {
            if (this.finder.hasPath(selectedEntity))
            {
                this.removedSprites.Add(this.finder.getPath(selectedEntity));
                this.displaySprites.Add(this.finder.nextPathStep(selectedEntity));
            }

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

    }


}
