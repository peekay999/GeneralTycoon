using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a unit in the game.
/// </summary>
public partial class Unit : TileMover
{
	[Export(PropertyHint.Range, "1,9")]
	protected int unitCount = 1;
	[Export]
	private SpriteFrames unitSprite;
	[Export(PropertyHint.Range, "0.5,1.5")]
	protected float _moveSpeed = 1.0f;
	[Export]
	private float _spriteOffset_Y = 0.0f;
	public float _skewAmplitude;
	public float _skewPhaseOffset;
	protected List<AnimatedSprite2D> _animatedSprite2Ds;

	public override void _Ready()
	{
		base._Ready();
		_direction = Direction.NORTH;
		
		_animatedSprite2Ds = new List<AnimatedSprite2D>();
		for (int i = 0; i < unitCount; i++)
		{
			AnimatedSprite2D sprite = new AnimatedSprite2D();
			sprite.TextureFilter = TextureFilterEnum.Nearest;
			sprite.SpriteFrames = unitSprite;
			sprite.Offset = new Vector2(0, _spriteOffset_Y);
			sprite.Frame = 0;
			sprite.Position = Vector2.Zero;
			_animatedSprite2Ds.Add(sprite);
			_sprites.AddChild(sprite);
		}
		UpdateDirection(Direction.NORTH);

		RandomNumberGenerator _rng = new RandomNumberGenerator();

		_skewAmplitude = _rng.RandfRange(0.075f, 0.125f);
		_skewPhaseOffset = _rng.RandfRange(0.0f, Mathf.Pi * 0.5f);
	}

	public Vector2I GetCurrentCell()
	{
		return currentCell;
	}

	public float GetWalkSpeed()
	{
		return _moveSpeed;
	}

	public override void UpdateDirection(Direction direction)
	{
		base.UpdateDirection(direction);
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.Frame = (int)direction;
		}
	}

	public void SetAnimation(string animation)
	{
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.Animation = animation;
		}
	}

	public override void MoveToTile(Vector2I cellTo)
	{
		base.MoveToTile(cellTo);
		if (World.Instance.GetTopLayer(cellTo) == null)
		{
			return;
		}
		float rotation = TileMapUtil.GetTileRotationAmount(World.Instance.GetTopLayer(cellTo).GetCellAtlasCoords(cellTo));
		RotationDegrees = rotation;
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.RotationDegrees = -rotation;
		}
	}

	public override void LerpToTile(Vector2I cellFrom, Vector2I cellTo, float T)
	{
		base.LerpToTile(cellFrom, cellTo, T);
		float currentRotation = RotationDegrees;
		float targetRotation = TileMapUtil.GetTileRotationAmount(World.Instance.GetTopLayer(cellTo).GetCellAtlasCoords(cellTo));
		float rotation_T = Mathf.Lerp(currentRotation, targetRotation, T);
		RotationDegrees = rotation_T;
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.RotationDegrees = -rotation_T;
		}
	}
}