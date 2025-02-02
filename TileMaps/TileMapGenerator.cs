using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class TileMapGenerator : Node2D
{
	private int erasedTiles = 0;
	public List<TileMapLayer> GenerateTileMapLayers(Texture2D heightmap, int numberOfLayers, TileSet tileSet)
	{
		Image image = heightmap.GetImage();
		List<TileMapLayer> tileMapLayers = new List<TileMapLayer>();
		int Offset_Y = -TileMapUtil.TILE_HEIGHT / 2;

		if (image == null)
		{
			GD.PrintErr("HeightMap image is null");
			return null;
		}

		int imageWidth = image.GetWidth();
		int imageHeight = image.GetHeight();

		for (int i = 0; i < numberOfLayers; i++)
		{
			TileMapLayer tileMapLayer = new TileMapLayer()
			{
				TextureFilter = TextureFilterEnum.Nearest,
				Name = "Layer " + i,
				TileSet = tileSet,
				GlobalPosition = new Vector2(0, i * Offset_Y),
				YSortOrigin = -i * Offset_Y,
				YSortEnabled = true,
				ZIndex = 0,
				CollisionEnabled = false,
				NavigationEnabled = false,
				RenderingQuadrantSize = 16
			};

			tileMapLayers.Add(tileMapLayer);

			List<Vector2I> tilePositions = new List<Vector2I>();

			int threshold = 255 / numberOfLayers * i;

			for (int x = 0; x < imageWidth - 1; x++)
			{
				for (int y = 0; y < imageHeight - 1; y++)
				{
					if (image.GetPixel(x, y).R8 >= threshold)
					{
						tilePositions.Add(new Vector2I(x, y));
					}
				}
			}

			// Batch set tiles
			foreach (Vector2I position in tilePositions)
			{
				tileMapLayer.SetCell(position, 0, TileMapUtil.tile_base);
			}
		}
		return tileMapLayers;
	}

	public void CalculateEdgePieces(List<TileMapLayer> tileMapLayers, int smoothingIterations)
	{
		Vector2I NW;
		Vector2I N;
		Vector2I NE;
		Vector2I E;
		Vector2I SE;
		Vector2I S;
		Vector2I SW;
		Vector2I W;

		for (int iter = 0; iter < smoothingIterations; iter++)
		{
			for (int i = 0; i < tileMapLayers.Count - 1; i++)
			{
				TileMapLayer currentLayer = tileMapLayers[i];
				TileMapLayer nextLayer = tileMapLayers[i + 1];
				Array<Vector2I> cells = currentLayer.GetUsedCells();
				foreach (Vector2I cell in cells)
				{
					int tileIndex = 0;

					/* Cardinal directions of surrounding tiles*/

					NW = new Vector2I(cell.X - 1, cell.Y - 1);
					N = new Vector2I(cell.X, cell.Y - 1);
					NE = new Vector2I(cell.X + 1, cell.Y - 1);
					E = new Vector2I(cell.X + 1, cell.Y);
					SE = new Vector2I(cell.X + 1, cell.Y + 1);
					S = new Vector2I(cell.X, cell.Y + 1);
					SW = new Vector2I(cell.X - 1, cell.Y + 1);
					W = new Vector2I(cell.X - 1, cell.Y);

					tileIndex = CalculateTileIndex(nextLayer, cell);

					switch (tileIndex)
					{
						case 0:
							break;
						case 1:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_NW);

							if (IsTileMatch(currentLayer, S, new HashSet<Vector2I>
											{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_NW,
												TileMapUtil.tile_corner_NW,
												TileMapUtil.tile_corner_high_SW,
												TileMapUtil.tile_slope_W,
												TileMapUtil.tile_corner_double_SW,
												TileMapUtil.tile_corner_SW,
												TileMapUtil.tile_slope_S,
												TileMapUtil.tile_corner_SE
											})
							|| IsTileMatch(currentLayer, W, new HashSet<Vector2I>
							{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_NW,
												TileMapUtil.tile_corner_NW,
												TileMapUtil.tile_corner_high_SW,
												TileMapUtil.tile_slope_W,
												TileMapUtil.tile_corner_double_SW,
												TileMapUtil.tile_corner_SW,
												TileMapUtil.tile_slope_S,
												TileMapUtil.tile_corner_SE
							}))
							{
								nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NW);
							}

							break;
						case 2:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_NE);

							if (IsTileMatch(currentLayer, S, new HashSet<Vector2I>
											{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_NE,
												TileMapUtil.tile_corner_NE,
												TileMapUtil.tile_corner_high_SE,
												TileMapUtil.tile_slope_E,
												TileMapUtil.tile_corner_double_SE,
												TileMapUtil.tile_corner_SE,
												TileMapUtil.tile_slope_S,
												TileMapUtil.tile_corner_SW
											})
							|| IsTileMatch(currentLayer, E, new HashSet<Vector2I>
							{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_NE,
												TileMapUtil.tile_corner_NE,
												TileMapUtil.tile_corner_high_SE,
												TileMapUtil.tile_slope_E,
												TileMapUtil.tile_corner_double_SE,
												TileMapUtil.tile_corner_SE,
												TileMapUtil.tile_slope_S,
												TileMapUtil.tile_corner_SW
							}))
							{
								nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_NE);
							}

							break;
						case 3:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_slope_N);
							break;
						case 4:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_SE);

							if (IsTileMatch(currentLayer, N, new HashSet<Vector2I>
											{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_SE,
												TileMapUtil.tile_corner_SE,
												TileMapUtil.tile_corner_high_NE,
												TileMapUtil.tile_slope_E,
												TileMapUtil.tile_corner_double_NE,
												TileMapUtil.tile_corner_NE,
												TileMapUtil.tile_slope_N,
												TileMapUtil.tile_corner_NW
											})
							|| IsTileMatch(currentLayer, E, new HashSet<Vector2I>
							{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_SE,
												TileMapUtil.tile_corner_SE,
												TileMapUtil.tile_corner_high_NE,
												TileMapUtil.tile_slope_E,
												TileMapUtil.tile_corner_double_NE,
												TileMapUtil.tile_corner_NE,
												TileMapUtil.tile_slope_N,
												TileMapUtil.tile_corner_NW
							}))
							{
								nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SE);
							}

							break;
						case 5:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_NW_SE);
							break;
						case 6:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_slope_E);
							break;
						case 7:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_NE);
							break;
						case 8:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_SW);

							if (IsTileMatch(currentLayer, N, new HashSet<Vector2I>
											{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_SW,
												TileMapUtil.tile_corner_SW,
												TileMapUtil.tile_corner_high_NW,
												TileMapUtil.tile_slope_W,
												TileMapUtil.tile_corner_double_NW,
												TileMapUtil.tile_corner_NW,
												TileMapUtil.tile_slope_N,
												TileMapUtil.tile_corner_NE
											})
							|| IsTileMatch(currentLayer, W, new HashSet<Vector2I>
							{
												TileMapUtil.tile_none,
												TileMapUtil.tile_corner_double_SW,
												TileMapUtil.tile_corner_SW,
												TileMapUtil.tile_corner_high_NW,
												TileMapUtil.tile_slope_W,
												TileMapUtil.tile_corner_double_NW,
												TileMapUtil.tile_corner_NW,
												TileMapUtil.tile_slope_N,
												TileMapUtil.tile_corner_NE
							}))
							{
								nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_double_SW);
							}

							break;
						case 9:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_slope_W);
							break;
						case 10:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_SW_NE);
							break;
						case 11:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_NW);
							break;
						case 12:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_slope_S);
							break;
						case 13:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_SW);
							break;
						case 14:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_corner_high_SE);
							break;
						case 15:
							nextLayer.SetCell(cell, 0, TileMapUtil.tile_base);
							break;
					}
				}
			}


		}
		UpdateCurrentLayerTiles(tileMapLayers);
	}

	private int CalculateTileIndex(TileMapLayer nextLayer, Vector2I cell)
	{
		int tileIndex = 0;

		Vector2I NW = new Vector2I(cell.X - 1, cell.Y - 1);
		Vector2I N = new Vector2I(cell.X, cell.Y - 1);
		Vector2I NE = new Vector2I(cell.X + 1, cell.Y - 1);
		Vector2I E = new Vector2I(cell.X + 1, cell.Y);
		Vector2I SE = new Vector2I(cell.X + 1, cell.Y + 1);
		Vector2I S = new Vector2I(cell.X, cell.Y + 1);
		Vector2I SW = new Vector2I(cell.X - 1, cell.Y + 1);
		Vector2I W = new Vector2I(cell.X - 1, cell.Y);

		if (
			IsTileMatch(nextLayer, NW, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_E, TileMapUtil.tile_slope_S, TileMapUtil.tile_corner_SE, TileMapUtil.tile_corner_high_SW, TileMapUtil.tile_corner_high_NE, TileMapUtil.tile_corner_high_SE, TileMapUtil.tile_corner_NW_SE })
			|| IsTileMatch(nextLayer, N, new HashSet<Vector2I> { TileMapUtil.tile_slope_W, TileMapUtil.tile_corner_SW, TileMapUtil.tile_corner_high_NW })
		)
		{
			tileIndex |= 1;
		}
		if (IsTileMatch(nextLayer, N, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_S, TileMapUtil.tile_corner_high_SW, TileMapUtil.tile_corner_high_SE }))
		{
			tileIndex |= 1;
			tileIndex |= 2;
		}
		if (IsTileMatch(nextLayer, NE, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_S, TileMapUtil.tile_slope_W, TileMapUtil.tile_corner_SW, TileMapUtil.tile_corner_high_SW, TileMapUtil.tile_corner_high_NW, TileMapUtil.tile_corner_high_SE })
		|| IsTileMatch(nextLayer, E, new HashSet<Vector2I> { TileMapUtil.tile_slope_N, TileMapUtil.tile_corner_NW, TileMapUtil.tile_corner_high_NE })
		)
		{
			tileIndex |= 2;
		}
		if (IsTileMatch(nextLayer, E, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_W, TileMapUtil.tile_corner_high_SW, TileMapUtil.tile_corner_high_NW }))
		{
			tileIndex |= 2;
			tileIndex |= 4;
		}
		if (IsTileMatch(nextLayer, SE, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_N, TileMapUtil.tile_slope_W, TileMapUtil.tile_corner_NW, TileMapUtil.tile_corner_high_SW, TileMapUtil.tile_corner_high_NW, TileMapUtil.tile_corner_high_NE })
		|| IsTileMatch(nextLayer, S, new HashSet<Vector2I> { TileMapUtil.tile_slope_E, TileMapUtil.tile_corner_NE, TileMapUtil.tile_corner_high_SE })
		)
		{
			tileIndex |= 4;
		}
		if (IsTileMatch(nextLayer, S, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_N, TileMapUtil.tile_corner_high_NW, TileMapUtil.tile_corner_high_NE }))
		{
			tileIndex |= 4;
			tileIndex |= 8;
		}
		if (IsTileMatch(nextLayer, SW, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_N, TileMapUtil.tile_slope_E, TileMapUtil.tile_corner_NE, TileMapUtil.tile_corner_high_NW, TileMapUtil.tile_corner_high_NE, TileMapUtil.tile_corner_high_SE })
		|| IsTileMatch(nextLayer, W, new HashSet<Vector2I> { TileMapUtil.tile_slope_S, TileMapUtil.tile_corner_SE, TileMapUtil.tile_corner_high_SW })
		)
		{
			tileIndex |= 8;
		}
		if (IsTileMatch(nextLayer, W, new HashSet<Vector2I> { TileMapUtil.tile_base, TileMapUtil.tile_slope_E, TileMapUtil.tile_corner_high_NE, TileMapUtil.tile_corner_high_SE }))
		{
			tileIndex |= 8;
			tileIndex |= 1;
		}
		if (nextLayer.GetCellSourceId(cell) != -1)
		{
			tileIndex = 0;
		}

		return tileIndex;
	}

	// Returns true if the tile at the given cell is in the given set of tile types
	private bool IsTileMatch(TileMapLayer layer, Vector2I cell, HashSet<Vector2I> tileTypes)
	{
		return tileTypes.Contains(layer.GetCellAtlasCoords(cell));
	}

	private void UpdateCurrentLayerTiles(List<TileMapLayer> tileMapLayers)
	{
		for (int i = 0; i < tileMapLayers.Count - 1; i++)
		{
			TileMapLayer currentLayer = tileMapLayers[i];
			TileMapLayer nextLayer = tileMapLayers[i + 1];

			foreach (Vector2I cell in currentLayer.GetUsedCells())
			{
				if (nextLayer.GetCellSourceId(cell) != -1)
				{
					currentLayer.SetCell(cell, 0, TileMapUtil.tile_walls_only);
				}
			}
		}
		for (int i = 0; i < tileMapLayers.Count - 1; i++)
		{
			TileMapLayer currentLayer = tileMapLayers[i];
			TileMapLayer nextLayer = tileMapLayers[i + 1];

			foreach (Vector2I cell in currentLayer.GetUsedCells())
			{
				if (currentLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_walls_only)
				{
					if (nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_NE || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_NW || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_SE || nextLayer.GetCellAtlasCoords(cell) == TileMapUtil.tile_corner_double_SW)
					{
						currentLayer.EraseCell(cell);
					}
				}
			}
		}
	}
}
