using Godot;

/// <summary>
/// Utility class for tile maps.
/// </summary>
public static class TileMapUtil
{
    	/* ATLAS coords of tiles */
		public static Vector2I tile_none = new Vector2I(-1, -1);
		public static Vector2I tile_base = new Vector2I(0, 0);

		public static Vector2I tile_corner_NW = new Vector2I(0, 1);
		public static Vector2I tile_corner_NE = new Vector2I(2, 1);
		public static Vector2I tile_corner_SE = new Vector2I(0, 2);
		public static Vector2I tile_corner_SW = new Vector2I(2, 2);
		public static Vector2I tile_corner_NW_SE = new Vector2I(1, 4);
		public static Vector2I tile_corner_SW_NE = new Vector2I(2, 4);

		public static Vector2I tile_slope_N = new Vector2I(1, 1);
		public static Vector2I tile_slope_E = new Vector2I(3, 1);
		public static Vector2I tile_slope_S = new Vector2I(1, 2);
		public static Vector2I tile_slope_W = new Vector2I(3, 2);

		public static Vector2I tile_corner_high_NW = new Vector2I(1, 3);
		public static Vector2I tile_corner_high_NE = new Vector2I(2, 3);
		public static Vector2I tile_corner_high_SE = new Vector2I(3, 3);
		public static Vector2I tile_corner_high_SW = new Vector2I(0, 3);

		public static Vector2I tile_corner_double_NW = new Vector2I(2, 0);
		public static Vector2I tile_corner_double_NE = new Vector2I(3, 0);
		public static Vector2I tile_corner_double_SE = new Vector2I(3, 4);
		public static Vector2I tile_corner_double_SW = new Vector2I(1, 0);

		public static Vector2I tile_debug_0 = new Vector2I(0, 4);
		public static Vector2I tile_debug_1 = new Vector2I(1, 4);
		public static Vector2I tile_debug_2 = new Vector2I(2, 4);
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