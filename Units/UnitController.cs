using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Controller class for all units on the map. Handles unit creation and positioning.
/// </summary>
public partial class UnitController : Node2D
{
	private Node2D _world;
	private TileMapController _tileMapController;
	private SelectionLayer _selectionLayer;
	private Pathfinder _pathfinder;
	private TileMapLayer _unitLayer;
	private HashSet<Formation> _formations;

	private Formation _debugFormation;
	private Direction _direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetParent<Node2D>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
		_pathfinder = _world.GetNode<Pathfinder>("Pathfinder");
		_selectionLayer = _world.GetNode<SelectionLayer>("SelectionLayer");

		_unitLayer = new TileMapLayer();
		_unitLayer.ZIndex = 2;
		_unitLayer.Name = "UnitLayer";
		_unitLayer.TileSet = _tileMapController.GetTileSet();
		AddChild(_unitLayer);

		_formations = new HashSet<Formation>();

		AddFormation(new Vector2I(30, 30));
	}

	public override void _Input(InputEvent @event)
	{
		//if is a left mouse click
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			// Vector2I cell = _selectionLayer.GetSelectedCell();
			// if (cell.X != -1 && cell.Y != -1)
			// {
			// 	AddFormation(cell);
			// }
		}

		if (@event is InputEventKey key && key.Pressed)
		{
			switch (key.Keycode)
			{
				case Key.Key1:
					_direction = Direction.NORTH;
					break;
				case Key.Key2:
					_direction = Direction.NORTH_EAST;
					break;
				case Key.Key3:
					_direction = Direction.EAST;
					break;
				case Key.Key4:
					_direction = Direction.SOUTH_EAST;
					break;
				case Key.Key5:
					_direction = Direction.SOUTH;
					break;
				case Key.Key6:
					_direction = Direction.SOUTH_WEST;
					break;
				case Key.Key7:
					_direction = Direction.WEST;
					break;
				case Key.Key8:
					_direction = Direction.NORTH_WEST;
					break;
				case Key.C:
					MoveFormationsOnPath();
					break;
				case Key.Space:
					if (_debugFormation != null)
					{
						_debugFormation.SetWaypoint(_selectionLayer.GetSelectedCell(), _direction);
					}
					break;
				// case Key.Y:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.PivotLeft();
				// 	}
				// 	break;

				// case Key.U:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.PivotRight();
				// 	}
				// 	break;

				// case Key.I:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.RetireLeft();
				// 	}
				// 	break;
				// case Key.O:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.RetireRight();
				// 	}
				// 	break;
				// 	case Key.P:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.Retire();
				// 	}
				// 	break;
				// 	case Key.L:
				// 	if (_debugFormation != null)
				// 	{
				// 		_debugFormation.Advance();
				// 	}
				// 	break;

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
		if (_debugFormation == null)
		{
			_debugFormation = new Formation();
			_debugFormation.Name = "DebugFormation";
			AddChild(_debugFormation);
			_formations.Add(_debugFormation);
		}
		_debugFormation.MoveToTile(cell, _direction);
	}

	public void SetWaypoint(Formation formation, Vector2I cell, Direction direction)
	{
		formation.SetWaypoint(cell, direction);
	}

	public void MoveFormationsOnPath()
	{
		foreach (Formation formation in _formations)
		{
			formation.MoveUnitsOnPath();
		}
	}

	public void _on_unit_move_attempted(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);

		// if (_unitLayer.GetCellSourceId(cellTo) != -1)
		// {
		// 	// unit.
		// }

		_unitLayer.SetCell(cellFrom, 2, new Vector2I(-1, -1));
		_unitLayer.SetCell(cellTo, 2, UnitType.BLUE_INF.AtlasCoords);

		unit.UpdateRealPosition(cellTo, topLayer);
	}

	public async void _on_unit_waypoint_updated(Unit unit, Vector2I cellFrom, Vector2I cellTo, Direction direction)
	{
		List<Vector2I> path = await _pathfinder.FindPathAsync(cellFrom, cellTo);
		// List<Vector2I> path = _pathfinder.FindPath(cellFrom, cellTo);
		unit.SetPath(path);
		unit.UpdateDirection(direction);
	}
}
