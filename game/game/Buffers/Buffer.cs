using System.Threading;
using System;

namespace Game.Buffers
{
    abstract class Buffer
    {
        /******************
        Class Fields
        ****************/
        
        private readonly Object thisLock;


        /******************
        Constructors
        ****************/
        public Buffer()
        {
            this.thisLock = new Object();
        }

        /******************
        Methods
        ****************/
        public void getLock()
        {
            Monitor.Enter(this.thisLock);
        }

        public void releaseLock()
        {
            Monitor.Exit(this.thisLock);
        }
    }
}
