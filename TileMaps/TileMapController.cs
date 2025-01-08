
using System.Collections;
using System.Collections.Generic;
using Godot;

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
	private Node2D _world;
	private Pathfinder _pathfinder;
	private FormationController _unitController;
	private Dictionary<Vector2I, int> _tileHeights;
	private Dictionary<Vector2I, TileMapLayer> _topLayers;
	private readonly object _tileHeightsLock = new object();

	public override void _Ready()
	{
		YSortEnabled = true;
		_world = GetParent<Node2D>();
		_pathfinder = _world.GetNode<Pathfinder>("Pathfinder");
		_unitController = _world.GetNode<FormationController>("FormationController");

		tileMapLayers = new List<TileMapLayer>();

		_tileMapGenerator = GetChild<TileMapGenerator>(0);

		if (_heightMap == null || _tileSet == null)
		{
			GD.PrintErr("HeightMap or BaseLayer not set");
			return;
		}
		else
		{
			GetGeneratedTileMapLayers();

		}

	}

	public override void _Process(double delta)
	{
	}

	public List<TileMapLayer> GetTileMapLayers()
	{
		return tileMapLayers;
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
	private void GetGeneratedTileMapLayers()
	{
		tileMapLayers = _tileMapGenerator.GenerateTileMapLayers(_heightMap, _totalLayers, _tileSet);
		foreach (TileMapLayer layer in tileMapLayers)
		{
			AddChild(layer);
		}
		_tileMapGenerator.CalculateEdgePieces(tileMapLayers, _smoothingIterations);

		lock (_tileHeightsLock)
		{
			_tileHeights = new Dictionary<Vector2I, int>();
			_topLayers = new Dictionary<Vector2I, TileMapLayer>();
			List<Vector2I> usedCells = new List<Vector2I>(tileMapLayers[0].GetUsedCells());
			foreach (Vector2I cell in usedCells)
			{
				TileMapLayer topLayer = DetermineTopLayer(cell);
				_tileHeights.Add(cell, (int)topLayer.Position.Y);
				_topLayers.Add(cell, topLayer);
			}


		}
	}

	/// <summary>
	/// Gets the top layer at the specified cell.
	/// </summary>
	public int GetTopLayerOffset(Vector2I cell)
	{
		return _tileHeights.ContainsKey(cell) ? _tileHeights[cell] : 0;
	}

	/// <summary>
	/// Gets the top layer at the specified cell.
	/// </summary>
	public TileMapLayer GetTopLayer(Vector2I cell)
	{
		return _topLayers.ContainsKey(cell) ? _topLayers[cell] : null;
	}

	private TileMapLayer DetermineTopLayer(Vector2I cell)
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

