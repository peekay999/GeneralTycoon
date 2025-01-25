using System.Collections.Generic;
using Godot;

/// <summary>
/// Utility class for tile maps.
/// </summary>
public static class TileMapUtil
{
	public const int TILE_WIDTH = 64;
	public const int TILE_HEIGHT = 32;
	/* ATLAS coords of tiles */
	public static readonly Vector2I tile_none = new Vector2I(-1, -1);
	public static readonly Vector2I tile_base = new Vector2I(0, 0);
	public static readonly Vector2I tile_corner_NW = new Vector2I(0, 1);
	public static readonly Vector2I tile_corner_NE = new Vector2I(2, 1);
	public static readonly Vector2I tile_corner_SE = new Vector2I(0, 2);
	public static readonly Vector2I tile_corner_SW = new Vector2I(2, 2);
	public static readonly Vector2I tile_corner_NW_SE = new Vector2I(1, 4);
	public static readonly Vector2I tile_corner_SW_NE = new Vector2I(2, 4);
	public static readonly Vector2I tile_slope_N = new Vector2I(1, 1);
	public static readonly Vector2I tile_slope_E = new Vector2I(3, 1);
	public static readonly Vector2I tile_slope_S = new Vector2I(1, 2);
	public static readonly Vector2I tile_slope_W = new Vector2I(3, 2);
	public static readonly Vector2I tile_corner_high_NW = new Vector2I(1, 3);
	public static readonly Vector2I tile_corner_high_NE = new Vector2I(2, 3);
	public static readonly Vector2I tile_corner_high_SE = new Vector2I(3, 3);
	public static readonly Vector2I tile_corner_high_SW = new Vector2I(0, 3);
	public static readonly Vector2I tile_corner_double_NW = new Vector2I(2, 0);
	public static readonly Vector2I tile_corner_double_NE = new Vector2I(3, 0);
	public static readonly Vector2I tile_corner_double_SE = new Vector2I(3, 4);
	public static readonly Vector2I tile_corner_double_SW = new Vector2I(1, 0);
	public static readonly Vector2I tile_debug_0 = new Vector2I(0, 4);

	private static readonly HashSet<Vector2I> sloped_tiles_R = new HashSet<Vector2I>
	{
		tile_slope_N,
		tile_slope_E,
		tile_corner_high_NE,
		tile_corner_NE,
		tile_corner_double_NE

	};

	private static readonly HashSet<Vector2I> sloped_tiles_L = new HashSet<Vector2I>
	{
		tile_slope_S,
		tile_slope_W,
		tile_corner_high_SW,
		tile_corner_SW,
		tile_corner_double_SW
	};

	public static float GetTileRotationAmount(Vector2I tile)
	{
		if (sloped_tiles_R.Contains(tile))
		{
			return -10.0f;
		}
		else if (sloped_tiles_L.Contains(tile))
		{
			return 10.0f;
		}
		return 0;
	}

		public static Direction DetermineDirection(Vector2I start, Vector2I end)
	{
		Vector2I diff = end - start;
		if (diff.X == 0 && diff.Y == 0)
		{
			return Direction.NORTH;
		}
		else if (diff.X == 0 && diff.Y < 0)
		{
			return Direction.NORTH;
		}
		else if (diff.X > 0 && diff.Y < 0)
		{
			return Direction.NORTH_EAST;
		}
		else if (diff.X > 0 && diff.Y == 0)
		{
			return Direction.EAST;
		}
		else if (diff.X > 0 && diff.Y > 0)
		{
			return Direction.SOUTH_EAST;
		}
		else if (diff.X == 0 && diff.Y > 0)
		{
			return Direction.SOUTH;
		}
		else if (diff.X < 0 && diff.Y > 0)
		{
			return Direction.SOUTH_WEST;
		}
		else if (diff.X < 0 && diff.Y == 0)
		{
			return Direction.WEST;
		}
		else if (diff.X < 0 && diff.Y < 0)
		{
			return Direction.NORTH_WEST;
		}
		return Direction.NORTH;
	}
}

/// <summary>
/// Enum for the different tile sets. Used to determine which tile set to use for a given tile.
/// </summary>
public enum TileSets
{
	BASE = 0,
	SELECTION = 1,
	UNITS = 2,
}