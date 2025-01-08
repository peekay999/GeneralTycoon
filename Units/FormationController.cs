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
	// private Direction _direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetParent<Node2D>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
		_pathfinder = _world.GetNode<Pathfinder>("Pathfinder");
		_selectionLayer = _world.GetNode<SelectionLayer>("SelectionLayer");
		_formationUiController = GetNode<FormationUiController>("FormationUiController");
		_units = new Dictionary<Unit, Vector2I>();

		_unitLayer = new TileMapLayer();
		_unitLayer.ZIndex = -1;
		_unitLayer.Name = "UnitLayer";
		_unitLayer.TileSet = _tileMapController.GetTileSet();
		AddChild(_unitLayer);

		_formations = new HashSet<Formation>();
		// _formations = new Dictionary<Formation, Vector2I>();

		AddCompany(new Vector2I(30, 30));

		AddCompany(new Vector2I(50, 40));

		AddCompany(new Vector2I(60, 50));
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
	public void AddCompany(Vector2I cell)
	{
		PackedScene companyScene = (PackedScene)ResourceLoader.Load("res://Units/company.tscn");
		Company company = (Company)companyScene.Instantiate();
		company.FormationSelected += () => _on_formation_selected(company);
		AddChild(company);
		_formations.Add(company);
		company.Name = "Formation " + _formations.Count;
		company.MoveToTile(cell, Direction.NORTH);
	}

	public Formation GetSelectedFormation()
	{
		return _formationUiController.GetSelectedFormation();
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

	private void UpdateUnitPosition(Unit unit, Vector2I cell, Vector2I targetCell)
	{
		_unitLayer.SetCell(cell, 2, new Vector2I(-1, -1));
		_unitLayer.SetCell(targetCell, 2, UnitType.BLUE_INF.AtlasCoords);
		_units[unit] = targetCell;
	}

	public void _on_formation_selected(ControlledFormation formation)
	{
		_formationUiController.SetFormation(formation);
	}

	public void _on_tileMover_move_attempted(TileMover unit, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);
		if (topLayer == null)
		{
			return;
		}
		unit.UpdateTransformPosition(cellTo, topLayer);
	}

	public void _on_unit_move_attempted(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		TileMapLayer topLayer = _tileMapController.GetTopLayer(cellTo);

		if (topLayer == null)
		{
			return;
		}
		UpdateUnitPosition(unit, cellFrom, cellTo);
		unit.UpdateTransformPosition(cellTo, topLayer);
	}

	public async void _on_unit_waypoint_updated(Unit unit, Vector2I cellFrom, Vector2I cellTo, Direction direction)
	{
		List<Vector2I> path = await _pathfinder.FindPathAsync(cellFrom, cellTo);
		unit.SetPath(path);
		unit.UpdateDirection(direction);
	}
}
