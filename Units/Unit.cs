using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : Node2D
{
	private Node2D _Ysort;
	private Node2D _sprites;
	private List<AnimatedSprite2D> animatedSprite2Ds;
	private UnitController _parentController;
	private Direction _frameDirection;
	public List<Vector2I> path;
	public override void _Ready()
	{
		_Ysort = GetNode<Node2D>("YSort");
		_sprites = _Ysort.GetNode<Node2D>("Sprites");
		_frameDirection = Direction.NORTH_EAST;
		_parentController = GetParent<UnitController>();
		animatedSprite2Ds = new List<AnimatedSprite2D>();
		foreach (Node node in _sprites.GetChildren())
		{
			if (node is AnimatedSprite2D)
			{
				animatedSprite2Ds.Add((AnimatedSprite2D)node);
			}
		}
	}

	public void MoveToTile(Vector2I cell, TileMapLayer tileMapLayer)
	{
		Vector2 worldPosition = tileMapLayer.MapToLocal(cell);
		Position = worldPosition + tileMapLayer.Position;
		_Ysort.Position = new Vector2(_Ysort.Position.X, tileMapLayer.Position.Y * -1);
		_sprites.Position = new Vector2(_sprites.Position.X, tileMapLayer.Position.Y);

		_Ysort.MoveLocalY(-1);
		_sprites.MoveLocalY(1);

		if (tileMapLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
		{
			Position = new Vector2(Position.X, Position.Y + 8);
		}
	}

	public void MoveToTile(Vector2I cell, TileMapLayer tileMapLayer, Direction direction)
	{
		MoveToTile(cell, tileMapLayer);
		UpdateDirection(direction);
	}

	public void UpdateYoffset(int value)
	{
		_Ysort.Position = new Vector2(_Ysort.Position.X, _Ysort.Position.Y - value);
		_sprites.Position = new Vector2(_sprites.Position.X, _sprites.Position.Y + value);
	}

	public void UpdateDirection(Direction direction)
	{
		foreach (AnimatedSprite2D sprite in animatedSprite2Ds)
		{
			sprite.Frame = (int)direction;
		}
		_frameDirection = direction;
	}
}
