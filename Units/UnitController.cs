using Godot;

/// <summary>
/// Controller class for all units on the map. Handles unit creation and positioning.
/// </summary>
public partial class UnitController : Node2D
{
	private Node2D world;
	private TileMapController _tileMapController;
	private TileMapLayer _unitLayer;

	private Formation debugFormation;
	private Direction direction;
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

		// if is number 1 key
		if (@event is InputEventKey key && key.Pressed)
		{
			switch(key.Keycode)
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
			}
		}
	}

	/// <summary>
	/// Adds a unit to the unit layer at the specified cell.
	/// </summary>
	/// <param name="cell">The cell on the map where the unit should be placed.</param>
	public void AddUnit(Vector2I cell)
	{
		if (debugFormation == null)
		{
			debugFormation = new Formation();
			AddChild(debugFormation);
		}
		// Formation formation = new Formation();
		// AddChild(formation);
		debugFormation.MoveToTile(cell, direction);
	}

	/// <summary>
	/// Signal in. Handles the updating of a unit's position on the map.
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="cellFrom"></param>
	/// <param name="cellTo"></param>
	public void _on_unit_position_updated(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		_unitLayer.SetCell(cellFrom, -1);
		_unitLayer.SetCell(cellTo, (int)TileSets.UNITS, unit.GetUnitTypeAtlasCoords());

		unit.UpdateRealPosition(cellTo, _tileMapController.GetTopLayer(cellTo));
	}
}
