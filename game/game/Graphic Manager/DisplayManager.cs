using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;
using Game.Buffers;

namespace Game.Graphic_Manager
{
    class Game_Screen
    {
        /************
         * class members
         ***************/
        private RenderWindow _mainWindow;
        private Game.Buffers.DisplayBuffer _buffer;
        private InputBuffer input;
        private HashSet<Sprite> displayedSprites;
        private HashSet<Sprite> removedSprites;
        private HashSet<Animation> animations;
        private HashSet<Animation> doneAnimations;
        private Sprite background;
        private Sprite crosshair;
        private View UIview;

        System.Diagnostics.Stopwatch remove = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch displayWatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch synch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch update = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch other = new System.Diagnostics.Stopwatch();
        int runs = 0;

        /************
         * constructor
         ***************/
        public Game_Screen(Game.Buffers.DisplayBuffer buffer, Texture _background, RenderWindow window, InputBuffer _input)
        {
            this._buffer = buffer;
            this.input = _input;
            background = new Sprite(_background);
            displayedSprites = new HashSet<Sprite>();
            removedSprites = new HashSet<Sprite>();
            animations = new HashSet<Animation>();
            doneAnimations = new HashSet<Animation>();
            this._mainWindow = window;
            this.crosshair = new Sprite(new Texture("images/UI/crosshairs.png"));
            this.crosshair.Origin = new Vector2f(this.crosshair.Texture.Size.X / 2, this.crosshair.Texture.Size.Y / 2);
            this.UIview = new View(new Vector2f(_background.Size.X/2, _background.Size.Y/2), new Vector2f(_background.Size.X, _background.Size.Y));
            this.displayWatch.Start();
        }

        /************
         * Class methods
         *************/

        /***********
         * communication
         ************/

        private void updateInfo()
        {
            lock (input)
            {
                if (input.GraphicInput)
                {
                    List<BufferEvent> list = input.graphicEvents();
                    foreach (BufferEvent action in list)
                    {
                        if (action.type() == BufferType.ENDGAME)
                            displayStats();
                    }

                }
            }
            this.synch.Start();
            if (this._buffer.Updated)
            {
                lock (this._buffer)
                {
                    this._buffer.analyzeData();
                    this.findSpritesToRemove();
                    this.findSpritesToDisplay();
                    this.updateAnimations();
                    this._buffer.Updated = false;
                }
            }
            this.synch.Stop();
            this.remove.Start();
            removeSprites();
            this.remove.Stop();
            this.update.Start();
            enterAnimations();
            displaySprites();
            drawUI();
            this.update.Stop();
        }

        private void drawUI()
        {
            crosshair.Position = _mainWindow.ConvertCoords(Mouse.GetPosition(_mainWindow));
            this._mainWindow.Draw(crosshair);
        }



        private void findSpritesToDisplay()
        {
            this.displayedSprites.UnionWith(this._buffer.newSpritesToDisplay());
        }

        private void findSpritesToRemove()
        {
            this.removedSprites.UnionWith(this._buffer.spritesToRemove());
        }

        private void updateAnimations()
        {
            this.animations.UnionWith(this._buffer.getAnimations());
        }


        /***********
         * main loop
         ***********/
        /*
        public void run()
        {
            while (active)
            {
                loop();
            }
        }*/

        public void loop()
        {
            this.other.Start();
            this._mainWindow.Clear();
            this._mainWindow.Draw(background);
            this.other.Stop();
            this.updateInfo();
            this.displayWatch.Start();
            this.display();
            this.displayWatch.Stop();
            runs++;
        }


        /***********
         * info handling
         ***********/

        private void displaySprites()
        {
            foreach (Sprite sprite in this.displayedSprites)
            {
                this._mainWindow.Draw(sprite);
            }
        }

        private void enterAnimations()
        {
            
            foreach (Animation animation in this.animations)
            {
                this.removedSprites.Add(animation.current());
                if (animation.isDone())
                {
                    doneAnimations.Add(animation);
                }
                else
                {
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

        internal void displayStats()
        {
            this.displayWatch.Stop();
            Console.Out.WriteLine("synch was " + this.synch.Elapsed + " , display was " + this.displayWatch.Elapsed + " , update was " + this.update.Elapsed + " , remove was " + this.remove.Elapsed + " , other was " + other.Elapsed);
            Console.Out.WriteLine("amount of graphic loops: " + runs + " average milliseconds per frame: " + this.displayWatch.ElapsedMilliseconds/runs);
        }
    }
}
