using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a unit in the game.
/// </summary>
public partial class Unit : Node2D
{
	private Node2D _Ysort;
	private Node2D _sprites;
	private List<AnimatedSprite2D> animatedSprite2Ds;
	// private UnitController _parentController;
	private Direction _frameDirection;
	private List<Vector2I> path;
	private UnitType unitType;
	private Vector2I currentCell;

	[Signal]
	public delegate void PositionUpdatedEventHandler(Vector2I oldCell, Vector2I newCell);

	public override void _Ready()
	{
		_Ysort = GetNode<Node2D>("YSort");
		_sprites = _Ysort.GetNode<Node2D>("Sprites");
		_frameDirection = Direction.NORTH_EAST;
		// _parentController = GetParent<UnitController>();
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

	/// <summary>
	/// Gets the current cell position of the unit.
	/// </summary>
	/// <returns>The current cell position as a Vector2I.</returns>
	public Vector2I GetCurrentCell()
	{
		return currentCell;
	}

	/// <summary>
	/// Gets the atlas coordinates of the unit type.
	/// </summary>
	/// <returns>The atlas coordinates as a Vector2I.</returns>
	public Vector2I GetUnitTypeAtlasCoords()
	{
		return unitType.AtlasCoords;
	}

	/// <summary>
	/// Moves the unit to a specific cell. Handles real position and tracking via the parent controller.
	/// </summary>
	/// <param name="cell">The target cell to move to.</param>
	public void MoveToTile(Vector2I cell)
	{
		// EmitSignal(nameof(PositionUpdated), currentCell, cell);
		EmitSignal(SignalName.PositionUpdated, currentCell, cell);
		currentCell = cell;
	}

	/// <summary>
	/// Moves the unit to a specific cell and updates its direction. Handles real position and tracking via the parent controller.
	/// </summary>
	/// <param name="cell">The target cell to move to.</param>
	/// <param name="direction">The direction to face after moving.</param>
	public void MoveToTile(Vector2I cell, Direction direction)
	{
		MoveToTile(cell);
		UpdateDirection(direction);
	}

	/// <summary>
	/// Adjusts the unit's real transform position and offsets for Y-sorting.
	/// Used by the UnitController to update the unit's position.
	/// For simply moving a unit around the map, use MoveToTile.
	/// </summary>
	/// <param name="cell">The target cell to move to.</param>
	/// <param name="tileMapLayer">The tile map layer to use for position calculations.</param>
	public void UpdateRealPosition(Vector2I cell, TileMapLayer tileMapLayer)
	{
		Vector2 worldPosition = tileMapLayer.MapToLocal(cell);
		_Ysort.Position = Vector2.Zero;
		_sprites.Position = Vector2.Zero;
		Position = worldPosition + tileMapLayer.Position;
		GD.Print("worldPosition: " + worldPosition);
		GD.Print("tileMapLayer.Position: " + tileMapLayer.Position);

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
	}

	/// <summary>
	/// Updates the direction the unit is facing.
	/// </summary>
	/// <param name="direction">The new direction to face.</param>
	public void UpdateDirection(Direction direction)
	{
		foreach (AnimatedSprite2D sprite in animatedSprite2Ds)
		{
			sprite.Frame = (int)direction;
		}
		_frameDirection = direction;
	}
}
