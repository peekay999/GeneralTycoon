using Godot;
using System;

public partial class World : Node2D
{
	TileMapController _tileMapController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMapController = GetNode<TileMapController>("TileMapController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector2 MapToTopLayerLocal(Vector2I cell)
	{
		Vector2 YOffset = new Vector2(0, _tileMapController.GetTopLayerOffset(cell));
		Vector2 cellPos = _tileMapController.GetTileMapLayers()[0].MapToLocal(cell);
		return cellPos + YOffset;
	}
}
