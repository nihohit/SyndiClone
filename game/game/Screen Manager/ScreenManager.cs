using SFML.Graphics;
using SFML.Window;
using System;
using Game.Buffers;
using System.Threading;

namespace Game.Screen_Manager
{
    static class ScreenManager
    {
        /***************
         * class consts
         **************/
        const float AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN = 30;
        const int SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING = 30;
        const int SCREEN_EDGE_MOUSE_SCROLL_RELATION = 4;
        const int SPEED_OF_MOUSE_SCROLL = 30;

        /**************
         * class fields
         *************/

        static private float minX = 400F, minY, topY, topX; //tthese represent the bounds on the size of the view
        static private float backgroundX, backgroundY;
        static private RenderWindow mainWindow;
        static private Buffers.InputBuffer input;
        static private Graphic_Manager.Game_Screen gameScreen;
        static private bool active = false, inGame = false, activeGame = false;
        static private FileReader reader = new FileReader();
        static private float xMove, yMove;
        static private float mouseX, mouseY;
        static Thread logicThread = null;
        static int mouseScrollAmount = 0;

        private static void initialise()
        {
            mainWindow = new RenderWindow(new VideoMode(reader.getScreenWidth(), reader.GetScreenHeight(), reader.getBits()), "main display");

            minY = minX * reader.GetScreenHeight() / reader.getScreenWidth();
            mainWindow.SetActive(false);
            mainWindow.Closed += new EventHandler(OnClosed);
            mainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            mainWindow.GainedFocus += new EventHandler(OnGainingFocus);
            mainWindow.LostFocus += new EventHandler(OnLosingFocus);
            mainWindow.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(Zooming);
            mainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseClick);
            mainWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(MouseMoved);
            mainWindow.SetMouseCursorVisible(false);
            mainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseButtonPressed);
            active = true;
        }

        //TODO - change to private, internally create the buffers.
        public static void startNewGame(int mapX, int mapY, int civAmount)
        {
            City_Generator.GameBoard city = City_Generator.CityFactory.createCity(mapX, mapY);
            Buffers.DisplayBuffer disp = new Buffers.DisplayBuffer();
            input = new Buffers.InputBuffer();
            Buffers.SoundBuffer sound = new Buffers.SoundBuffer();
            Logic.GameLogic logic = new Logic.GameLogic(disp, input, sound, city, civAmount);
            Texture background = city.Img;
            gameScreen = new Graphic_Manager.Game_Screen(disp, background, mainWindow, input);
            topX = background.Size.X;
            topY = background.Size.X * reader.GetScreenHeight() / reader.getScreenWidth();
            backgroundX = background.Size.X;
            backgroundY = background.Size.Y;
            inGame = true;
            activeGame = true;
            logicThread = new Thread(new ThreadStart(logic.run));
            logicThread.Start();
        }

        public static void run()
        {
            initialise();
            startNewGame(40, 30, 100);
            while(active)
            {
                mainWindow.DispatchEvents();
                scrollScreen();
                if(inGame)
                {
                    gameScreen.loop();
                }
            }
        }

        private static void scrollScreen()
        {
            if (mouseScrollAmount < SPEED_OF_MOUSE_SCROLL)
                mouseScrollAmount++;
            else
            {
                centerNew(xMove, yMove);
                mouseScrollAmount = 0;
            }
        }

        internal static bool initialised()
        {
            return active;
        }

        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            View currentView = ((RenderWindow)sender).GetView();
            if (e.Code == Keyboard.Key.Left)
            {
                centerNew( - 10, 0);
            }

            if (e.Code == Keyboard.Key.Right)
            {
                centerNew(10, 0);
            }

            if (e.Code == Keyboard.Key.Up)
            {
                centerNew(0, - 10);
                return;
            }

            if (e.Code == Keyboard.Key.Down)
            {
                centerNew(0, 10);
                return;
            }
            if (e.Code == Keyboard.Key.Space && inGame)
            {
                lock(input)
                {
                    if (activeGame)
                        input.enterEvent(new PauseEvent());
                    else
                        input.enterEvent(new UnPauseEvent());
                    activeGame = !activeGame;
                }
                return;
            }
            if (e.Code == Keyboard.Key.Escape)
            {
                OnClosed(sender, e);
                return;
            }
            if (e.Code == Keyboard.Key.Return)
            {
                startNewGame(40, 30, 100);
                return;
            }
        }

        static void Zooming(object sender, MouseWheelEventArgs e)
        {
            float scale = 1 - (e.Delta / 10F);
            if (scale != 1)
            {
                View currentView = ((RenderWindow)sender).GetView();
                float newX = currentView.Size.X * scale, newY = currentView.Size.Y * scale;
                if (((scale > 1) && (newX < topX && newY < topY))
                    || ((scale < 1) && (newX > minX || newY > minY)))
                    currentView.Zoom(scale);

                else if (scale > 1) currentView.Size = new Vector2f(topX, topY); else  currentView.Size = new Vector2f(minX, minY);
                float left = currentView.Center.X - currentView.Size.X / 2;
                float right = currentView.Center.X + currentView.Size.X / 2;
                float up = currentView.Center.Y - currentView.Size.Y / 2;
                float down = currentView.Center.Y + currentView.Size.Y / 2;
                newX = 0;
                newY = 0;

                if (right > topX)
                    newX = right - topX;
                else
                    if (left < 0)
                        newX = left;
                if (up < 0)
                    newY = up;
                else
                    if (down > topY)
                        newY = down - topY;
                currentView.Center = new Vector2f(currentView.Center.X - newX, currentView.Center.Y - newY);
                ((RenderWindow)sender).SetView(currentView);
            }
        }

        static void OnLosingFocus(object sender, EventArgs e)
        {
            input.enterEvent(new PauseEvent());
            activeGame = false;
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
            active = false;
            input.enterEvent(new EndGameEvent());
            logicThread.Join();
        }

        static void OnGainingFocus(object sender, EventArgs e)
        {
            input.enterEvent(new UnPauseEvent());
            activeGame = true;
        }

        static void MouseClick(object sender, MouseButtonEventArgs e)
        {
            //TODO - missing function
        }

        static void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            float x = mainWindow.Size.X;
            float y = mainWindow.Size.Y;
            xMove = 0;
            yMove = 0;
            //System.Console.Out.WriteLine("x,y, and size: " + e.X + " " + e.Y + "  " + x + " " + y );
            if (mouseX < SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING)
            {
                xMove = -(SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING-mouseX) / SCREEN_EDGE_MOUSE_SCROLL_RELATION;
            }
            if (x - mouseX < SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING)
            {
                xMove = (SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING -(x - mouseX)) / SCREEN_EDGE_MOUSE_SCROLL_RELATION;
            }
            if (mouseY < SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING)
            {
                yMove = -(SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING - mouseY) / SCREEN_EDGE_MOUSE_SCROLL_RELATION;
            }
            if (mouseY > y - SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING)
            {
                yMove = (SCREEN_EDGE_WIDTH_FOR_MOUSE_SCROLLING -(y - mouseY)) / SCREEN_EDGE_MOUSE_SCROLL_RELATION;
            }
            //if(input != null)
                //input.enterEvent(new BufferMouseMoveEvent(mainWindow.ConvertCoords(new Vector2i(Convert.ToInt16(mouseX), Convert.ToInt16(mouseY)))));
        }

        static void centerNew(float x, float y)
        {
            View currentView = mainWindow.GetView();
            float halfX = (currentView.Size.X / 2);
            float halfY = (currentView.Size.Y / 2);
            y = Math.Min(currentView.Center.Y+y, backgroundY - halfY + AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
            y = Math.Max(y, halfY - AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
            x = Math.Min(currentView.Center.X+x, backgroundX - halfX + AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
            x = Math.Max(x, halfX - AMOUNT_OF_PIXEL_ALLOWED_OFFSCREEN);
            currentView.Center = new Vector2f(x,y);
            mainWindow.SetView(currentView);
        }

        static void MouseButtonPressed(Object sender, MouseButtonEventArgs e)
        {
        }
    }
}
