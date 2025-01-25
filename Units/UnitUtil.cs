using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Utility class for units.
/// </summary>
public static class UnitUtil
{
	public const float TILE_WIDTH = TileMapUtil.TILE_WIDTH;
	public const float TILE_HEIGHT = TileMapUtil.TILE_HEIGHT;



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

	public static Vector2[,] GetSpritePositions(Direction direction)
	{
		Vector2[,] positions;

		float margin = 0.66f;

		float A = -TILE_HEIGHT / 2 * margin;
		float B = TILE_WIDTH / 2 * margin;
		float C = TILE_HEIGHT / 2 * margin;
		float D = -TILE_WIDTH / 2 * margin;

		float padding = A / 2;

		Vector2 pos_0_0 = new Vector2(0, A);
		Vector2 pos_1_0 = new Vector2(B / 2, A / 2);
		Vector2 pos_2_0 = new Vector2(B, 0);
		Vector2 pos_0_1 = new Vector2(D / 2, A / 2);
		Vector2 pos_1_1 = new Vector2(0, 0);
		Vector2 pos_2_1 = new Vector2(B / 2, C / 2);
		Vector2 pos_0_2 = new Vector2(D, 0);
		Vector2 pos_1_2 = new Vector2(D / 2, C / 2);
		Vector2 pos_2_2 = new Vector2(0, C);

		if (direction == Direction.NORTH_WEST || direction == Direction.SOUTH_WEST || direction == Direction.NORTH_EAST || direction == Direction.SOUTH_EAST)
		{
			if (direction == Direction.NORTH_WEST || direction == Direction.SOUTH_EAST)
			{
				A = -TILE_HEIGHT / 2 * (margin / 2);
				B = TILE_WIDTH / 2 * margin;
				C = TILE_HEIGHT / 2 * (margin / 2);
				D = -TILE_WIDTH / 2 * margin;
			}
			else if (direction == Direction.NORTH_EAST || direction == Direction.SOUTH_WEST)
			{
				A = -TILE_HEIGHT / 2 * margin;
				B = TILE_WIDTH / 2 * (margin / 2);
				C = TILE_HEIGHT / 2 * margin;
				D = -TILE_WIDTH / 2 * (margin / 2);
			}

			pos_0_0 = new Vector2(D, A + padding);
			pos_1_0 = new Vector2(0, A + padding);
			pos_2_0 = new Vector2(B, A + padding);
			pos_0_1 = new Vector2(D, 0 + padding);
			pos_1_1 = new Vector2(0, 0 + padding);
			pos_2_1 = new Vector2(B, 0 + padding);
			pos_0_2 = new Vector2(D, C + padding);
			pos_1_2 = new Vector2(0, C + padding);
			pos_2_2 = new Vector2(B, C + padding);
		}
		positions = new Vector2[3, 3] {
			{ pos_0_0, pos_0_1, pos_0_2 },
			{ pos_1_0, pos_1_1, pos_1_2 },
			{ pos_2_0, pos_2_1, pos_2_2 }
		};
		return positions;
	}
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