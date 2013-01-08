using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;
using Game.Buffers;

namespace Game.Graphic_Manager
{
    //This class is in charge of displaying an actual game instance. 
    class GameDisplay
    {
        /*TODO - right now there's a central problem, that some UI elements are handled by the manager 
         * and some by the buffer (basically, what depends on whether they demand knowledge of logic events is handled by the buffer and what 
         * demands knowledge of screen events is handled by the manager). 
         * This should be remedied - maybe move everything to the buffer. 

        /************
         * class members
         ***************/
        private Sprite crosshair = new Sprite(new Texture("images/UI/crosshairs.png"));
        private HashSet<Sprite> displayedSprites = new HashSet<Sprite>();
        private HashSet<Sprite> removedSprites = new HashSet<Sprite>();
        private HashSet<Animation> animations = new HashSet<Animation>();
        private HashSet<Animation> doneAnimations = new HashSet<Animation>();

        private RenderWindow _mainWindow;
        private Game.Buffers.DisplayBuffer _buffer;
        private InputBuffer input; //HACK - is this needed? maybe put that in the display buffer too?
        private Sprite background;
        private View UIview;

        //DEBUG & PERFORMANCE TOOLS
        //TODO - remove.
        System.Diagnostics.Stopwatch remove = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch displayWatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch synch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch update = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch other = new System.Diagnostics.Stopwatch();
        int runs = 0;

        /************
         * constructor
         * 
         ***************/
        public GameDisplay(Game.Buffers.DisplayBuffer buffer, Texture _background, RenderWindow window, InputBuffer _input)
        {
            this._buffer = buffer;
            this.input = _input;
            background = new Sprite(_background);
            this._mainWindow = window;
            this.crosshair.Origin = new Vector2f(this.crosshair.Texture.Size.X / 2, this.crosshair.Texture.Size.Y / 2);
            this.UIview = new View(new Vector2f(_background.Size.X/2, _background.Size.Y/2), new Vector2f(_background.Size.X, _background.Size.Y));
            this.displayWatch.Start();
        }

        /************
         * Class methods
         *************/

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
         * communication
         ************/

        //This is the central 
        private void updateInfo()
        {
            handleInputBuffer();
            handleDisplayBuffer();
            this.remove.Start();
            removeSprites();
            this.remove.Stop();
            this.update.Start();
            enterAnimations();
            displaySprites();
            drawUI();
            this.update.Stop();
        }

        private void handleDisplayBuffer()
        {
            this.synch.Start();

            lock (this._buffer)
            {
                if (!this._buffer.Updated)
                {
                    //TODO - needs reviewing
                    System.Threading.Monitor.Wait(this._buffer);
                }
                this._buffer.analyzeData();
                this.findSpritesToRemove();
                this.findSpritesToDisplay();
                this.updateAnimations();
                this._buffer.Updated = false;
            }
            this.synch.Stop();
        }

        private void handleInputBuffer()
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


        //TODO - debug, remove
        internal void displayStats()
        {
            this.displayWatch.Stop();
            Console.Out.WriteLine("synch was " + this.synch.Elapsed + " , display was " + this.displayWatch.Elapsed + " , update was " + this.update.Elapsed + " , remove was " + this.remove.Elapsed + " , other was " + other.Elapsed);
            Console.Out.WriteLine("amount of graphic loops: " + runs + " average milliseconds per frame: " + this.displayWatch.ElapsedMilliseconds/runs);
        }
    }
}
