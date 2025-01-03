using Godot;


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

	public void AddUnit(Vector2I cell)
	{
		PackedScene unitScene = (PackedScene)ResourceLoader.Load("res://Units/British_troops_01.tscn");
		Unit unit = (Unit)unitScene.Instantiate();
		AddChild(unit);
		unit.MoveToTile(cell);
	}

	public void UpdateUnitPosition(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		_unitLayer.SetCell(cellFrom, -1);
		_unitLayer.SetCell(cellTo, (int)TileSets.UNITS, unit.GetUnitTypeAtlasCoords());

		unit.UpdateRealPosition(cellTo, _tileMapController.GetTopLayer(cellTo));
	}
}
