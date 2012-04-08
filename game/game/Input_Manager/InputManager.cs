using SFML.Window;
using System.Collections.Generic;

namespace Game.Input_Manager
{
    class InputManager
    {
        Window _window;
        Buffers.InputBuffer _buffer;

        InputManager(Window window, Buffers.InputBuffer buffer)
        {
            this._buffer = buffer;
            this._window = window;
        }

        internal void run()
        {
            while (true)
            {
                this._window.DispatchEvents();
            }
        }


    }
}
