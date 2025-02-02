using Godot;
using System;
using System.Collections.Generic;

public partial class Flankers : ControlledUnit
{
	[Export]
	public SpriteFrames SecondSpriteFrames { get; protected set; }
	[Export]
	protected float _SecondspriteOffset_Y = 0.0f;

	public FlankType LeftOrRight { get; set; }

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void InitialiseSprites()
	{
		base.InitialiseSprites();

		// modify half the sprites in the list
		int half = _animatedSprite2Ds.Count / 2;
		for (int i = half; i < _animatedSprite2Ds.Count; i++)
		{
			_animatedSprite2Ds[i].SpriteFrames = SecondSpriteFrames;
			_animatedSprite2Ds[i].Offset = new Vector2(0, _spriteOffset_Y);
		}
	}

	public override void UpdateDirection(Direction direction)
	{
		base.UpdateDirection(direction);
		if (_animatedSprite2Ds.Count > 1)
		{
			Vector2[,] positionsGrid = UnitUtil.GetSpritePositions(direction);

            var leftPositions = new Dictionary<Direction, Vector2[]>
		{
			{ Direction.NORTH, new[] { positionsGrid[2, 0], positionsGrid[2, 2] } },
			{ Direction.NORTH_EAST, new[] { positionsGrid[2, 2], positionsGrid[0, 2] } },
			{ Direction.EAST, new[] { positionsGrid[2, 2], positionsGrid[0, 2] } },
			{ Direction.SOUTH_EAST, new[] { positionsGrid[0, 2], positionsGrid[0, 0] } },
			{ Direction.SOUTH, new[] { positionsGrid[0, 2], positionsGrid[0, 0] } },
			{ Direction.SOUTH_WEST, new[] { positionsGrid[0, 0], positionsGrid[2, 0] } },
			{ Direction.WEST, new[] { positionsGrid[0, 0], positionsGrid[2, 0] } },
			{ Direction.NORTH_WEST, new[] { positionsGrid[2, 0], positionsGrid[2, 2] } }
		};

			var rightPositions = new Dictionary<Direction, Vector2[]>
		{
			{ Direction.NORTH, new[] { positionsGrid[0, 0], positionsGrid[0, 2] } },
			{ Direction.NORTH_EAST, new[] { positionsGrid[2, 0], positionsGrid[0, 0] } },
			{ Direction.EAST, new[] { positionsGrid[2, 0], positionsGrid[0, 0] } },
			{ Direction.SOUTH_EAST, new[] { positionsGrid[2, 2], positionsGrid[2, 0] } },
			{ Direction.SOUTH, new[] { positionsGrid[2, 2], positionsGrid[2, 0] } },
			{ Direction.SOUTH_WEST, new[] { positionsGrid[0, 2], positionsGrid[2, 2] } },
			{ Direction.WEST, new[] { positionsGrid[0, 2], positionsGrid[2, 2] } },
			{ Direction.NORTH_WEST, new[] { positionsGrid[0, 0], positionsGrid[0, 2] } }
		};

            Vector2[] positions = LeftOrRight == FlankType.LEFT ? leftPositions[direction] : rightPositions[direction];

            for (int i = 0; i < _animatedSprite2Ds.Count; i++)
			{
				_animatedSprite2Ds[i].Position = positions[i % 2];
			}
		}
	}

	public enum FlankType
	{
		LEFT,
		RIGHT
	}
}
