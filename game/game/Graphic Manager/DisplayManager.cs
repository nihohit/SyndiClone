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
         * class fields
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
        }

        /************
         * Class methods
         *************/

        /***********
         * communication
         ************/

        private void updateInfo()
        {
            if (input.GraphicInput)
            {
                List<BufferEvent> list = input.graphicEvents();
                foreach (BufferEvent action in list)
                {
                    switch (action.type())
                    {
                        case BufferType.MOUSEMOVE:
                            this.crosshair.Position = ((BufferMouseMoveEvent)action).Coords;
                            //Console.Out.WriteLine(((BufferMouseMoveEvent)action).X + " " + ((BufferMouseMoveEvent)action).Y);
                            break;
                    }
                }
                
            }
            lock (this._buffer)
            {
                findSpritesToRemove();
                findSpritesToDisplay();
                updateAnimations();
            }
            removeSprites();
            enterAnimations();
            displaySprites();
            drawUI();
        }

        private void drawUI()
        {
            crosshair.Position = _mainWindow.ConvertCoords(Mouse.GetPosition(_mainWindow));
            this._mainWindow.Draw(crosshair);
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
            this._mainWindow.Clear();
            this._mainWindow.Draw(background);
            this.updateInfo();
            this.display();
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

    }
}
