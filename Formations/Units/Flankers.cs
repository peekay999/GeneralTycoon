using Godot;
using System;

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
			Vector2[] positions = new Vector2[2];

			if (LeftOrRight == FlankType.LEFT)
			{
				positions[0] = positionsGrid[2, 0];
				positions[1] = positionsGrid[2, 2];
			}
			else
			{
				positions[0] = positionsGrid[0, 0];
				positions[1] = positionsGrid[0, 2];
			}
			foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
			{
				sprite.Position = positions[_animatedSprite2Ds.IndexOf(sprite)];
			}

		}
	}

	public enum FlankType
	{
		LEFT,
		RIGHT
	}
}
