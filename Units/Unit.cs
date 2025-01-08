using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a unit in the game.
/// </summary>
public partial class Unit : TileMover
{
	private Node2D _Ysort;
	private Node2D _sprites;
	private List<AnimatedSprite2D> animatedSprite2Ds;
	private Direction _frameDirection;
	private List<Vector2I> path;
	private UnitType unitType;
	private Area2D _area2D;

	[Signal]
	public delegate void WaypointUpdatedEventHandler(Vector2I currentCell, Vector2I targetCell, Direction direction);

	[Signal]
	public delegate void MouseEnteredEventHandler();
	[Signal]
	public delegate void MouseExitedEventHandler();

	public override void _Ready()
	{
		_Ysort = GetNode<Node2D>("YSort");
		_sprites = _Ysort.GetNode<Node2D>("Sprites");
		_frameDirection = Direction.NORTH;
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.MouseShapeEntered += (id) => EmitSignal(SignalName.MouseEntered);
		_area2D.MouseShapeExited += (id) => EmitSignal(SignalName.MouseExited);
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
	/// Adjusts the unit's real transform position and offsets for Y-sorting.
	/// Used by the UnitController to update the unit's position.
	/// For simply moving a unit around the map, use MoveToTile.
	/// </summary>
	/// <param name="cellTo">The target cell to move to.</param>
	/// <param name="tileMapLayer">The tile map layer to use for position calculations.</param>
	public override void UpdateTransformPosition(Vector2I cellTo, TileMapLayer tileMapLayer)
	{
		base.UpdateTransformPosition(cellTo, tileMapLayer);
		UpdateDirection(UnitUtil.DetermineDirection(currentCell, cellTo));

		_Ysort.Position = Vector2.Zero;
		_sprites.Position = Vector2.Zero;

		_Ysort.MoveLocalY(-tileMapLayer.Position.Y);
		_sprites.MoveLocalY(tileMapLayer.Position.Y);

		if (tileMapLayer.GetCellAtlasCoords(cellTo) != TileMapUtil.tile_base)
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
		if (direction == Direction.CONTINUE)
		{
			return;
		}
		foreach (AnimatedSprite2D sprite in animatedSprite2Ds)
		{
			sprite.Frame = (int)direction;
		}
		_frameDirection = direction;
	}

	public void SetPath(List<Vector2I> path)
	{
		if (path != null && path.Count > 0)
		{
			this.path = path;
			target = path[path.Count - 1];
			// QueueRedraw();
		}
	}

	public void MoveOnPath()
	{
		if (path != null && path.Count > 0)
		{
			Vector2I nextCell = path[0];
			path.RemoveAt(0);
			MoveToTile(nextCell);
			SetWaypoint(target, Direction.CONTINUE);
		}
	}

	public void SetWaypoint(Vector2I targetCell, Direction direction)
	{
		EmitSignal(SignalName.WaypointUpdated, currentCell, targetCell, (int)direction);
	}
}