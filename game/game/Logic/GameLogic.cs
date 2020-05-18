using Game.Buffers;
using Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logic {

  public class GameLogic {

    #region fields

    static private readonly uint FRAMES_PER_SECOND = FileHandler.GetUintProperty("logic frames per second", FileAccessor.LOGIC); //determines the amount of repeats the system can have in a second
    private readonly uint MIN_MILLISECONDS_PER_FRAME = 1000 / FRAMES_PER_SECOND;

    private List<Entity> m_activeEntities = new List<Entity>(); //TODO - readonly HashSet?
    private readonly HashSet<MovingEntity> m_movingEntities = new HashSet<MovingEntity>();
    private readonly HashSet<Entity> m_playerUnits = new HashSet<Entity>();
    private readonly HashSet<IShooter> m_shootingEntities = new HashSet<IShooter>();
    private readonly HashSet<IConstructor> m_constructingEntities = new HashSet<IConstructor>();
    private readonly HashSet<Entity> m_alwaysActiveEntities; //TODO - we can skip this, if we decide that civilians work even when not in view. all depends on calculations' weight
    private readonly Dictionary<int, Affiliation> m_playerIdToAffiliation = new Dictionary<int, Affiliation>();

    private readonly int m_maximumAmountOfCivilians;
    private int m_currentCivilianAmount;
    private readonly Grid m_grid;
    private readonly DisplayBuffer m_displayBuffer;
    private readonly InputBuffer m_inputBuffer;
    private readonly SoundBuffer m_soundBuffer;
    private bool m_unpaused;
    private bool m_gameRunning;
    private System.Diagnostics.Stopwatch m_frameTimer = new System.Diagnostics.Stopwatch();

    //TODO - debug, remove.
    private int runs = 0;

    private System.Diagnostics.Stopwatch synch = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch move = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch shoot = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch construct = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch other = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch orders = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch sight = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch totalWatch = new System.Diagnostics.Stopwatch();

    #endregion fields

    #region constructor

    public GameLogic(DisplayBuffer disp, InputBuffer input, SoundBuffer sound, Game.City_Generator.GameBoard city, int civAmount) {
      m_displayBuffer = disp;
      m_inputBuffer = input;
      m_soundBuffer = sound;
      m_maximumAmountOfCivilians = civAmount;
      civAmount = 0;
      m_grid = GameBoardToGameGridConverter.ConvertBoard(city);
      m_alwaysActiveEntities = new HashSet<Entity>(m_grid.GetAllEntities());
      m_unpaused = true;
      m_gameRunning = true;
      totalWatch.Start();
      m_frameTimer.Start();
      m_playerIdToAffiliation.Add(1, Affiliation.CORP1);
    }

    #endregion constructor

    #region public methods

    public void Run() {
      while (m_gameRunning) {
        //Console.Out.WriteLine("logic loop");
        Loop();
      }
    }

    /*
     * This is the main loop of the logic
     */
    public void Loop() {
      synch.Start();
      HandleInput();
      synch.Stop();
      if (!m_gameRunning) {
        totalWatch.Stop();
        Console.Out.WriteLine("synch was " + synch.Elapsed);
        Console.Out.WriteLine("move was " + other.Elapsed);
        Console.Out.WriteLine("shoot was " + shoot.Elapsed);
        Console.Out.WriteLine("construct was " + construct.Elapsed);
        Console.Out.WriteLine("orders was " + orders.Elapsed);
        Console.Out.WriteLine("sight was " + sight.Elapsed);
        Console.Out.WriteLine("amount of graphic loops: " + runs + " average milliseconds per frame: " + totalWatch.ElapsedMilliseconds / runs);
      }
      if (m_unpaused) {
        other.Start();

        PopulateActionLists();
        ResolveOrders();

        other.Stop();

        move.Start();

        HandleMovement();

        move.Stop();

        shoot.Start();

        HandleShooting();

        shoot.Stop();

        construct.Start();

        HandleUnitCreation();

        construct.Stop();

        synch.Start();

        UpdateOutput();

        synch.Stop();

        ClearData();
      }
      FrameLimit();
      runs++;
    }

    #endregion public methods

    #region private methods

    /*
     * This method limits the amount of logic frames per second.
     */
    private void FrameLimit() {
      while (m_frameTimer.ElapsedMilliseconds < MIN_MILLISECONDS_PER_FRAME) { }
      m_frameTimer.Restart();
    }

    private void HandleUnitCreation() {
      int civAmountToCreate = m_maximumAmountOfCivilians - m_currentCivilianAmount;
      foreach (IConstructor constructor in m_constructingEntities) {
        switch (((Entity)constructor).Loyalty) {
          case (Affiliation.CIVILIAN):
            if (civAmountToCreate > 0) {
              m_grid.ResolveConstruction(constructor, constructor.GetConstruct());
              civAmountToCreate--;
              m_currentCivilianAmount++;
            }
            break;

          default:
            m_grid.ResolveConstruction(constructor, constructor.GetConstruct());
            break;
        }
      }
    }

    private void ClearData() {
      m_shootingEntities.Clear();
      m_movingEntities.Clear();
      m_constructingEntities.Clear();
      m_activeEntities.Clear();
      m_grid.ClearLists();
    }

    private void UpdateOutput() {
      List<IBufferEvent> actions = m_grid.ReturnCommitedActions();
      foreach (IBufferEvent action in actions.ToList()) {
        switch (action.Type()) {
          case (BufferType.LOGIC_INTERNAL_DESTROY):
            InternalDestroyBufferEvent temp = (InternalDestroyBufferEvent)action;
            m_alwaysActiveEntities.Remove(temp.DestroyedEntity);
            m_playerUnits.Remove(temp.DestroyedEntity);
            if ((temp.DestroyedEntity.Type == EntityType.PERSON) && temp.DestroyedEntity.Loyalty == Affiliation.CIVILIAN) {
              m_currentCivilianAmount--;
            }
            actions.Remove(action);
            actions.Add(new ExternalDestroyBufferEvent(temp));
            break;

          case (BufferType.LOGIC_INTERNAL_CREATE):
            //TODO - change this function
            m_alwaysActiveEntities.Add(((InternalCreateUnitBufferEvent)action).CreatedEntity);
            actions.Remove(action);
            actions.Add(new ExternalCreateUnitBufferEvent((InternalCreateUnitBufferEvent)action));
            break;
            //TODO - missing cases?
        }
      }
      //TODO - try smarter threading, with waiting only a limited time on entering.
      lock (m_displayBuffer) {
        //List<ExternalEntity> newPath = grid.getVisibleEntities();
        List<VisualEntityInformation> newList = new List<VisualEntityInformation>(m_grid.GetAllVisualEntitiesInformation());
        m_displayBuffer.ReceiveVisibleEntities(newList);
        if (actions.Count > 0) {
          List<IBufferEvent> actionList = new List<IBufferEvent>(actions);
          m_displayBuffer.ReceiveActions(actionList);
        }
        m_displayBuffer.Updated = true;
      }
    }

    private void HandleInput() {
      lock (m_inputBuffer) {
        if (m_inputBuffer.LogicInput) {
          List<IBufferEvent> events = m_inputBuffer.GetEvents(InputModuleAccessors.Logic);
          foreach (IBufferEvent action in events) {
            switch (action.Type()) {
              case BufferType.PAUSE:
                m_unpaused = false;
                break;

              case BufferType.UNPAUSE:
                m_unpaused = true;
                break;

              case BufferType.ENDGAME:
                m_unpaused = false;
                m_gameRunning = false;
                break;

              case BufferType.SELECT:
                m_grid.SelectUnit(((MouseSelectBufferEvent)action).Coords.ToPoint(), m_playerIdToAffiliation[((MouseSelectBufferEvent)action).PlayerId]);
                break;

              case BufferType.DESELECT:
                m_grid.DeselectUnit();
                break;
            }
          }
        }
      }
    }

    /*
     * This function iterates over every active entity, checks if they need to act, and if they do, finds their new reaction.
     */
    private void ResolveOrders() {
      foreach (Entity ent in m_activeEntities) {
        orders.Start();
        if (ent.DoesReact()) {
          if (ent.WhatSees.Count == 0) {
            orders.Stop();
            sight.Start();
            m_grid.WhatSees(ent);
            sight.Stop();
          }
          orders.Start();
          ent.ResolveOrders();
          ent.WhatSees.Clear();
        }
        Reaction react = ent.Reaction;
        ActionType action = react.Action();

        if (action == ActionType.FIRE_AT || action == ActionType.MOVE_WHILE_SHOOT) {
          IShooter temp = (IShooter)ent;
          if (temp.ReadyToShoot()) {
            m_shootingEntities.Add(temp);
          }
        }

        if (action == ActionType.MOVE_TOWARDS || action == ActionType.MOVE_WHILE_SHOOT || action == ActionType.RUN_AWAY_FROM ||
          (action == ActionType.IGNORE && ent.Type != EntityType.BUILDING)) {
          MovingEntity temp = (MovingEntity)ent;
          if (temp.ReadyToMove(temp.Speed)) {
            m_movingEntities.Add(temp);
          }
        }

        if (action == ActionType.CONSTRUCT_ENTITY) {
          IConstructor temp = (IConstructor)ent;
          bool check = temp.ReadyToConstruct();
          if (check) //TODO - for structures, to make sure they update their building order. other solution?
          {
            m_constructingEntities.Add(temp);
          }
        }
        orders.Stop();
      }
    }

    private void HandleMovement() {
      foreach (MovingEntity ent in m_movingEntities) {
        m_grid.ResolveMove(ent);
      }
    }

    private void HandleShooting() {
      foreach (IShooter ent in m_shootingEntities) {
        m_grid.ResolveShoot(ent, ent.Target());
      }
    }

    /*
     * This function populates the active entities for this round, by the logic of - all player units, and every entity they see
     */
    private void PopulateActionLists() {
      /*
      activeEntities.listAdd(alwaysActive);
      foreach (Entity t in playerUnits)
      {
          activeEntities.uniqueAdd(t);
          grid.whatSees(t);
          foreach (Entity temp in t.WhatSees)
          {
              activeEntities.uniqueAdd(temp);
          }
      }
       */
      m_activeEntities = m_grid.GetAllEntities();
    }

    //TODO - add blocks, add the whole player logic, add research

    #endregion private methods
  }
}
