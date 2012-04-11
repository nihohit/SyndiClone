using System.Collections.Generic;
using Game.Buffers;

namespace Game.Input_Manager
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
        }

        public bool LogicInput
        {
            get { return _logicFlag; }
        }

        public bool GraphicInput
        {
            get { return _graphicFlag; }
        }

    }
}
