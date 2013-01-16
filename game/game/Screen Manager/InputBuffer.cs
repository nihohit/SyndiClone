using System.Collections.Generic;
using System;

namespace Game.Buffers
{
    /// <summary>
    /// the buffer for the input from the game screen to the game logic and display manager.
    /// </summary>
    public class InputBuffer : Buffer
    {
        #region fields
        private List<IBufferEvent> m_soundEvents = new List<IBufferEvent>();
        private List<IBufferEvent> m_logicEvents = new List<IBufferEvent>();
        private List<IBufferEvent> m_graphicEvents = new List<IBufferEvent>();
        #endregion

        #region properties
        public bool SoundInput {get; private set; }

        public bool LogicInput {get; private set; }

        public bool GraphicInput {get; private set; }
        #endregion

        #region public method

        //TODO - do we bother with different events?
        public void EnterEvent(IBufferEvent input)
        {
            lock (this)
            {
                m_graphicEvents.Add(input);
                m_logicEvents.Add(input);
                m_soundEvents.Add(input);
                SetAllFlags();
                /*
                switch (input.type())
                {
                    case BufferType.PAUSE:
                        graphicEvents.Add(input);
                        logicEvents.Add(input);
                        soundEvents.Add(input);
                        setAllFlags();
                        break;
                    case BufferType.UNPAUSE:
                        graphicEvents.Add(input);
                        logicEvents.Add(input);
                        soundEvents.Add(input);
                        setAllFlags();
                        break;
                    case BufferType.END:
                        graphicEvents.Add(input);
                        logicEvents.Add(input);
                        soundEvents.Add(input);
                        setAllFlags();
                        break;
                }*/
                
            }
        }

        public List<IBufferEvent> GetEvents(InputModuleAccessors accessor)
        {
            switch (accessor)
            {
                case InputModuleAccessors.Graphics:
                    GraphicInput = false;
                    return GetEventsFromList(m_graphicEvents);

                case InputModuleAccessors.Logic:
                    LogicInput = false;
                    return GetEventsFromList(m_logicEvents);

                case InputModuleAccessors.Sounds:
                    SoundInput = false;
                    return GetEventsFromList(m_soundEvents);

                default:
                    throw new Exception("invalid InputModuleAccessor " + accessor);
            }
        }

        #endregion

        #region private methods

        private List<IBufferEvent> GetEventsFromList(List<IBufferEvent> eventsList)
        {
            List<IBufferEvent> temp = new List<IBufferEvent>(eventsList);
            eventsList.Clear();
            return temp;
        }

        private void SetAllFlags()
        {
            LogicInput = true;
            SoundInput = true;
            GraphicInput = true;
        }

        #endregion
    }
}
