using System;
using System.Collections.Generic;

namespace Game.Buffers {

  /// <summary>
  /// the buffer for the input from the game screen to the game logic and display manager.
  /// </summary>
  public class InputBuffer : Buffer {
    private static readonly List<IBufferEvent> list = new List<IBufferEvent>();

    #region fields

    private readonly List<IBufferEvent> m_soundEvents = list;
    private readonly List<IBufferEvent> m_logicEvents = new List<IBufferEvent>();
    private readonly List<IBufferEvent> m_graphicEvents = new List<IBufferEvent>();

    #endregion fields

    #region properties

    public bool SoundInput { get; private set; }

    public bool LogicInput { get; private set; }

    public bool GraphicInput { get; private set; }

    #endregion properties

    #region public method

    //TODO - do we bother with different events?
    public void EnterEvent(IBufferEvent input) {
      lock (this) {
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

    public List<IBufferEvent> GetEvents(InputModuleAccessors accessor) {
      switch (accessor) {
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

    #endregion public method

    #region private methods

    private List<IBufferEvent> GetEventsFromList(List<IBufferEvent> eventsList) {
      List<IBufferEvent> temp = new List<IBufferEvent>(eventsList);
      eventsList.Clear();
      return temp;
    }

    private void SetAllFlags() {
      LogicInput = true;
      SoundInput = true;
      GraphicInput = true;
    }

    #endregion private methods
  }
}
