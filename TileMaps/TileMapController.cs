using Godot;
using Godot.Collections;
using System.Collections.Generic;

/// <summary>
/// Controller for the tile map. Handles the generation of the map by way of the TileMapGenerator and the selection of tiles.
/// </summary>
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

	private TileMapGenerator _tileMapGenerator;
	private List<TileMapLayer> tileMapLayers;
	private TileMapLayer _selectionLayer;
	private Node2D _world;
	private Pathfinder _pathfinder;
	private UnitController _unitController;

	public override void _Ready()
	{
		YSortEnabled = true;
		_world = GetParent<Node2D>();
		_pathfinder = GetNode<Pathfinder>("Pathfinder");
		_unitController = _world.GetNode<UnitController>("UnitController");

		tileMapLayers = new List<TileMapLayer>();
		_selectionLayer = new TileMapLayer();
		_selectionLayer.Name = "SelectionLayer";
		_selectionLayer.TileSet = _tileSet;
		_selectionLayer.TextureFilter = TextureFilterEnum.Nearest;
		AddChild(_selectionLayer);
		_selectionLayer.ZIndex = 1;

		_tileMapGenerator = GetChild<TileMapGenerator>(0);

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

	/// <summary>
	/// Gets the tile at the current mouse position.
	/// </summary>
	/// <returns>The tile at the mouse's current position</returns>
	public Vector2I GetSelectionTile()
	{
		for (int i = tileMapLayers.Count - 1; i > 0; i--)
		{
			Vector2 mousePos = GetGlobalMousePosition() - tileMapLayers[i - 1].Position + new Vector2(0, 2);
			_selectionLayer.Clear();
			Vector2I cell = tileMapLayers[i].LocalToMap(ToLocal(mousePos));

			int layer = i - 1;
			if (tileMapLayers[layer].GetCellSourceId(cell) != -1 && (tileMapLayers[layer].GetCellAtlasCoords(cell) != TileMapUtil.tile_corner_double_SE))
			{
				// check the layer above for a tile
				while (i < _totalLayers)
				{
					if (tileMapLayers[i].GetCellSourceId(cell) != -1)
					{
						layer = i;
					}
					i++;
				}

				_selectionLayer.Position = tileMapLayers[layer].Position;
				_selectionLayer.ZIndex = tileMapLayers[layer].ZIndex;
				_selectionLayer.YSortEnabled = true;
				_selectionLayer.YSortOrigin = tileMapLayers[layer].YSortOrigin + 1;
				Vector2I cellAtlasCoords = tileMapLayers[layer].GetCellAtlasCoords(cell);
				_selectionLayer.SetCell(cell, 1, cellAtlasCoords);
				return cell;
			}
		}
		return new Vector2I(-1, -1);
	}

	/// <summary>
	/// Gets the tile set used by the tile map.
	/// </summary>
	public TileSet GetTileSet()
	{
		return _tileSet;
	}

	/// <summary>
	/// Generates the tile map using the height map and tile set. Adds the layers to the scene.
	/// </summary>
	private void GenerateTileMap()
	{
		tileMapLayers = _tileMapGenerator.GenerateTileMapLayers(_heightMap, _totalLayers, _tileSet);
		foreach (TileMapLayer layer in tileMapLayers)
		{
			AddChild(layer);
		}
		_tileMapGenerator.CalculateEdgePieces(tileMapLayers, _smoothingIterations);
	}

	/// <summary>
	/// Gets the top layer at the specified cell.
	/// </summary>
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

