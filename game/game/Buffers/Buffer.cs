using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Buffers
{
    interface Buffer
    {
        void receiveInput(Input input);
        bool readyForInput();
        bool readyToOutput();
        LinkedList<Output> returnOutput();

    }
}
