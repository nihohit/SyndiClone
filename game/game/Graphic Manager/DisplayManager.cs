using SFML.Graphics;
using SFML.Window;

namespace Game.Graphic_Manager
{
    class DisplayManager
    {
        RenderWindow mainWindow;

        public DisplayManager(uint x, uint y, uint bits)
        {
            this.mainWindow = new RenderWindow(new VideoMode(x, y, bits), "main display");
        }

        public void display()
        {
            mainWindow.Display();
        }


    }
}
