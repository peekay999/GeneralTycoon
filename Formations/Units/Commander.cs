using Godot;
using System.Collections.Generic;

public partial class Commander : ControlledUnit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	protected override void InitialiseSprites()
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
}
