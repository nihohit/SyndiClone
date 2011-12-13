using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Game.Graphic_Manager
{
    class DisplayManager
    {
        RenderWindow _mainWindow;
        Game.Buffers.DisplayBuffer _buffer;
        HashSet<Sprite> displayedSprites;
        HashSet<Sprite> removedSprites;
        HashSet<Animation> animations;


        public DisplayManager(uint x, uint y, uint bits, Game.Buffers.DisplayBuffer buffer, Image background)
        {
            this._buffer = buffer;
            this._mainWindow = new RenderWindow(new VideoMode(x, y, bits), "main display");
            Sprite back = new Sprite(background);
            this._mainWindow.Draw(back);
            displayedSprites = new HashSet<Sprite>();
            removedSprites = new HashSet<Sprite>();
            animations = new HashSet<Animation>();

        }

        private void findSpritesToDisplay()
        {
            this.displayedSprites.UnionWith(this._buffer.newSpritesToDisplay());
            //TODO - see if we need to convert the sprite's position according to view
        }

        private void findSpritesToRemove()
        {
            this.removedSprites = new HashSet<Sprite>(this._buffer.spritesToRemove());
        }

        private void updateAnimations()
        {
            //TODO - missing function
        }

        private void displaySprites()
        {
            foreach (Sprite sprite in this.displayedSprites)
            {
                this._mainWindow.Draw(sprite);
            }
        }

        public void loop()
        {
            this.updateInfo();
            this.display();
        }

        private void updateInfo()
        {
            lock (this._buffer)
            {
                findSpritesToRemove();
                findSpritesToDisplay();
                updateAnimations();
            }
            removeSprites();
            enterAnimations();
            displaySprites();
        }

        private void enterAnimations()
        {
            HashSet<Animation> done = new HashSet<Animation>();
            foreach (Animation animation in this.animations)
            {
                this.removedSprites.Remove(animation.current());
                this.displayedSprites.Add(animation.getNext());
                if (animation.isDone())
                {
                    done.Add(animation);
                }
            }

            foreach (Animation animation in done)
            {
                this.animations.Remove(animation);
            }
        }

        private void removeSprites()
        {
            foreach (Sprite sprite in this.removedSprites)
            {
                this.displayedSprites.Remove(sprite);
            }
        }

        public void display()
        {
            _mainWindow.Display();
        }


    }
}
