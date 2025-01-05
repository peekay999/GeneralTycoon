using System.Collections.Generic;
using Godot;

/// <summary>
/// Controller class for all units on the map. Handles unit creation and positioning.
/// </summary>
public partial class UnitController : Node2D
{
	private Node2D world;
	private TileMapController _tileMapController;
	private Pathfinder _pathfinder;
	private TileMapLayer _unitLayer;
	private HashSet<Formation> formations;

	private Formation debugFormation;
	private Direction direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetParent<Node2D>();
		_tileMapController = world.GetNode<TileMapController>("TileMapController");
		_pathfinder = world.GetNode<Pathfinder>("Pathfinder");

		_unitLayer = new TileMapLayer();
		_unitLayer.ZIndex = 2;
		_unitLayer.Name = "UnitLayer";
		_unitLayer.TileSet = _tileMapController.GetTileSet();
		AddChild(_unitLayer);

		formations = new HashSet<Formation>();
	}

	public override void _Input(InputEvent @event)
	{
		//if is a left mouse click
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			Vector2I cell = _tileMapController.GetSelectionTile();
			if (cell.X != -1 && cell.Y != -1)
			{
				AddFormation(cell);
			}
		}

		if (@event is InputEventKey key && key.Pressed)
		{
			switch (key.Keycode)
			{
				case Key.Key1:
					direction = Direction.NORTH;
					break;
				case Key.Key2:
					direction = Direction.NORTH_EAST;
					break;
				case Key.Key3:
					direction = Direction.EAST;
					break;
				case Key.Key4:
					direction = Direction.SOUTH_EAST;
					break;
				case Key.Key5:
					direction = Direction.SOUTH;
					break;
				case Key.Key6:
					direction = Direction.SOUTH_WEST;
					break;
				case Key.Key7:
					direction = Direction.WEST;
					break;
				case Key.Key8:
					direction = Direction.NORTH_WEST;
					break;
				case Key.C:
					MoveFormationsOnPath();
					break;
				case Key.Space:
					if (debugFormation != null)
					{
						debugFormation.SetWaypoint(_tileMapController.GetSelectionTile(), direction);
					}
					break;
			}
		}
	}

	public TileMapLayer GetUnitLayer()
	{
		return _unitLayer;
	}

	/// <summary>
	/// Adds a unit to the unit layer at the specified cell.
	/// </summary>
	/// <param name="cell">The cell on the map where the unit should be placed.</param>
	public void AddFormation(Vector2I cell)
	{
		if (debugFormation == null)
		{
			debugFormation = new Formation();
			debugFormation.Name = "DebugFormation";
			AddChild(debugFormation);
			formations.Add(debugFormation);
		}
		debugFormation.MoveToTile(cell, direction);
	}

	public void SetWaypoint(Formation formation, Vector2I cell, Direction direction)
	{
		formation.SetWaypoint(cell, direction);
	}

	public void MoveFormationsOnPath()
	{
		foreach (Formation formation in formations)
		{
			formation.MoveUnitsOnPath();
		}
	}

	public void _on_unit_move_attempted(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);

		if (_unitLayer.GetCellSourceId(cellTo) != -1)
		{
			// unit.
		}

		_unitLayer.SetCell(cellFrom, 2, new Vector2I(-1, -1));
		_unitLayer.SetCell(cellTo, 2, UnitType.BLUE_INF.AtlasCoords);

		unit.UpdateRealPosition(cellTo, topLayer);
	}

	public void _on_unit_waypoint_updated(Unit unit, Vector2I cellFrom, Vector2I cellTo, Direction direction)
	{
		List<Vector2I> path = _pathfinder.FindPath(cellFrom, cellTo);
		unit.SetPath(path);
		unit.UpdateDirection(direction);
	}
}
