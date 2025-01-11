using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a unit in the game.
/// </summary>
public partial class Unit : TileMover
{
	private Node2D _Ysort;
	private Node2D _sprites;
	private List<AnimatedSprite2D> _animatedSprite2Ds;
	private Direction _direction;
	// private List<Vector2I> path;
	private UnitType _unitType;
	private Area2D _area2D;
	private ActionQueue _actionQueue;

	private float _moveSpeed = 1.0f;
	public float _skewAmplitude;
	public float _skewPhaseOffset;
	[Signal]
	public delegate void MouseEnteredEventHandler();
	[Signal]
	public delegate void MouseExitedEventHandler();

	public override void _Ready()
	{
		_Ysort = GetNode<Node2D>("YSort");
		_sprites = _Ysort.GetNode<Node2D>("Sprites");
		_direction = Direction.NORTH;
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.MouseShapeEntered += (id) => EmitSignal(SignalName.MouseEntered);
		_area2D.MouseShapeExited += (id) => EmitSignal(SignalName.MouseExited);
		_actionQueue = new ActionQueue(100000);
		AddChild(_actionQueue);
		// _parentController = GetParent<UnitController>();
		_animatedSprite2Ds = new List<AnimatedSprite2D>();
		foreach (Node node in _sprites.GetChildren())
		{
			if (node is AnimatedSprite2D)
			{
				_animatedSprite2Ds.Add((AnimatedSprite2D)node);
			}
		}

		_unitType = UnitType.BLUE_INF;

		RandomNumberGenerator _rng = new RandomNumberGenerator();

		_skewAmplitude = _rng.RandfRange(0.075f, 0.125f);
		_skewPhaseOffset = _rng.RandfRange(0.0f, Mathf.Pi * 0.5f);
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
		return _unitType.AtlasCoords;
	}

	/// <summary>
	/// Adjusts the unit's real transform position and offsets for Y-sorting.
	/// Used by the UnitController to update the unit's position.
	/// For simply moving a unit around the map, use MoveToTile.
	/// </summary>
	/// <param name="cellTo">The target cell to move to.</param>
	/// <param name="tileMapLayer">The tile map layer to use for position calculations.</param>
	public override void UpdatePosition(Vector2I cellTo)
	{
		base.UpdatePosition(cellTo);
		UpdateSpritesYoffset(DetermineSpritesYoffset(cellTo));
		UpdateDirection(Direction.CONTINUE);
	}

	public void UpdateSpritesYoffset(float offset)
	{
		_Ysort.Position = new Vector2(0, offset);
		_sprites.Position = new Vector2(0, -offset);
	}

	public static float DetermineSpritesYoffset(Vector2I cell)
	{
		float offset = 0.0f;
		TileMapLayer tileMapLayer = World.Instance.GetTopLayer(cell);
		offset -= tileMapLayer.Position.Y;
		if (tileMapLayer.GetCellAtlasCoords(cell) != TileMapUtil.tile_base)
		{
			offset += 8;
		}
		offset += 1;
		return offset;
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
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.Frame = (int)direction;
		}
		if (_animatedSprite2Ds.Count > 1)
		{
			Vector2[] positions = UnitUtil.GetSpritePositions(direction);
			for (int i = 0; i < _animatedSprite2Ds.Count; i++)
			{
				if (i > positions.Length - 1)
				{
					break;
				}
				_animatedSprite2Ds[i].Position = positions[i];
			}
		}
		_direction = direction;
	}

	public void SetAnimation(string animation)
	{
		foreach (AnimatedSprite2D sprite in _animatedSprite2Ds)
		{
			sprite.Animation = animation;
		}
	}

	public async void AssignPath(Vector2I cell, Direction direction)
	{
		_actionQueue.ClearQueue();
		List<Vector2I> path = await World.Instance.GetPathfinder().FindPathAsync(currentCell, cell);
		for (int i = 0; i < path.Count - 1; i++)
		{
			MoveAction moveAction = new MoveAction(this, path[i], path[i + 1]);
			_actionQueue.EnqueueAction(moveAction);
		}

		// add in a final turn action
		_actionQueue.EnqueueAction(new TurnAction(this, direction));
	}

	public void ExecuteNextAction()
	{
		_actionQueue.ExecuteQueue();
	}

	public void ResetActionPoints()
	{
		_actionQueue.ResetPoints();
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
}