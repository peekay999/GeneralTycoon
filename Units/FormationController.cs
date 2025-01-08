using System.Collections.Generic;
using Godot;

/// <summary>
/// Controller class for all formations on the map. Handles unit creation and positioning.
/// </summary>
public partial class FormationController : Node2D
{
	private Node2D _world;
	private TileMapController _tileMapController;
	private SelectionLayer _selectionLayer;
	private FormationUiController _formationUiController;
	private Pathfinder _pathfinder;
	private TileMapLayer _unitLayer;
	private HashSet<Formation> _formations;
	// private Dictionary<Formation, Vector2I> _formations;
	private Dictionary<Unit, Vector2I> _units;

	private Formation _selectedFormation;
	// private Direction _direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetParent<Node2D>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
		_pathfinder = _world.GetNode<Pathfinder>("Pathfinder");
		_selectionLayer = _world.GetNode<SelectionLayer>("SelectionLayer");
		_formationUiController = GetNode<FormationUiController>("FormationUiController");
		_selectedFormation = null;
		_units = new Dictionary<Unit, Vector2I>();

		_unitLayer = new TileMapLayer();
		_unitLayer.ZIndex = -1;
		_unitLayer.Name = "UnitLayer";
		_unitLayer.TileSet = _tileMapController.GetTileSet();
		AddChild(_unitLayer);

		_formations = new HashSet<Formation>();
		// _formations = new Dictionary<Formation, Vector2I>();

		AddFormation(new Vector2I(30, 30));

		AddFormation(new Vector2I(50, 40));

		AddFormation(new Vector2I(60, 50));
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
			if (key.Keycode == Key.C)
			{
				MoveFormationsOnPath();
			}
			// switch (key.Keycode)
			// {
			// 	case Key.Key1:
			// 		_direction = Direction.NORTH;
			// 		break;
			// 	case Key.Key2:
			// 		_direction = Direction.NORTH_EAST;
			// 		break;
			// 	case Key.Key3:
			// 		_direction = Direction.EAST;
			// 		break;
			// 	case Key.Key4:
			// 		_direction = Direction.SOUTH_EAST;
			// 		break;
			// 	case Key.Key5:
			// 		_direction = Direction.SOUTH;
			// 		break;
			// 	case Key.Key6:
			// 		_direction = Direction.SOUTH_WEST;
			// 		break;
			// 	case Key.Key7:
			// 		_direction = Direction.WEST;
			// 		break;
			// 	case Key.Key8:
			// 		_direction = Direction.NORTH_WEST;
			// 		break;
			// 	case Key.C:
			// 		MoveFormationsOnPath();
			// 		break;
			// 	case Key.Space:
			// 		if (_selectedFormation != null)
			// 		{
			// 			_selectedFormation.SetWaypoint(_selectionLayer.GetSelectedCell(), _direction);
			// 		}
			// 		break;
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

			// }
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
		// if (_selectedFormation == null)
		// {
		// 	_selectedFormation = new Formation();
		// 	_selectedFormation.Name = "DebugFormation";
		// 	AddChild(_selectedFormation);
		// 	_formations.Add(_selectedFormation);
		// _selectedFormation.MoveToTile(cell, _direction);
		// }

		Formation formation = new Formation();
		formation.FormationSelected += () => _on_formation_selected(formation);
		AddChild(formation);
		_formations.Add(formation);
		formation.Name = "Formation " + _formations.Count;
		formation.MoveToTile(cell, Direction.NORTH);
	}

	public Formation GetSelectedFormation()
	{
		return _selectedFormation;
	}

	public void _on_formation_selected(Formation formation)
	{
		GD.Print("Formation " + formation.Name + " selected");
		_selectedFormation = formation;
		_formationUiController.SetFormation(formation);
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
		// foreach (Formation formation in _formations.Keys)
		// {
		// 	formation.MoveUnitsOnPath();
		// }
	}

	private void UpdateUnitPosition(Unit unit, Vector2I cell, Vector2I targetCell)
	{
		_unitLayer.SetCell(cell, 2, new Vector2I(-1, -1));
		_unitLayer.SetCell(targetCell, 2, UnitType.BLUE_INF.AtlasCoords);
		_units[unit] = targetCell;
	}

	public void _on_tileMover_move_attempted(TileMover unit, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);
		unit.UpdateTransformPosition(cellTo, topLayer);
	}

	public void _on_unit_move_attempted(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);

		UpdateUnitPosition(unit, cellFrom, cellTo);
		unit.UpdateTransformPosition(cellTo, topLayer);
	}

	public async void _on_unit_waypoint_updated(Unit unit, Vector2I cellFrom, Vector2I cellTo, Direction direction)
	{
		List<Vector2I> path = await _pathfinder.FindPathAsync(cellFrom, cellTo);
		// List<Vector2I> path = _pathfinder.FindPath(cellFrom, cellTo);
		unit.SetPath(path);
		unit.UpdateDirection(direction);
	}
}
