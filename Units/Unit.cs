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
	// private List<Vector2I> path;
	private UnitType unitType;
	private Area2D _area2D;
	private ActionQueue _actionQueue;

	private float _moveSpeed = 1.0f;
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
		_actionQueue = new ActionQueue(100000);
		AddChild(_actionQueue);
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

	public float GetWalkSpeed()
	{
		return _moveSpeed;
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
	public override void UpdateTransformPosition(Vector2I cellTo)
	{
		base.UpdateTransformPosition(cellTo);
		UpdateSpritesYoffset(cellTo);
		UpdateDirection(Direction.CONTINUE);
	}

	public void UpdateSpritesYoffset(Vector2I cell)
	{
		_Ysort.Position = Vector2.Zero;
		_sprites.Position = Vector2.Zero;

		TileMapLayer topLayer = World.Instance.GetTopLayer(cell);

		_Ysort.MoveLocalY(-topLayer.Position.Y);
		_sprites.MoveLocalY(topLayer.Position.Y);

		if (topLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
		{
			MoveLocalY(8);
			_Ysort.MoveLocalY(-8);
			_sprites.MoveLocalY(8);
		}

		_Ysort.MoveLocalY(-1);
		_sprites.MoveLocalY(1);
	}

		public void UpdateSpritesYoffset(Vector2I cell, float Yoffset)
	{
		_Ysort.Position = Vector2.Zero + new Vector2(0, Yoffset);
		_sprites.Position = Vector2.Zero + new Vector2(0, Yoffset);

		if (World.Instance.GetTopLayer(cell).GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
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

	public void SetTilePath(List<Vector2I> path)
	{
		_actionQueue.ClearQueue();
		for (int i = 0; i < path.Count - 1; i++)
		{
			int cost = Pathfinder.GetMovementCost(path[i], path[i + 1]);
			MoveAction moveAction = new MoveAction(cost, this, path[i], path[i + 1]);
			_actionQueue.EnqueueAction(moveAction);
		}
	}

	public void SetTilePath(List<MoveAction> moveActions)
	{
		foreach (MoveAction moveAction in moveActions)
		{
			_actionQueue.EnqueueAction(moveAction);
		}
	}

	public void ExecuteNextAction()
	{
		_actionQueue.ResetPoints();
		_actionQueue.ExecuteQueue();
	}

	public List<Vector2I> GetTilePath()
	{
		List<UnitAction> actions = new List<UnitAction>(_actionQueue.GetActionQueue());
		List<Vector2I> path = new List<Vector2I>();
		foreach (UnitAction action in actions)
		{
			if (action is MoveAction moveAction)
			{
				path.Add(moveAction.GetTargetCell());
			}
		}
		return path;
	}

	public void SetWaypoint(Vector2I targetCell, Direction direction)
	{
		EmitSignal(SignalName.WaypointUpdated, currentCell, targetCell, (int)direction);
	}
}