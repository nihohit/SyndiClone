using SFML.Graphics;
using SFML.Window;

namespace Game.Graphic_Manager
{
    class DisplayManager
    {
        RenderWindow _mainWindow;
        Game.Buffers.DisplayBuffer _buffer;

        public DisplayManager(uint x, uint y, uint bits, Game.Buffers.DisplayBuffer buffer)
        {
            this._buffer = buffer;
            this._mainWindow = new RenderWindow(new VideoMode(x, y, bits), "main display");
        }

        public void display()
        {
            _mainWindow.Display();
        }


    }
}
