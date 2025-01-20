using Godot;
using System.Collections.Generic;

public partial class SelectionLayer : TileMapLayer
{
	private TileMapController _tileMapController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMapController = World.Instance.GetNode<TileMapController>("TileMapController");
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DetermineSelectionCell();
	}

	/// <summary>
	/// Gets the tile at the current mouse position.
	/// </summary>
	/// <returns>The tile at the mouse's current position</returns>
	public void DetermineSelectionCell()
	{
		Position = Vector2.Zero;
		List<TileMapLayer> tileMapLayers = _tileMapController.GetTileMapLayers();
		int layerCount = tileMapLayers.Count;
		Clear();
		for (int i = layerCount - 1; i >= 0; i--)
		{
			Position = tileMapLayers[i].Position;
			TileMapLayer tileMapLayer = tileMapLayers[i];
			Vector2 localMousePos = ToLocal(GetGlobalMousePosition());
			Vector2I cell = tileMapLayer.LocalToMap(localMousePos);

			int layer = i;
			if (tileMapLayer.GetCellSourceId(cell) != -1)
			{
				// check the layer above for a tile
				while (i < layerCount)
				{
					if (tileMapLayers[i].GetCellSourceId(cell) != -1)
					{
						layer = i;
					}
						i++;
				}
				tileMapLayer = tileMapLayers[layer];
				Position = tileMapLayer.Position - new Vector2(0, 1);
				YSortOrigin = tileMapLayer.YSortOrigin - 1;
				SetCell(cell, 1, tileMapLayer.GetCellAtlasCoords(cell));
				return;
			}
		}
	}

	public Vector2I GetSelectedCell()
	{
		return GetUsedCells().Count > 0 ? GetUsedCells()[0] : new Vector2I(-1, -1);
	}
}
