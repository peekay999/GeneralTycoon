using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a unit in the game.
/// </summary>
public abstract partial class Unit : TileMover
{
	[Export(PropertyHint.Range, "1,9")]
	public int UnitCount { get; protected set; }
	[Export]
	public SpriteFrames UnitSprite { get; protected set; }

	[Export(PropertyHint.Range, "0.5,1.5")]
	protected float _walkSpeed = 0.8f;
	[Export(PropertyHint.Range, "1.0,2.5")]
	protected float _runSpeed = 1.5f;

	public bool IsRunning { get; set; } = false;
	
	[Export]
	protected float _spriteOffset_Y = 0.0f;
	protected List<AnimatedSprite2D> _animatedSprite2Ds;

	public override void _Ready()
	{
		base._Ready();
		_direction = Direction.NORTH;
		InitialiseSprites();
		UpdateDirection(Direction.NORTH);
	}

	protected virtual void InitialiseSprites()
	{
		_animatedSprite2Ds = new List<AnimatedSprite2D>();
		for (int i = 0; i < UnitCount; i++)
		{
			AnimatedSprite2D sprite = new AnimatedSprite2D();
			sprite.TextureFilter = TextureFilterEnum.Nearest;
			sprite.SpriteFrames = UnitSprite;
			sprite.Offset = new Vector2(0, _spriteOffset_Y);
			sprite.Frame = 0;
			sprite.Position = Vector2.Zero;
			_animatedSprite2Ds.Add(sprite);
			_sprites.AddChild(sprite);
		}
	}

	public void SetSprite(SpriteFrames spriteFrames)
	{
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.SpriteFrames = spriteFrames;
		}
	}

	public Vector2I GetCurrentCell()
	{
		return currentCell;
	}

	public float GetMoveSpeed()
	{
		return IsRunning? _runSpeed : _walkSpeed;
	}

	public string GetMoveAnimation()
	{
		return IsRunning ? Animations.RUN : Animations.WALK;
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