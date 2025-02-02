using Godot;
using System;
using System.Collections.Generic;

public partial class GhostUnit : Unit
{
	private ControlledUnit _controlledUnit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UnitCount = 1;
		base._Ready();
	}

	public void SetControlledUnit(ControlledUnit controlledUnit)
	{
		_controlledUnit = controlledUnit;
		SetSprite(_controlledUnit.GhostSprite);
	}

	protected override void InitialiseSprites()
	{
		_animatedSprite2Ds = new List<AnimatedSprite2D>();
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
