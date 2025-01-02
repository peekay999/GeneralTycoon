using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class TileMapController : Node2D
{
	[Export]
	private Texture2D _heightMap;
	[Export]
	private int _totalLayers;
	[Export]
	private int _smoothingIterations;
	[Export]
	private TileSet _tileSet;

	private TileMapGenerator tileMapGenerator;
	private List<TileMapLayer> tileMapLayers;
	private TileMapLayer selectionLayer;
	private Node2D world;
	private UnitController unitController;

	public override void _Ready()
	{
		YSortEnabled = true;
		world = GetParent<Node2D>();
		unitController = world.GetNode<UnitController>("UnitController");

		tileMapLayers = new List<TileMapLayer>();
		selectionLayer = new TileMapLayer();
		selectionLayer.Name = "SelectionLayer";
		selectionLayer.TileSet = _tileSet;
		selectionLayer.TextureFilter = TextureFilterEnum.Nearest;
		AddChild(selectionLayer);
		selectionLayer.ZIndex = 1;

		tileMapGenerator = GetChild<TileMapGenerator>(0);

		if (_heightMap == null || _tileSet == null)
		{
			GD.PrintErr("HeightMap or BaseLayer not set");
			return;
		}
		else
		{
			GenerateTileMap();
		}

	}

	public override void _Process(double delta)
	{
		GetSelectionTile();
	}
	
	public Vector2I GetSelectionTile()
	{
		Vector2 globalMousePos = GetGlobalMousePosition();
		selectionLayer.Clear();

		for (int i = tileMapLayers.Count; i > 0; i--)
		{
			TileMapLayer currentLayer = tileMapLayers[i - 1];
			Vector2 localMousePos = ToLocal(globalMousePos) - currentLayer.Position;

			// Adjust for the Y offset of each layer
			localMousePos.Y += (tileMapLayers.Count - i) * 16;

			Vector2I cell = currentLayer.LocalToMap(localMousePos);

			if (currentLayer.GetCellSourceId(cell) != -1 && currentLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_corner_double_SE)
			{

				selectionLayer.Position = currentLayer.Position;
				selectionLayer.ZIndex = currentLayer.ZIndex;
				selectionLayer.YSortEnabled = true;
				selectionLayer.YSortOrigin = currentLayer.YSortOrigin + 1;
				Vector2I cellAtlasCoords = currentLayer.GetCellAtlasCoords(cell);
				selectionLayer.SetCell(cell, 1, cellAtlasCoords);
				return cell;
			}
		}
		return new Vector2I(-1, -1);
	}

	private void GenerateTileMap()
	{
		tileMapLayers = tileMapGenerator.GenerateTileMapLayers(_heightMap, _totalLayers, _tileSet);
		foreach (TileMapLayer layer in tileMapLayers)
		{
			AddChild(layer);
		}
		tileMapGenerator.CalculateEdgePieces(tileMapLayers, _smoothingIterations);
	}

	public TileMapLayer GetTopLayer(Vector2I cell)
	{
		for (int i = tileMapLayers.Count; i > 0; i--)
		{
			if (tileMapLayers[i - 1].GetCellSourceId(cell) != -1)
			{
				return tileMapLayers[i - 1];
			}
		}
		return null;
	}
}

