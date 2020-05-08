using Game.Logic.Entities;
using System.Collections.Generic;

namespace Game.Logic {

  #region delegates

  public delegate bool EntityChecker(Entity ent); //These functions check basic questions about entities and return a bool

  public delegate void ShotEffect(Entity ent); //These functions simulate effects on entities. mostly will be damage

  public delegate bool WasBlocked(Entity ent); //These functions check whether an entitiy blocks a certain effect

  public delegate Entity targetChooser(IEnumerable<Entity> targets); //These functions choose which entity, out of the list of possible entities, to target

  public delegate Reaction reactionFunction(IEnumerable<Entity> ent); //These functions set the reaction of entities

  #endregion delegates

  #region enumerators

  public enum ActionType { FIRE_AT, IGNORE, RUN_AWAY_FROM, MOVE_TOWARDS, MOVE_WHILE_SHOOT, CONSTRUCT_ENTITY, PURSUE } //This enum checks the possible actions entities can take

  public enum EntityType { PERSON, VEHICLE, BUILDING } //the different types of entities

  public enum Visibility { CLOAKED, MASKED, REVEALED, SOLID } //the visibility of an entity

  public enum Affiliation { INDEPENDENT, CORP1, CORP2, CORP3, CORP4, CIVILIAN } //to which player each entity belongs

  public enum SightType { DEFAULT_SIGHT, BLIND } //different sights

  public enum WeaponType { PISTOL, ASSAULT, BAZOOKA, SNIPER, RAILGUN } //different weapons

  public enum MovementType { GROUND, HOVER, FLYER, CRUSHER }

  public enum TerrainType { ROAD, WATER, BUILDING }

  public enum BlastType { } //different blast effect

  public enum ShotType { SIGHT, PISTOL_BULLET }

  public enum Direction { LEFT, RIGHT, UP, DOWN, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT }

  public enum Corporations { BIOTECH, STEALTH, ARMS, VEHICLES, VISION }

  public enum Upgrades { BULLETPROOF_VEST, VISIBILITY_SOLID, BUILDING_BLIND, FLYER, HOVER, CRUSHER }

  #endregion enumerators

  #region TerrainGrid

  //This class is a holder for an array - used so the array won't be transferred by value over functions.
  public class TerrainGrid {
    private readonly TerrainType[,] grid;

    public TerrainGrid(int x, int y) {
      grid = new TerrainType[x, y];
    }

    public Logic.TerrainType[,] Grid {
      get { return grid; }
    }
  }

  #endregion TerrainGrid

  #region reactions

  #region Reaction

  //This interface describes the reaction of an entity to the enemies it sees.
  public interface Reaction {

    ActionType Action();
  }

  #endregion Reaction

  #region ShootReaction

  public struct ShootReaction : Reaction {
    private readonly Entity m_focus;

    public Entity Focus {
      get { return m_focus; }
    }

    ActionType Reaction.Action() {
      return ActionType.FIRE_AT;
    }

    public ShootReaction(Entity focus) {
      m_focus = focus;
    }
  }

  #endregion ShootReaction

  #region PursueReaction

  public struct PursueReaction : Reaction {
    private readonly Entity m_focus;

    public Entity Focus {
      get { return m_focus; }
    }

    public ActionType Action() {
      return ActionType.PURSUE;
    }

    public PursueReaction(Entity focus) {
      m_focus = focus;
    }
  }

  #endregion PursueReaction

  #region ShootAndMoveReaction

  public struct ShootAndMoveReaction : Reaction {
    private readonly Entity m_focus;

    public Entity Focus {
      get { return m_focus; }
    }

    ActionType Reaction.Action() {
      return ActionType.MOVE_WHILE_SHOOT;
    }

    private ShootAndMoveReaction(Entity focus) {
      m_focus = focus;
    }
  }

  #endregion ShootAndMoveReaction

  #region ConstructReaction

  public struct ConstructReaction : Reaction {
    private readonly MovingEntity m_focus;

    public MovingEntity Focus {
      get { return m_focus; }
    }

    ActionType Reaction.Action() {
      return ActionType.CONSTRUCT_ENTITY;
    }

    public ConstructReaction(MovingEntity focus) {
      m_focus = focus;
    }
  }

  #endregion ConstructReaction

  #region RunAwayReaction

  public struct RunAwayReaction : Reaction {
    private readonly Entity m_focus;

    public Entity Focus {
      get { return m_focus; }
    }

    ActionType Reaction.Action() {
      return ActionType.RUN_AWAY_FROM;
    }

    public RunAwayReaction(Entity focus) {
      m_focus = focus;
    }
  }

  #endregion RunAwayReaction

  #region IgnoreReaction

  public struct IgnoreReaction : Reaction {

    ActionType Reaction.Action() {
      return ActionType.IGNORE;
    }
  }

  #endregion IgnoreReaction

  #endregion reactions

  #region UniqueList

  //This class represents a list that verifies that entities entered into it only once.
  public class UniqueList<T> : List<T> {

    public UniqueList() : base() {
    }

    public UniqueList(List<T> old) : base(old) {
    }

    public void uniqueAdd(T obj) {
      if (!base.Contains(obj)) base.Add(obj);
    }

    public void listAdd(UniqueList<T> list) {
      foreach (T t in list) {
        uniqueAdd(t);
      }
    }
  }

  #endregion UniqueList

  #region LocationFullException

  public class LocationFullException : System.ApplicationException {

    public LocationFullException() {
    }

    public LocationFullException(string message) {
    }

    // Constructor needed for serialization
    // when exception propagates from a remoting server to the client.
    protected LocationFullException(System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) { }
  }

  #endregion LocationFullException

  #region EntityInformation

  public class SelectedEntityInformation {
    public bool Controlled { get; private set; }

    //TODO - determine what needs to be here.
  }

  #endregion EntityInformation
}
