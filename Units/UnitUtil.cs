using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Utility class for units.
/// </summary>
public static class UnitUtil
{
	public const float TILE_WIDTH = 64.0f; // Adjusted width
	public const float TILE_HEIGHT = 32.0f; // Adjusted height

	/// <summary>
	/// Determines the direction from a start to an end point. Used for unit movement. 
	/// </summary>
	/// <param name="start">The starting point.</param>
	/// <param name="end">The ending point.</param>
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

	public static Direction GetClockwiseDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.NORTH:
				return Direction.NORTH_EAST;
			case Direction.NORTH_EAST:
				return Direction.EAST;
			case Direction.EAST:
				return Direction.SOUTH_EAST;
			case Direction.SOUTH_EAST:
				return Direction.SOUTH;
			case Direction.SOUTH:
				return Direction.SOUTH_WEST;
			case Direction.SOUTH_WEST:
				return Direction.WEST;
			case Direction.WEST:
				return Direction.NORTH_WEST;
			case Direction.NORTH_WEST:
				return Direction.NORTH;
			default:
				return Direction.NORTH;
		}
	}

	public static Direction GetAntiClockwiseDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.NORTH:
				return Direction.NORTH_WEST;
			case Direction.NORTH_WEST:
				return Direction.WEST;
			case Direction.WEST:
				return Direction.SOUTH_WEST;
			case Direction.SOUTH_WEST:
				return Direction.SOUTH;
			case Direction.SOUTH:
				return Direction.SOUTH_EAST;
			case Direction.SOUTH_EAST:
				return Direction.EAST;
			case Direction.EAST:
				return Direction.NORTH_EAST;
			case Direction.NORTH_EAST:
				return Direction.NORTH;
			default:
				return Direction.NORTH;
		}
	}

	public static Vector2[] GetSpritePositions(Direction direction)
	{
		Vector2[] positions;
		switch (direction)
		{
			case Direction.NORTH:
				positions = SpritePositions.NORTH;
				break;
			case Direction.NORTH_EAST:
				positions = SpritePositions.NORTH_EAST;
				break;
			case Direction.EAST:
				positions = SpritePositions.EAST;
				break;
			case Direction.SOUTH_EAST:
				positions = SpritePositions.SOUTH_EAST;
				break;
			case Direction.SOUTH:
				positions = SpritePositions.SOUTH;
				break;
			case Direction.SOUTH_WEST:
				positions = SpritePositions.SOUTH_WEST;
				break;
			case Direction.WEST:
				positions = SpritePositions.WEST;
				break;
			case Direction.NORTH_WEST:
				positions = SpritePositions.NORTH_WEST;
				break;
			default:
				positions = SpritePositions.NORTH;
				break;
		}
		return positions;
	}


	// public static Vector2[] GetSpritePositions(Direction direction, int unitCount)
	// {
	// 	Vector2[] result = new Vector2[unitCount];

	// 	if (unitCount <= 0 || unitCount > 9)
	// 		return result; // invalid unit count
	// 	int files = 3;
	// 	int ranks = (int)Mathf.Ceil(unitCount / files);

	// 	float totalWidth = TILE_WIDTH * 0.66f;
	// 	float totalHeight = TILE_HEIGHT * 0.66f;

	// 	Vector2 startPos = new Vector2(0, -TILE_HEIGHT / 2 * 0.66f);
	// 	float stepX = totalWidth / 6;
	// 	float stepY = stepX / 2;
	// 	for (int i = 0; i < unitCount; i++)
	// 	{
	// 		result[i] = startPos += new Vector2(stepX, stepY);
	// 	}
	// 	return result;
	// }
}


/// <summary>
/// Enum for the different directions a unit can face.
/// </summary>
public enum Direction
{

	/// <summary>
	/// Used for when moving in the same direction as before.
	/// </summary>

	CONTINUE = -1,
	NORTH_WEST = 0,
	WEST = 1,
	SOUTH_WEST = 2,
	SOUTH = 3,
	SOUTH_EAST = 4,
	EAST = 5,
	NORTH_EAST = 6,
	NORTH = 7
}

public struct Animations
{
	public const string STAND = "default";
	public const string WALK = "walk";
	public const string WALK_READY = "walk_ready";
}

public static class SpritePositions
{
	public static readonly Vector2[] NORTH = new Vector2[]
	{
		new Vector2(0, -12), new Vector2(12, -6), new Vector2(24, 0),
		new Vector2(-11, -5), new Vector2(1, 1), new Vector2(13, 7),
		new Vector2(-25, 1), new Vector2(-13, 7), new Vector2(-1, 13)
	};

	public static readonly Vector2[] NORTH_EAST = new Vector2[]
	{
		new Vector2(7, -13), new Vector2(8, -1), new Vector2(6, 9),
		new Vector2(-6, -12), new Vector2(-6, 0), new Vector2(-7, 8)
	};

	public static readonly Vector2[] EAST = new Vector2[]
	{
		new Vector2(-15, 4), new Vector2(-2, -2), new Vector2(10, -9),
		new Vector2(-3, 9), new Vector2(9, 4), new Vector2(20, -3)
	};

	public static readonly Vector2[] SOUTH_EAST = new Vector2[]
	{
		new Vector2(21, 7), new Vector2(-1, 7), new Vector2(-22, 7),
		new Vector2(20, -2), new Vector2(-1, -3), new Vector2(-22, -2)
	};

	public static readonly Vector2[] SOUTH = new Vector2[]
	{
		new Vector2(5, 10), new Vector2(-6, 6), new Vector2(-16, 0),
		new Vector2(14, 5), new Vector2(5, 0), new Vector2(-4, -4)
	};

	public static readonly Vector2[] SOUTH_WEST = new Vector2[]
	{
		new Vector2(-7, 8), new Vector2(-6, 0), new Vector2(-6, -12),
		new Vector2(6, 9), new Vector2(8, -1), new Vector2(7, -13)
	};

	public static readonly Vector2[] WEST = new Vector2[]
	{
		new Vector2(-15, 4), new Vector2(-2, -2), new Vector2(10, -9),
		new Vector2(-3, 9), new Vector2(9, 4), new Vector2(20, -3)
	};

	public static readonly Vector2[] NORTH_WEST = new Vector2[]
	{
		new Vector2(-22, -2), new Vector2(-1, -3), new Vector2(20, -2),
		new Vector2(-22, 7), new Vector2(-1, 7), new Vector2(21, 7)
	};
}