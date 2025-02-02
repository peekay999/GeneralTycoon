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

	private static Vector2[,] North_Positions = DetermineSpritePositions(Direction.NORTH);
	private static Vector2[,] North_East_Positions = DetermineSpritePositions(Direction.NORTH_EAST);
	private static Vector2[,] East_Positions = DetermineSpritePositions(Direction.EAST);
	private static Vector2[,] South_East_Positions = DetermineSpritePositions(Direction.SOUTH_EAST);
	private static Vector2[,] South_Positions = DetermineSpritePositions(Direction.SOUTH);
	private static Vector2[,] South_West_Positions = DetermineSpritePositions(Direction.SOUTH_WEST);
	private static Vector2[,] West_Positions = DetermineSpritePositions(Direction.WEST);
	private static Vector2[,] North_West_Positions = DetermineSpritePositions(Direction.NORTH_WEST);


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
		switch (direction)
		{
			case Direction.NORTH:
				return North_Positions;
			case Direction.NORTH_EAST:
				return North_East_Positions;
			case Direction.EAST:
				return East_Positions;
			case Direction.SOUTH_EAST:
				return South_East_Positions;
			case Direction.SOUTH:
				return South_Positions;
			case Direction.SOUTH_WEST:
				return South_West_Positions;
			case Direction.WEST:
				return West_Positions;
			case Direction.NORTH_WEST:
				return North_West_Positions;
			case Direction.CONTINUE:
			default:
				return North_Positions;
		}

	}

	private static Vector2[,] DetermineSpritePositions(Direction direction)
	{
		Vector2[,] positions;

		float margin = 0.66f;

		float TOP = -TILE_HEIGHT / 2 * margin;
		float RIGHT = TILE_WIDTH / 2 * margin;
		float BOTTOM = TILE_HEIGHT / 2 * margin;
		float LEFT = -TILE_WIDTH / 2 * margin;

		float padding = 0;

		Vector2 pos_0_0 = new Vector2(0, TOP);
		Vector2 pos_1_0 = new Vector2(RIGHT / 2, TOP / 2);
		Vector2 pos_2_0 = new Vector2(RIGHT, 0);
		Vector2 pos_0_1 = new Vector2(LEFT / 2, TOP / 2);
		Vector2 pos_1_1 = new Vector2(0, 0);
		Vector2 pos_2_1 = new Vector2(RIGHT / 2, BOTTOM / 2);
		Vector2 pos_0_2 = new Vector2(LEFT, 0);
		Vector2 pos_1_2 = new Vector2(LEFT / 2, BOTTOM / 2);
		Vector2 pos_2_2 = new Vector2(0, BOTTOM);

		if (direction == Direction.NORTH_WEST || direction == Direction.SOUTH_WEST || direction == Direction.NORTH_EAST || direction == Direction.SOUTH_EAST)
		{
			padding = TOP / 2;

			if (direction == Direction.NORTH_WEST || direction == Direction.SOUTH_EAST)
			{
				TOP = -TILE_HEIGHT / 2 * (margin / 2);
				RIGHT = TILE_WIDTH / 2 * margin;
				BOTTOM = TILE_HEIGHT / 2 * (margin / 2);
				LEFT = -TILE_WIDTH / 2 * margin;
			}
			else if (direction == Direction.NORTH_EAST || direction == Direction.SOUTH_WEST)
			{
				TOP = -TILE_HEIGHT / 2 * margin;
				RIGHT = TILE_WIDTH / 2 * (margin / 2);
				BOTTOM = TILE_HEIGHT / 2 * margin;
				LEFT = -TILE_WIDTH / 2 * (margin / 2);
			}

			pos_0_0 = new Vector2(LEFT, TOP);
			pos_1_0 = new Vector2(0, TOP);
			pos_2_0 = new Vector2(RIGHT, TOP);
			pos_0_1 = new Vector2(LEFT, 0);
			pos_1_1 = new Vector2(0, 0);
			pos_2_1 = new Vector2(RIGHT, 0);
			pos_0_2 = new Vector2(LEFT, BOTTOM);
			pos_1_2 = new Vector2(0, BOTTOM);
			pos_2_2 = new Vector2(RIGHT, BOTTOM);
		}
		positions = new Vector2[3, 3] {
			{ pos_0_0, pos_0_1, pos_0_2 },
			{ pos_1_0, pos_1_1, pos_1_2 },
			{ pos_2_0, pos_2_1, pos_2_2 }
		};

		for (int i = 0; i < positions.GetLength(0); i++)
		{
			for (int j = 0; j < positions.GetLength(1); j++)
			{
				positions[i, j] += new Vector2(0, padding);
			}
		}
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