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
	private List<Vector2I> path;
	private UnitType unitType;
	private Vector2I currentCell;
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

		unitType = UnitType.BLUE_INF;
	}

	public Vector2I GetCurrentCell()
	{
		return currentCell;
	}

	public Vector2I GetUnitTypeAtlasCoords()
	{
		return unitType.AtlasCoords;
	}

	public void MoveToTile(Vector2I cell)
	{
		_parentController.UpdateUnitPosition(this, currentCell, cell);
		currentCell = cell;

		// Vector2 worldPosition = tileMapLayer.MapToLocal(cell);
		// Position = worldPosition + tileMapLayer.Position;
		// GD.Print(tileMapLayer.Position.Y);

		// if (tileMapLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
		// {
			
		// 	Position = new Vector2(Position.X, Position.Y + 8);
		// 	UpdateYoffset(tileMapLayer.Position.Y - 8);
		// }
		// else{
		// 	UpdateYoffset(tileMapLayer.Position.Y);
		// }
		// _parentController.UpdateUnitPosition(this, currentCell, cell);
		// currentCell = cell;
	}

	// public void MoveToTile(Vector2I cell, Direction direction)
	// {
	// 	MoveToTile(cell);
	// 	UpdateDirection(direction);
	// }

	public void UpdateRealPosition(Vector2I cell, TileMapLayer tileMapLayer)
	{
		Vector2 worldPosition = tileMapLayer.MapToLocal(cell);
		Position = worldPosition + tileMapLayer.Position;

		_Ysort.MoveLocalY(-tileMapLayer.Position.Y);
		_sprites.MoveLocalY(tileMapLayer.Position.Y);

		if (tileMapLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
		{
			MoveLocalY(8);
			_Ysort.MoveLocalY(-8);
			_sprites.MoveLocalY(8);
		}

		_Ysort.MoveLocalY(-1);
		_sprites.MoveLocalY(1);

		// _Ysort.Position = new Vector2(_Ysort.Position.X, _Ysort.Position.Y - value);
		// _sprites.Position = new Vector2(_sprites.Position.X, _sprites.Position.Y + value);

		// _Ysort.MoveLocalY(-1);
		// _sprites.MoveLocalY(1);
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
