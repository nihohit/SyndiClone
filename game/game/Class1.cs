using SFML.Graphics;
using SFML.Window;

namespace Game
{
    class main
    {
        public static void Main()
        {
            RenderWindow mainWindow = new RenderWindow(new VideoMode(600, 400), "main display");
            Gwen.Renderer.SFML UIrenderer = new Gwen.Renderer.SFML(mainWindow);
            Gwen.Skin.TexturedBase skin = new Gwen.Skin.TexturedBase(UIrenderer, "DefaultSkin.png");
            Gwen.Control.Canvas canvas = new Gwen.Control.Canvas(skin);
            canvas.SetSize((int)mainWindow.Size.X, (int)mainWindow.Size.Y);
            canvas.MouseInputEnabled = true;
            Gwen.Input.SFML input = new Gwen.Input.SFML();
            input.Initialize(canvas, mainWindow);
            Gwen.Control.Button newGameButton = new Gwen.Control.Button(canvas);

            newGameButton.Text = "New Game";
            newGameButton.SetBounds((int)mainWindow.Size.X / 2, (int)mainWindow.Size.Y / 2, 200, 200);
            newGameButton.AutoSizeToContents = true;
            newGameButton.Pressed += newGameScreen;
            while (true)
            {
                mainWindow.SetActive();
                mainWindow.DispatchEvents();
                mainWindow.Clear();
                canvas.RenderCanvas();
                mainWindow.Display();
            }
        }

        static private void newGameScreen(Gwen.Control.Base control)
        {
            System.Console.Out.WriteLine("button clicked");
        }
    }
}
