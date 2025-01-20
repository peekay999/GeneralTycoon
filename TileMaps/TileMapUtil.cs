using Godot;

/// <summary>
/// Utility class for tile maps.
/// </summary>
public static class TileMapUtil
{
    	/* ATLAS coords of tiles */
		public static readonly Vector2I TILE_NONE = new Vector2I(-1, -1);
		public static readonly Vector2I TILE_BASE = new Vector2I(0, 0);
		public static readonly Vector2I TILE_CORNER_NW = new Vector2I(0, 1);
		public static readonly Vector2I TILE_CORNER_NE = new Vector2I(2, 1);
		public static readonly Vector2I TILE_CORNER_SE = new Vector2I(0, 2);
		public static readonly Vector2I TILE_CORNER_SW = new Vector2I(2, 2);
		public static readonly Vector2I TILE_CORNER_NW_SE = new Vector2I(1, 4);
		public static readonly Vector2I TILE_CORNER_SW_NE = new Vector2I(2, 4);
		public static readonly Vector2I TILE_SLOPE_N = new Vector2I(1, 1);
		public static readonly Vector2I TILE_SLOPE_E = new Vector2I(3, 1);
		public static readonly Vector2I TILE_SLOPE_S = new Vector2I(1, 2);
		public static readonly Vector2I TILE_SLOPE_W = new Vector2I(3, 2);
		public static readonly Vector2I TILE_CORNER_HIGH_NW = new Vector2I(1, 3);
		public static readonly Vector2I TILE_CORNER_HIGH_NE = new Vector2I(2, 3);
		public static readonly Vector2I TILE_CORNER_HIGH_SE = new Vector2I(3, 3);
		public static readonly Vector2I TILE_CORNER_HIGH_SW = new Vector2I(0, 3);
		public static readonly Vector2I TILE_CORNER_DOUBLE_NW = new Vector2I(2, 0);
		public static readonly Vector2I TILE_CORNER_DOUBLE_NE = new Vector2I(3, 0);
		public static readonly Vector2I TILE_CORNER_DOUBLE_SE = new Vector2I(3, 4);
		public static readonly Vector2I TILE_CORNER_DOUBLE_SW = new Vector2I(1, 0);
		public static readonly Vector2I TILE_DEBUG_0 = new Vector2I(0, 4);
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