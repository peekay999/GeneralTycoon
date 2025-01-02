using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

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

	[Export]
	private CompressedTexture2D mouseFollowTexture;
	private TileMapGenerator tileMapGenerator;
	private Sprite2D mouseFollowSprite;
	private List<TileMapLayer> tileMapLayers;
	private TileMapLayer selectionLayer;
	private Node2D world;



	public override void _Ready()
	{
		YSortEnabled = true;
		// get the parent node
		world = GetParent<Node2D>();
		mouseFollowSprite = new Sprite2D() { Texture = mouseFollowTexture, ZIndex = 0, YSortEnabled = true };
		AddChild(mouseFollowSprite);

		tileMapLayers = new List<TileMapLayer>();
		selectionLayer = new TileMapLayer();
		selectionLayer.Name = "SelectionLayer";
		selectionLayer.TileSet = _tileSet;
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

	public override void _Input(InputEvent @event)
	{
		//if is a left mouse click
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			Vector2I cell = DetermineSelectionOverlay();
			if (cell.X != -1 && cell.Y != -1)
			{
				PackedScene unitTest = (PackedScene)ResourceLoader.Load("res://Units/British_colour_01.tscn");
				if (unitTest != null)
				{
					Unit unit = (Unit)unitTest.Instantiate();
					unit.GlobalPosition = tileMapLayers[0].MapToLocal(cell) + selectionLayer.Position;
					// unit.MoveLocalY(-16);
					unit.ZIndex = 0;
					unit.YSortEnabled = true;
					AddChild(unit);
					unit.updateYoffset((int)selectionLayer.Position.Y);
					if (selectionLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
					{
						unit.GlobalPosition += new Vector2(0, 16);
					}
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		DetermineSelectionOverlay();
		// mouseFollowSprite.GlobalPosition = GetGlobalMousePosition();

	}

	private Vector2I DetermineSelectionOverlay()
	{
		for (int i = tileMapLayers.Count; i > 0; i--)
		{
			Vector2 mousePos = GetGlobalMousePosition() - tileMapLayers[i - 1].Position;
			selectionLayer.Clear();
			Vector2I cell = tileMapLayers[i - 1].LocalToMap(ToLocal(mousePos));

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

				selectionLayer.Position = tileMapLayers[layer].Position;
				selectionLayer.ZIndex = tileMapLayers[layer].ZIndex;
				selectionLayer.YSortEnabled = true;
				selectionLayer.YSortOrigin = tileMapLayers[layer].YSortOrigin + 1;
				Vector2I cellAtlasCoords = tileMapLayers[layer].GetCellAtlasCoords(cell);
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

	// private void GenerateEdges()
	// {
	// 	Vector2I NW;
	// 	Vector2I N;
	// 	Vector2I NE;
	// 	Vector2I E;
	// 	Vector2I SE;
	// 	Vector2I S;
	// 	Vector2I SW;
	// 	Vector2I W;

	// 	for (int iter = 0; iter < _smoothingIterations; iter++)
	// 	{
	// 		for (int i = 0; i < tileMapLayers.Count - 1; i++)
	// 		{
	// 			TileMapLayer currentLayer = tileMapLayers[i];
	// 			TileMapLayer nextLayer = tileMapLayers[i + 1];
	// 			Array<Vector2I> cells = currentLayer.GetUsedCells();
	// 			foreach (Vector2I cell in cells)
	// 			{
	// 				int tileIndex = 0;

	// 				/* Cardinal directions of surrounding tiles*/

	// 				NW = new Vector2I(cell.X - 1, cell.Y - 1);
	// 				N = new Vector2I(cell.X, cell.Y - 1);
	// 				NE = new Vector2I(cell.X + 1, cell.Y - 1);
	// 				E = new Vector2I(cell.X + 1, cell.Y);
	// 				SE = new Vector2I(cell.X + 1, cell.Y + 1);
	// 				S = new Vector2I(cell.X, cell.Y + 1);
	// 				SW = new Vector2I(cell.X - 1, cell.Y + 1);
	// 				W = new Vector2I(cell.X - 1, cell.Y);

	// 				if (nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_slope_E
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_slope_S
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_corner_SE
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_corner_high_SW
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_corner_high_NE
	// 				|| nextLayer.GetCellAtlasCoords(NW) == TileMapUtil.tile_corner_high_SE
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_slope_W
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_corner_SW
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_corner_high_NW
	// 				)
	// 				{
	// 					tileIndex |= 1;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_slope_S
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_corner_high_SW
	// 				|| nextLayer.GetCellAtlasCoords(N) == TileMapUtil.tile_corner_high_SE
	// 				)
	// 				{
	// 					tileIndex |= 1;
	// 					tileIndex |= 2;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_slope_S
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_slope_W
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_corner_SW
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_corner_high_SW
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_corner_high_NW
	// 				|| nextLayer.GetCellAtlasCoords(NE) == TileMapUtil.tile_corner_high_SE
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_slope_N
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_corner_NW
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_corner_high_NE
	// 				)
	// 				{
	// 					tileIndex |= 2;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_slope_W
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_corner_high_SW
	// 				|| nextLayer.GetCellAtlasCoords(E) == TileMapUtil.tile_corner_high_NW
	// 				)
	// 				{
	// 					tileIndex |= 2;
	// 					tileIndex |= 4;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_slope_N
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_slope_W
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_corner_NW
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_corner_high_SW
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_corner_high_NW
	// 				|| nextLayer.GetCellAtlasCoords(SE) == TileMapUtil.tile_corner_high_NE
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_slope_E
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_corner_NE
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_corner_high_SE
	// 				)
	// 				{
	// 					tileIndex |= 4;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_slope_N
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_corner_high_NW
	// 				|| nextLayer.GetCellAtlasCoords(S) == TileMapUtil.tile_corner_high_NE
	// 				)
	// 				{
	// 					tileIndex |= 4;
	// 					tileIndex |= 8;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_slope_N
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_slope_E
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_corner_NE
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_corner_high_NW
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_corner_high_NE
	// 				|| nextLayer.GetCellAtlasCoords(SW) == TileMapUtil.tile_corner_high_SE
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_slope_S
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_corner_SE
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_corner_high_SW
	// 				)
	// 				{
	// 					tileIndex |= 8;
	// 				}
	// 				if (nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_base
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_slope_E
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_corner_high_NE
	// 				|| nextLayer.GetCellAtlasCoords(W) == TileMapUtil.tile_corner_high_SE
	// 				)
	// 				{
	// 					tileIndex |= 8;
	// 					tileIndex |= 1;
	// 				}
	// 				if (nextLayer.GetCellSourceId(cell) != -1)
	// 				{
	// 					tileIndex = 0;
	// 				}

	// 				HashSet<Vector2I> allowedTiles_N;
	// 				HashSet<Vector2I> allowedTiles_E;
	// 				HashSet<Vector2I> allowedTiles_S;
	// 				HashSet<Vector2I> allowedTiles_W;

	// 				switch (tileIndex)
	// 				{
	// 					case 0:
	// 						break;
	// 					case 1:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_NW);

	// 						allowedTiles_S = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_NW,
	// 							TileMapUtil.tile_corner_NW,
	// 							TileMapUtil.tile_corner_high_SW,
	// 							TileMapUtil.tile_slope_W,
	// 							TileMapUtil.tile_corner_double_SW,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_slope_S,
	// 							TileMapUtil.tile_corner_SE
	// 						};
	// 						allowedTiles_E = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_NW,
	// 							TileMapUtil.tile_corner_NW,
	// 							TileMapUtil.tile_corner_high_NE,
	// 							TileMapUtil.tile_slope_N,
	// 							TileMapUtil.tile_corner_double_NE,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_slope_E,
	// 							TileMapUtil.tile_corner_SE
	// 						};
	// 						if (allowedTiles_S.Contains(currentLayer.GetCellAtlasCoords(S)) && allowedTiles_E.Contains(currentLayer.GetCellAtlasCoords(E)))
	// 						{
	// 							currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NW);
	// 							// currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NW_walls);
	// 						}

	// 						break;
	// 					case 2:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_NE);

	// 						allowedTiles_S = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_NE,
	// 							TileMapUtil.tile_corner_double_SE,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_slope_E,
	// 							TileMapUtil.tile_corner_SE,
	// 							TileMapUtil.tile_slope_S,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_corner_high_SE,
	// 						};
	// 						allowedTiles_W = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_NW,
	// 							TileMapUtil.tile_corner_double_NE,
	// 							TileMapUtil.tile_corner_NW,
	// 							TileMapUtil.tile_slope_N,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_slope_W,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_corner_high_NW
	// 						};
	// 						if (allowedTiles_S.Contains(currentLayer.GetCellAtlasCoords(S)) && allowedTiles_W.Contains(currentLayer.GetCellAtlasCoords(W)))
	// 						{
	// 							currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NE);
	// 							// currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NE_walls);
	// 						}

	// 						break;
	// 					case 3:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_slope_N);
	// 						break;
	// 					case 4:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_SE);

	// 						allowedTiles_N = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_SE,
	// 							TileMapUtil.tile_corner_SE,
	// 							TileMapUtil.tile_corner_high_NE,
	// 							TileMapUtil.tile_slope_E,
	// 							TileMapUtil.tile_corner_double_NE,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_slope_N,
	// 							TileMapUtil.tile_corner_NW
	// 						};
	// 						allowedTiles_W = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_SE,
	// 							TileMapUtil.tile_corner_SE,
	// 							TileMapUtil.tile_corner_high_SW,
	// 							TileMapUtil.tile_slope_S,
	// 							TileMapUtil.tile_corner_double_SW,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_slope_W,
	// 							TileMapUtil.tile_corner_NW
	// 						};
	// 						if (allowedTiles_N.Contains(currentLayer.GetCellAtlasCoords(N)) && allowedTiles_W.Contains(currentLayer.GetCellAtlasCoords(W)))
	// 						{
	// 							currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SE);
	// 							// currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SE_walls);
	// 						}

	// 						break;
	// 					case 5:
	// 						nextLayer.SetCell(cell, 0, TileMapUtil.tile_base);
	// 						break;
	// 					case 6:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_slope_E);
	// 						break;
	// 					case 7:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_NE);
	// 						break;
	// 					case 8:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_SW);

	// 						allowedTiles_N = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_SW,
	// 							TileMapUtil.tile_corner_double_NW,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_slope_W,
	// 							TileMapUtil.tile_corner_NW,
	// 							TileMapUtil.tile_slope_N,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_corner_high_NW,
	// 						};
	// 						allowedTiles_E = new HashSet<Vector2I>
	// 						{
	// 							TileMapUtil.tile_none,
	// 							TileMapUtil.tile_corner_double_SE,
	// 							TileMapUtil.tile_corner_double_SW,
	// 							TileMapUtil.tile_corner_SE,
	// 							TileMapUtil.tile_slope_S,
	// 							TileMapUtil.tile_corner_SW,
	// 							TileMapUtil.tile_slope_E,
	// 							TileMapUtil.tile_corner_NE,
	// 							TileMapUtil.tile_corner_high_SE
	// 						};
	// 						if (allowedTiles_N.Contains(currentLayer.GetCellAtlasCoords(N)) && allowedTiles_E.Contains(currentLayer.GetCellAtlasCoords(E)))
	// 						{
	// 							currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SW);
	// 							// currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SW_walls);
	// 						}

	// 						break;
	// 					case 9:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_slope_W);
	// 						break;
	// 					case 10:
	// 						nextLayer.SetCell(cell, 0, TileMapUtil.tile_base);
	// 						break;
	// 					case 11:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_NW);
	// 						break;
	// 					case 12:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_slope_S);
	// 						break;
	// 					case 13:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_SW);
	// 						break;
	// 					case 14:
	// 						currentLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_SE);
	// 						break;
	// 					case 15:
	// 						nextLayer.SetCell(cell, 0, TileMapUtil.tile_base);
	// 						break;
	// 				}
	// 			}
	// 		}


	// 	}
	// 	for (int i = 0; i < _totalLayers - 1; i++)
	// 	{
	// 		TileMapLayer currentLayer = tileMapLayers[i];
	// 		TileMapLayer nextLayer = tileMapLayers[i + 1];

	// 		foreach (Vector2I cell in currentLayer.GetUsedCells())
	// 		{
	// 			if (nextLayer.GetCellSourceId(cell) != -1)
	// 			{
	// 				currentLayer.SetCell(cell, 0, TileMapUtil.tile_base);
	// 			}
	// 			if (nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_NE || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_NW || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_SE || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_SW)
	// 			{
	// 				currentLayer.SetCell(cell, 0, TileMapUtil.tile_debug_1);
	// 			}
	// 		}
	// 	}
	// 	for (int i = 0; i < _totalLayers - 1; i++)
	// 	{
	// 		TileMapLayer currentLayer = tileMapLayers[i];
	// 		TileMapLayer nextLayer = tileMapLayers[i + 1];

	// 		foreach (Vector2I cell in currentLayer.GetUsedCells())
	// 		{
	// 			if (currentLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_debug_1)
	// 			{
	// 				if (nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_debug_1)
	// 				{
	// 					currentLayer.SetCell(cell, 0, TileMapUtil.tile_base);
	// 				}
	// 				else
	// 				{
	// 					currentLayer.SetCell(cell, 0, TileMapUtil.tile_debug_2);
	// 				}
	// 			}
	// 		}
	// 	}
	// }
}

