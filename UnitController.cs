using Godot;
using System;

public partial class UnitController : Node2D
{
	private Node2D world;
	private TileMapController _tileMapController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		world = GetParent<Node2D>();
		_tileMapController = world.GetNode<TileMapController>("TileMapController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		MoveToTile(unit, cell, FrameDirection.NORTH);
	}

	public void MoveToTile(Unit unit, Vector2I cell, FrameDirection direction)
	{
		TileMapLayer layer = _tileMapController.GetTopLayer(cell);
		unit.MoveToTile(cell, layer, direction);
	}

	public void MoveToTile(Unit unit, Vector2I cell)
	{
		TileMapLayer layer = _tileMapController.GetTopLayer(cell);
		unit.MoveToTile(cell, layer);
	}


}
