using System.Collections.Generic;

namespace Game.Buffers
{
    class InputBuffer : Buffers.Buffer
    {
        private bool _logicFlag = false;
        private bool _graphicFlag = false;
        private bool _soundFlag = false;
        private List<BufferEvent> _soundEvents = new List<BufferEvent>();
        private List<BufferEvent> _logicEvents = new List<BufferEvent>();
        private List<BufferEvent> _graphicEvents = new List<BufferEvent>();

        public bool SoundInput
        {
            get { return _soundFlag; }
            set { _logicFlag = value; }
        }

        public bool LogicInput
        {
            get { return _logicFlag; }
            set { _logicFlag = value; }
        }

        public bool GraphicInput
        {
            get { return _graphicFlag; }
            set { _graphicFlag = value; }
        }

        void setAllFlags()
        {
            this.LogicInput = true;
            this.SoundInput = true;
            this.GraphicInput = true;
        }

        public void enterEvent(BufferEvent input)
        {
            lock (this)
            {
                this._graphicEvents.Add(input);
                this._logicEvents.Add(input);
                this._soundEvents.Add(input);
                this.setAllFlags();
                /*
                switch (input.type())
                {
                    case BufferType.PAUSE:
                        this._graphicEvents.Add(input);
                        this._logicEvents.Add(input);
                        this._soundEvents.Add(input);
                        this.setAllFlags();
                        break;
                    case BufferType.UNPAUSE:
                        this._graphicEvents.Add(input);
                        this._logicEvents.Add(input);
                        this._soundEvents.Add(input);
                        this.setAllFlags();
                        break;
                    case BufferType.END:
                        this._graphicEvents.Add(input);
                        this._logicEvents.Add(input);
                        this._soundEvents.Add(input);
                        this.setAllFlags();
                        break;
                }*/
                
            }
        }

        public List<BufferEvent> logicEvents()
        {
            List<BufferEvent> temp = new List<BufferEvent>(this._logicEvents);
            this._logicEvents.Clear();
            this.LogicInput = false;
            return temp;
            
        }


        internal List<BufferEvent> graphicEvents()
        {
            List<BufferEvent> temp = new List<BufferEvent>(this._graphicEvents);
            this._graphicEvents.Clear();
            this.GraphicInput = false;
            return temp;
        }
    }
}
