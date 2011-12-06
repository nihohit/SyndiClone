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


        public DisplayManager(uint x, uint y, uint bits, Game.Buffers.DisplayBuffer buffer)
        {
            this._buffer = buffer;
            this._mainWindow = new RenderWindow(new VideoMode(x, y, bits), "main display");
        }

        private void findSpritesToDisplay()
        {
            this.displayedSprites.UnionWith(this._buffer.newSpritesToDisplay());
        }

        private void findSpritesToRemove()
        {
            this.removedSprites = new HashSet<Sprite>(this._buffer.newSpritesToDisplay());
        }

        private void newAnimations()
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
                newAnimations();
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
