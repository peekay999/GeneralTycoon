using Godot;

/// <summary>
/// Controller class for all units on the map. Handles unit creation and positioning.
/// </summary>
public partial class UnitController : Node2D
{
	private Node2D world;
	private TileMapController _tileMapController;
	private TileMapLayer _unitLayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetParent<Node2D>();
		_tileMapController = world.GetNode<TileMapController>("TileMapController");
		_unitLayer = new TileMapLayer();
		_unitLayer.Name = "UnitLayer";
		_unitLayer.TileSet = _tileMapController.GetTileSet();
		AddChild(_unitLayer);
	}

	public override void _Input(InputEvent @event)
	{
		//if is a left mouse click
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			Vector2I cell = _tileMapController.GetSelectionTile();
			if (cell.X != -1 && cell.Y != -1)
			{
				AddUnit(cell);
			}
		}
	}

	/// <summary>
	/// Adds a unit to the unit layer at the specified cell.
	/// </summary>
	/// <param name="cell">The cell on the map where the unit should be placed.</param>
	public void AddUnit(Vector2I cell)
	{
		PackedScene unitScene = (PackedScene)ResourceLoader.Load("res://Units/British_troops_01.tscn");
		Unit unit = (Unit)unitScene.Instantiate();
		AddChild(unit);
		unit.MoveToTile(cell);
	}

	/// <summary>
	/// Updates the position of a unit on the map. This is called when a unit moves. Used to update the unit layer for unit position tracking and updating a unit's real transform position.
	/// </summary>
	/// <param name="unit">The unit that has moved.</param>
	/// <param name="cellFrom">The cell which the unit is moving from.</param>
	/// <param name="cellTo">The cell which the unit is moving to.</param>
	public void UpdateUnitPosition(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		_unitLayer.SetCell(cellFrom, -1);
		_unitLayer.SetCell(cellTo, (int)TileSets.UNITS, unit.GetUnitTypeAtlasCoords());

		unit.UpdateRealPosition(cellTo, _tileMapController.GetTopLayer(cellTo));
	}
}
