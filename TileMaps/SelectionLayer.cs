using Godot;
using System;
using System.Collections.Generic;

public partial class SelectionLayer : TileMapLayer
{
	public bool _isSelecting;
	private World _world;
	private TileMapController _tileMapController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_world = GetParent<World>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Gets the tile at the current mouse position.
	/// </summary>
	/// <returns>The tile at the mouse's current position</returns>
	public void DetermineSelectionCell()
	{
		List<TileMapLayer> tileMapLayers = _tileMapController.GetTileMapLayers();
		if (!_isSelecting)
		{
			Clear();
			return;
		}
		for (int i = tileMapLayers.Count - 1; i > 0; i--)
		{
			Vector2 mousePos = GetGlobalMousePosition() - tileMapLayers[i - 1].Position + new Vector2(0, 2);
			Clear();
			Vector2I cell = tileMapLayers[i].LocalToMap(ToLocal(mousePos));

			int layer = i - 1;
			int totalLayers = tileMapLayers.Count;
			if (tileMapLayers[layer].GetCellSourceId(cell) != -1 && (tileMapLayers[layer].GetCellAtlasCoords(cell) != TileMapUtil.tile_corner_double_SE))
			{
				// check the layer above for a tile
				while (i < totalLayers)
				{
					if (tileMapLayers[i].GetCellSourceId(cell) != -1)
					{
						layer = i;
					}
					i++;
				}

				Position = tileMapLayers[layer].Position;
				ZIndex = tileMapLayers[layer].ZIndex;
				YSortEnabled = true;
				YSortOrigin = tileMapLayers[layer].YSortOrigin + 1;
				YSortOrigin = tileMapLayers[layer].YSortOrigin + 1;
				Vector2I cellAtlasCoords = tileMapLayers[layer].GetCellAtlasCoords(cell);
				SetCell(cell, 1, cellAtlasCoords);
				return;
			}
		}
	}

	public Vector2I GetSelectedCell()
	{
		return GetUsedCells().Count > 0 ? GetUsedCells()[0] : new Vector2I(-1, -1);
	}
}
