using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;

namespace Game.Graphic_Manager
{
    class DisplayManager
    {
        RenderWindow _mainWindow;
        Game.Buffers.DisplayBuffer _buffer;
        HashSet<Sprite> displayedSprites;
        HashSet<Sprite> removedSprites;
        HashSet<Animation> animations;
        HashSet<Animation> doneAnimations = new HashSet<Animation>();
        Sprite back;


        public DisplayManager(uint x, uint y, uint bits, Game.Buffers.DisplayBuffer buffer, Image background)
        {
            this._buffer = buffer;
            this._mainWindow = new RenderWindow(new VideoMode(x, y, bits), "main display");
            this._mainWindow.SetActive(false);
            back = new Sprite(background);
            displayedSprites = new HashSet<Sprite>();
            removedSprites = new HashSet<Sprite>();
            animations = new HashSet<Animation>();

            this._mainWindow.Closed += new EventHandler(OnClosed);
            this._mainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
        }

        private void findSpritesToDisplay()
        {
            this.displayedSprites.UnionWith(this._buffer.newSpritesToDisplay());
            //TODO - see if we need to convert the sprite's position according to view
        }

        private void findSpritesToRemove()
        {
            this.removedSprites.UnionWith(this._buffer.spritesToRemove());
        }

        private void updateAnimations()
        {
            this.animations.UnionWith(this._buffer.getAnimations());
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
            this._mainWindow.Clear();
            this._mainWindow.Draw(back);
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
            
            foreach (Animation animation in this.animations)
            {
                this.removedSprites.Remove(animation.current());
                if (animation.isDone())
                {
                    doneAnimations.Add(animation);
                }
                else
                {
                    this.removedSprites.Add(animation.current());
                    this.displayedSprites.Add(animation.getNext());
                }
            }

            foreach (Animation animation in doneAnimations)
            {
                this.animations.Remove(animation);
            }
            doneAnimations.Clear();
        }

        private void removeSprites()
        {
            foreach (Sprite sprite in this.removedSprites)
            {
                this.displayedSprites.Remove(sprite);
            }
            this.removedSprites.Clear();
        }

        public void display()
        {
            _mainWindow.Display();
        }

        public void run()
        {
            while (true)
            {
                //Console.Out.WriteLine("display loop");
                loop();
                this._mainWindow.DispatchEvents();
                

            }
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Window window = (Window)sender;
            Console.Out.WriteLine("key pressed");
            if (e.Code == KeyCode.Escape)
                window.Show(window.IsOpened());
        }

    }
}
