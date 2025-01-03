using Godot;
using Godot.NativeInterop;
using System;
using System.Runtime.ConstrainedExecution;

public static class UnitUtil
{
	public static Vector2[] POLY_BASE = new Vector2[]
	{
		new Vector2(0, 0),
		new Vector2(32, 16),
		new Vector2(0, 32),
		new Vector2(-32, 16)
	}; 

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

