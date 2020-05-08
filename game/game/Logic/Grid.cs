using Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Logic {

  internal class Grid {

    #region file read values

    private readonly UInt16 randomPathLength = FileHandler.GetUintProperty("random path length", FileAccessor.LOGIC);
    private readonly UInt16 CIV_FLEE_RANGE = FileHandler.GetUintProperty("civilian flee range", FileAccessor.LOGIC);

    #endregion file read values

    #region delegates

    private delegate ShotEffect CurriedEntityListAdd(HashSet<Entity> list); //These functions curry a list to an effect which will enter entities into the list

    private delegate ShotEffect CurriedDirectionListAdd(List<Direction> list); //These functions curry directions to an effect which will put them in lists

    private delegate bool boolPointOperator(Point point); //These functions as a binary question on a Point

    private delegate boolPointOperator CurriedPointOperator(Entity entity); //These are the curried version, which curry and entity for the question

    #endregion delegates

    #region fields

    private readonly HashSet<Entity> m_entities = new HashSet<Entity>();
    private readonly Dictionary<Entity, Area> m_entitiesToLocations = new Dictionary<Entity, Area>();
    private readonly List<Buffers.IBufferEvent> m_actionsDone = new List<Buffers.IBufferEvent>();
    private readonly HashSet<VisualEntityInformation> m_visibleEntities = new HashSet<VisualEntityInformation>();
    private readonly HashSet<Entity> m_destroyedEntities = new HashSet<Entity>();
    private readonly Entity[,] m_gameGrid;
    private readonly TerrainGrid m_pathFindingGrid;
    private Entity m_selected;
    private Pathfinding.AdvancedAStar m_pathfinder;
    private readonly Dictionary<ConstructorBuilding, Task<List<Direction>>> m_constructorsToPaths = new Dictionary<ConstructorBuilding, Task<List<Direction>>>();
    //TODO - add corporations

    #endregion fields

    #region constructor and initialisation

    public Grid(int x, int y) {
      m_gameGrid = new Entity[x, y];
      m_pathFindingGrid = new TerrainGrid(x, y);
    }

    //this function is called after all the buildings have been added to the grid.
    public void InitialiseTerrainGrid() {
      for (int i = 0; i < m_gameGrid.GetLength(0); i++) {
        for (int j = 0; j < m_gameGrid.GetLength(1); j++) {
          if (m_gameGrid[i, j] == null) m_pathFindingGrid.Grid[i, j] = TerrainType.ROAD;
          else m_pathFindingGrid.Grid[i, j] = TerrainType.BUILDING;
        }
        m_pathfinder = new Pathfinding.AdvancedAStar(m_pathFindingGrid);
      }
    }

    //this function is called after all the buildings have been inserted into the grid.
    public void InitialiseExitPoints() {
      foreach (Entity ent in m_entities) {
        SetExitPoint((Building)ent);
      }
    }

    #endregion constructor and initialisation

    #region public methods

    /// TODO - make this player specific.
    public IEnumerable<VisualEntityInformation> GetVisibleEntities() {
      return m_visibleEntities;
    }

    public List<Entity> GetAllEntities() {
      return new List<Entity>(m_entities);
    }

    public void ClearLists() {
      m_visibleEntities.Clear();
      m_actionsDone.Clear();
      m_destroyedEntities.Clear();
      foreach (var constructor in m_constructorsToPaths.Keys.ToList()) {
        if (m_constructorsToPaths[constructor].Status == TaskStatus.RanToCompletion) {
          SetPathInConstructor(constructor);
        }
      }
    }

    public List<VisualEntityInformation> GetAllVisualEntitiesInformation() {
      return new List<VisualEntityInformation>(m_entities.Select(ent => ent.VisualInfo));
    }

    /*
     * This function returns the list of actions performed in the current round.
     */
    public List<Buffers.IBufferEvent> ReturnCommitedActions() {
      foreach (Entity ent in m_destroyedEntities)
        Destroy(ent);
      return m_actionsDone;
    }

    //TODO - change sight logic, so that all entities of a single player add their sights instead of going over the same area again and again.
    //This function checks if any entity in the radius around the point answers the conditions in checker
    public void WhatSees(Entity ent) {
      //TODO - look into Sight simply having a list and the given blast, instead of creating a new list & blast for every iteration
      //Get all the relevant variables
      Point location = ConvertToCentralPoint(ent);
      Sight sight = ent.SightSystem;
      int radius = sight.Range;
      WasBlocked blocked = sight.Blocked;
      //curries the list of entities to an effect
      CurriedEntityListAdd listAdd = (HashSet<Entity> list) => {
        return (Entity entity) => {
          if (entity != null) { list.Add(entity); }
        };
      };

      ShotEffect effect = listAdd(ent.WhatSees);

      BlastEffect blast = BlastEffect.Instance(radius, effect, blocked, ShotType.SIGHT);

      AreaEffect(ent, blast);

      foreach (Entity temp in ent.WhatSees) {
        //TODO - this is wrong. it should only return what the player units see.
        m_visibleEntities.Add(temp.VisualInfo);
      }
    }

    /*
     * this function resolves a move command from the game logic, based on the reaction set by the entity
     */
    public void ResolveMove(MovingEntity ent) {
      ActionType action = ent.Reaction.Action();
      Point currentLocation = ConvertToCentralPoint(ent);
      switch (action) {
        case ActionType.IGNORE:
          break;

        case ActionType.RUN_AWAY_FROM:
          Point from = ConvertToCentralPoint(((RunAwayReaction)ent.Reaction).Focus);
          Point runTo = GetOppositePoint(from, currentLocation, CIV_FLEE_RANGE);
          ent.Path = GetSimplePath(currentLocation, runTo, ent);
          ((Civilian)ent).RunningAway();
          break;

        case ActionType.PURSUE:
          Point targetLocation = ConvertToCentralPoint(((PursueReaction)ent.Reaction).Focus);
          ent.Path = GetSimplePath(currentLocation, targetLocation, ent);
          break;
          //TODO - missing cases
      }
      Move(ent);
    }

    private Entity GetEntityInPoint(Point point) {
      return m_gameGrid[point.X, point.Y];
    }

    public void ResolveShoot(IShooter shooter, Entity target) {
      Shoot(shooter, target);
    }

    /*
     * This function adds an entity to a certain area
     * TODO - can this become private?
     */
    public void AddEntity(Entity ent, Area area) {
      //TODO - if (gameGrid[loc.getX, loc.Y] != null) throw new LocationFullException(loc.ToString() + " " + gameGrid[loc.getX, loc.getY].ToString());
      //else
      m_entitiesToLocations.Add(ent, area);
      foreach (Point point in area.GetPointArea()) {
        //if (gameGrid[point.X, point.Y] != null) throw new Exception();
        m_gameGrid[point.X, point.Y] = ent;
      }
      m_entities.Add(ent);
      ent.VisualInfo.Position = new Vector(area.Entry.X, area.Entry.Y).ToVector2f();
      AddEvent(new Buffers.InternalCreateUnitBufferEvent(ent, area));
    }

    public void ResolveConstruction(IConstructor constructor, MovingEntity entity) {
      if (constructor is ConstructorBuilding) {
        FinishResolveNewPath((ConstructorBuilding)constructor);
      }
      AddEntity(entity, FindConstructionSpot(constructor, entity));
      //TODO - add the transition of the entity from the center of the building to outside. currently just pops out.
    }

    #endregion public methods

    #region private methods

    #region communication logic

    /*
     * This function finds the central point of an entity - and entity is a grid, and the central point is that which the
     * shots/sights are coming from, and where other units will aim at.
     */
    private Point ConvertToCentralPoint(Entity ent) {
      Area area = m_entitiesToLocations[ent];
      int x = area.Entry.X + area.Size.X / 2, y = area.Entry.Y + area.Size.Y / 2;
      return new Point(x, y);
    }

    /*
     * This function adds events to be reported to future buffers
     */
    private void AddEvent(Buffers.IBufferEvent action) {
      m_actionsDone.Add(action);
    }

    #endregion communication logic

    #region movement logic

    /*
     * This is the basic moving function
     */
    private void Move(MovingEntity ent) {
      for (int tries = 0; ent.Path.Count == 0; tries++) {
        if (tries == 5) //just a precaution
        {
          m_destroyedEntities.Add(ent);
          return;
        }
        Point temp = ConvertToCentralPoint(ent);
        ent.Path = GetSimplePath(temp, GenerateRandomPoint(temp, randomPathLength), ent);
      }
      Direction dir = ent.GetDirection();
      bool result = CanMove(ent, dir);
      if (result) {
        DoMove(ent, dir);
      } else {
        Point temp = ConvertToCentralPoint(ent);
        ent.Path = GetSimplePath(temp, GenerateRandomPoint(temp, randomPathLength), ent);
      }
      ent.MoveResult(result);
    }

    //TODO - problematic, could lead into a building
    private Point GenerateRandomPoint(Point temp, int distance) {
      int maxX = Math.Min(temp.X + distance, m_gameGrid.GetLength(0) - 1);
      int minX = Math.Max(0, temp.X - distance);
      int maxY = Math.Min(temp.Y + distance, m_gameGrid.GetLength(1) - 1);
      int minY = Math.Max(0, temp.Y - distance);
      return new Point(minX, maxX, minY, maxY);
    }

    /*
     * This function is used mainly to generate the simple walking path for civilians.
     * Will probably need to make it better in future, it'll probably serve other functions.
     */
    private List<Direction> GetSimplePath(Point entry, Point target, MovingEntity ent) {
      List<Direction> ans = ProcessWalkingPath(entry, target);
      if (ans.Count != entry.GetDiffVector(target).Length()) {
        //TODO - probably continue from there.
      }
      return ans;
    }

    /*
     * This function will be used for player units, taht need complex routes.
     */
    private Task<List<Direction>> GetComplexPath(Point entry, Point target, Vector size, MovementType movement, Direction direction) {
      return m_pathfinder.FindPathAsync(entry, target, direction, new Pathfinding.AStarConfiguration(size, movement, Pathfinding.Heuristics.DiagonalTo(target), true, true));
    }

    /*
     * a case of Bresenham's line algorithm that returns a list of directions to go in.
     */
    private List<Direction> ProcessWalkingPath(Point exit, Point target) {
      List<Direction> ans = new List<Direction>();
      int x0 = exit.X;
      int y0 = exit.Y;
      int x1 = target.X;
      int y1 = target.Y;
      int dx = System.Math.Abs(x1 - x0);
      int dy = System.Math.Abs(y1 - y0);
      int sx, sy, e2;
      if (x0 < x1) sx = 1;
      else sx = -1;
      if (y0 < y1) sy = 1;
      else sy = -1;
      int err = dx - dy;
      while (!(x0 == x1 & y0 == y1)) {
        Entity ent = m_gameGrid[x0, y0];
        if (ent?.Type == EntityType.BUILDING) break;
        e2 = 2 * err;
        if (e2 > -dy) {
          err -= dy;
          x0 += sx;
          if (sx > 0) {
            if (e2 < dx) {
              err += dx;
              y0 += sy;
              if (sy > 0) {
                ans.Add(Direction.DOWNRIGHT);
              } else {
                ans.Add(Direction.UPRIGHT);
              }
            } else {
              ans.Add(Direction.RIGHT);
            }
          } else if (e2 < dx) {
            err += dx;
            y0 += sy;
            if (sy > 0) {
              ans.Add(Direction.DOWNLEFT);
            } else {
              ans.Add(Direction.UPLEFT);
            }
          } else {
            ans.Add(Direction.LEFT);
          }
        } else if (e2 < dx) {
          err += dx;
          y0 += sy;
          if (sy > 0) {
            ans.Add(Direction.DOWN);
          } else {
            ans.Add(Direction.UP);
          }
        }

        if ((y0 < 0) || (x0 < 0) || (x0 >= m_gameGrid.GetLength(0)) || (y0 >= m_gameGrid.GetLength(1))) {
          throw new IndexOutOfRangeException();
        }
      }
      return ans;
    }

    /*
     * Checks if a ceratin entity can mone, and whether it needs to flip its axis in order to move in the given direction
     */
    private bool CanMove(MovingEntity ent, Direction direction) {
      Area location = m_entitiesToLocations[ent];
      if (ent.NeedFlip()) {
        location = location.Flip();
      }

      Area areaToCheck = new Area(location, Vector.DirectionToVector(direction));

      return CanMove(ent, areaToCheck);
    }

    /*
     * Checks if an entity can enter a given area - whether each point is free
     */
    private bool CanMove(Entity ent, Area area) {
      //This delegate is a function that checks if the entity in the point is either null (point empty) or the same entity
      CurriedPointOperator checkEntityInArea = (Entity entity) => {
        return (Point point) => {
          Entity temp = GetEntityInPoint(point);
          return entity == temp ||
            temp == null;
        };
      };
      return IterateOverArea(area, checkEntityInArea(ent));
    }

    /**
     * This function serves for running away from a certain point - it finds where does the civilian run to.
     */
    private Point GetOppositePoint(Point from, Point center, int distance) {
      Vector dist = center.GetDiffVector(from);
      dist.CompleteToDistance(distance);
      return new Point(from, dist);
      //TODO - check
    }

    private void DoMove(MovingEntity ent, Direction dir) {
      RemoveFromLocation(ent);
      MoveToNewLocation(ent, dir);
    }

    /*
     *
     */
    private void MoveToNewLocation(MovingEntity ent, Direction dir) {
      Area location = m_entitiesToLocations[ent];
      if (ent.NeedFlip()) {
        location = location.Flip();
      }

      Area toSwitch = new Area(location, Vector.DirectionToVector(dir));

      boolPointOperator putEntityInArea(Entity entity) => (Point point) => {
        m_gameGrid[point.X, point.Y] = entity;
        return true;
      };

      IterateOverArea(toSwitch, putEntityInArea(ent));
      Debug.Assert(m_entities.Contains(ent), String.Format("entity {0} isn't in entities list", ent.ToString()));
      ent.VisualInfo.Position = new SFML.System.Vector2f(Convert.ToSingle(location.Entry.X), Convert.ToSingle(location.Entry.Y));
      m_entitiesToLocations[ent] = toSwitch;
    }

    private bool RemoveEntityFromPoint(Point point) {
      m_gameGrid[point.X, point.Y] = null;
      return true;
    }

    private void RemoveFromLocation(Entity ent) {
      Area area = m_entitiesToLocations[ent];
      IterateOverArea(area, RemoveEntityFromPoint);
    }

    /*
     * This function iterates an operator on every point in an area.
     */
    private bool IterateOverArea(Area area, boolPointOperator op) {
      bool ans = true;
      Point entry = area.Entry;
      for (int i = 0; i < area.Size.X; i++) {
        for (int j = 0; j < area.Size.Y; j++) {
          ans &= op(new Point(entry, new Vector(i, j)));
        }
      }
      return ans;
    }

    #endregion movement logic

    #region shooting logic

    private void AreaEffect(Entity ent, BlastEffect blast) {
      AreaEffect(ConvertToCentralPoint(ent), blast);
    }

    /*
     * this function sends a shot effect in every direction - lines to every point in the radius circumference
     */
    private void AreaEffect(Point location, BlastEffect blast) {
      int radius = blast.Radius;
      int x = location.X;
      int y = location.Y;
      int newX, newY;
      //checks in each of the four cardinal directions;
      //TODO - make sure that entities aren't affected more then once? or just reduce the effects to account for that?
      for (int i = 1; i < radius; i++) {
        newX = Math.Min(x + i, m_gameGrid.GetLength(0) - 1);
        newY = Math.Min(y + radius - i, m_gameGrid.GetLength(1) - 1);
        ProcessPath(location, new Point(newX, newY), blast);

        newY = Math.Max(y - radius + i, 0);
        ProcessPath(location, new Point(newX, newY), blast);

        newX = Math.Max(x - i, 0);
        ProcessPath(location, new Point(newX, newY), blast);

        newY = Math.Min(y + radius - i, m_gameGrid.GetLength(1) - 1);
        ProcessPath(location, new Point(newX, newY), blast);
      }
    }

    //This function simulates a single shot
    private void Shoot(IShooter shooter, Entity target) {
      if (target.Destroyed()) return;
      //get all relevant variables
      Weapons weapon = shooter.Weapon();
      Shot shot = weapon.Shot;
      Point exit = ConvertToCentralPoint((Entity)shooter);
      Point currentTargetLocation = ConvertToCentralPoint(target);
      Vector direction = new Vector(currentTargetLocation, exit);
      direction = direction.normalProbability(direction.Length() / weapon.Accuracy);
      direction = direction.CompleteToDistance(weapon.Range);
      //TODO - If there's a target that I see only parts of it, how do I aim at the visible parts?

      //get the path the bullet is going through, and affect targets
      Point endPoint = ProcessPath(exit, new Point(exit, direction), shot);

      if (shot.Blast != null) {
        AreaEffect(endPoint, shot.Blast);
      }
    }

    /*
     * A simple version of Berensham's line algorithm, that calculates the way of a bullet, and affects every entity in the way
     */
    private Point ProcessPath(Point exit, Point target, Shot shot) {
      ShotEffect effect = shot.Effect;
      WasBlocked blocked = shot.Blocked;
      int x0 = exit.X;
      int y0 = exit.Y;
      int x1 = target.X;
      int y1 = target.Y;
      int dx = System.Math.Abs(x1 - x0);
      int dy = System.Math.Abs(y1 - y0);
      int sx, sy, e2;
      if (x0 < x1) sx = 1;
      else sx = -1;
      if (y0 < y1) sy = 1;
      else sy = -1;
      int err = dx - dy;
      Entity ent = null;
      while ((!(x0 == x1 & y0 == y1)) && (!shot.Blocked(ent))) {
        e2 = 2 * err;
        if (e2 > -dy) {
          err -= dy;
          x0 += sx;
        }
        if (e2 < dx) {
          err += dx;
          y0 += sy;
        }
        if (x0 >= m_gameGrid.GetLength(0) || x0 < 0 || y0 < 0 || y0 >= m_gameGrid.GetLength(1))
          break;
        ent = m_gameGrid[x0, y0];
        if (ent != null) {
          effect(ent);
          if (ent.Destroyed()) {
            m_destroyedEntities.Add(ent);
          }
        }
      }
      Point res = new Point(x0, y0);

      if (shot.Type != ShotType.SIGHT) {
        AddEvent(new Buffers.ShotBufferEvent(shot.Type, exit, res));
      }
      return res;
    }

    private void Destroy(Entity ent) {
      if (ent.Type == EntityType.BUILDING) {
        Area area = m_entitiesToLocations[ent];
        foreach (Point point in area.GetPointArea()) {
          m_pathFindingGrid.Grid[point.X, point.Y] = TerrainType.ROAD;
        }
      }

      AddEvent(new Buffers.InternalDestroyBufferEvent(m_entitiesToLocations[ent], ent));
      RemoveFromLocation(ent);
      m_entitiesToLocations.Remove(ent);
      m_entities.Remove(ent);
      ent.Destroy();
    }

    #endregion shooting logic

    #region construction logic

    private Area FindConstructionSpot(IConstructor constructor, Entity ent) {
      return new Area(GetExitPoint((Building)constructor), ent.Size);
    }

    private Point GetExitPoint(Building constructor) {
      return new Point(ConvertToCentralPoint(constructor), constructor.Exit);
    }

    public void SelectUnit(Point point, Affiliation player) {
      Entity temp = GetEntityInPoint(point);
      var selectable = temp as ISelectable;
      if (selectable == null) {
        Console.WriteLine("nothing selected.");
        return;
      }
      if (m_selected == null) { //TODO - loyalty should be player issuing the order
        m_selected = temp;
        var centralPoint = ConvertToCentralPoint(temp);
        m_actionsDone.Add(new Buffers.UnitSelectBufferEvent(
          m_selected.VisualInfo,
          selectable.Select(player),
          new Vector(centralPoint.X, centralPoint.Y)));
        FinishResolveNewPath((ConstructorBuilding)temp);
      } else {
        var selection = selectable.Select(player);
        if (selection.Controlled) {
          point = GetExitPoint(temp as Building);
          var pol = m_selected as PoliceStation;
          m_constructorsToPaths.Add(pol, GetComplexPath(GetExitPoint(pol), point, new Vector(1, 1), MovementType.GROUND, pol.Exit.VectorToDirection()));
        }
        DeselectUnit();
      }
    }

    public void DeselectUnit() {
      m_actionsDone.Add(new Buffers.CancelActionBufferEvent());
      m_selected = null;
    }

    /*
     * This method sets randomly all the exit points of all the buildigns on the map.
     */
    private void SetExitPoint(Building ent) {
      Point center = ConvertToCentralPoint(ent);
      List<Vector> options = new List<Vector>();
      int x = center.X;
      int y = center.Y;
      int i;
      i = (ent.Size.X) / 2;
      if (x + i < m_gameGrid.GetLength(0) && (m_gameGrid[x + i, y] == null))
        options.Add(new Vector(i, 0));
      if (x - i >= 0 && (m_gameGrid[x - i, y] == null))
        options.Add(new Vector(-i, 0));
      i = (ent.Size.Y) / 2;
      if (y + i < m_gameGrid.GetLength(1) && (m_gameGrid[x, y + i] == null))
        options.Add(new Vector(0, i));
      if (y - i >= 0 && (m_gameGrid[x, y - i] == null))
        options.Add(new Vector(0, -i));
      //i = randomGenerator.Next(0, options.Count);
      if (options.Count > 0) {
        ent.Exit = options[Randomizer.Next(0, options.Count)];
      } else {
        i = (ent.Size.X / 2) + 1;
        if (x + i < m_gameGrid.GetLength(0) && (m_gameGrid[x + i, y] == null))
          options.Add(new Vector(i, 0));
        if (x - i >= 0 && (m_gameGrid[x - i, y] == null))
          options.Add(new Vector(-i, 0));
        i = (ent.Size.Y / 2) + 1;
        if (y + i < m_gameGrid.GetLength(1) && (m_gameGrid[x, y + i] == null))
          options.Add(new Vector(0, i));
        if (y - i >= 0 && (m_gameGrid[x, y - i] == null))
          options.Add(new Vector(0, -i));
        if (options.Count > 0)
          ent.Exit = options[Randomizer.Next(0, options.Count)];
        else throw new Exception("can't find exit point");
      }
    }

    private void FinishResolveNewPath(ConstructorBuilding build) {
      if (m_constructorsToPaths.ContainsKey(build)) {
        var task = m_constructorsToPaths[build];
        task.Wait();
        SetPathInConstructor(build);
      }
    }

    private void SetPathInConstructor(ConstructorBuilding constructor) {
      var task = m_constructorsToPaths[constructor];
      m_constructorsToPaths.Remove(constructor);
      constructor.Path = task.Result;
      m_actionsDone.Add(new Buffers.SetPathActionBufferEvent(constructor.VisualInfo, constructor.Path, FindConstructionSpot(constructor, constructor.GetConstruct()).Entry.ToVector2f()));
    }

    #endregion construction logic

    #endregion private methods
  }
}
