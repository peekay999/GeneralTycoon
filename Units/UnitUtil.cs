using Godot;

/// <summary>
/// Utility class for units.
/// </summary>
public static class UnitUtil
{
	/// <summary>
	/// Determines the direction from a start to an end point. Used for unit movement. 
	/// </summary>
	/// <param name="start">The starting point.</param>
	/// <param name="end">The ending point.</param>
	public static Direction DetermineDirection (Vector2I start, Vector2I end)
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
/// Enum for the different directions a unit can face.
/// </summary>
public enum Direction
{
	NORTH_WEST = 0,
	WEST = 1,
	SOUTH_WEST = 2,
	SOUTH = 3,
	SOUTH_EAST = 4,
	EAST = 5,
	NORTH_EAST = 6,
	NORTH = 7
}

