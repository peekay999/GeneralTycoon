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
	// [Export]
	// public SpriteFrames GhostSprite { get; set; }
	[Export(PropertyHint.Range, "0.5,1.5")]
	protected float _moveSpeed = 1.0f;
	[Export]
	private float _spriteOffset_Y = 0.0f;
	public float _skewAmplitude;
	public float _skewPhaseOffset;
	protected List<AnimatedSprite2D> _animatedSprite2Ds;
	protected UnitType _unitType;
	private Area2D _area2D;
	public ActionQueue ActionQueue { get; private set; }
	[Signal]
	public delegate void MouseEnteredEventHandler();
	[Signal]
	public delegate void MouseExitedEventHandler();
	[Signal]
	public delegate void PathfindingStartedEventHandler();
	[Signal]
	public delegate void PathfindingCompleteEventHandler();
	[Signal]
	public delegate void StartExecutingActionsEventHandler();

	public override void _Ready()
	{
		base._Ready();
		_direction = Direction.NORTH;
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.MouseShapeEntered += (id) => EmitSignal(SignalName.MouseEntered);
		_area2D.MouseShapeExited += (id) => EmitSignal(SignalName.MouseExited);
		ActionQueue = new ActionQueue(100);
		AddChild(ActionQueue);
		
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

		_unitType = UnitType.BLUE_INF;

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

	public Vector2I GetUnitTypeAtlasCoords()
	{
		return _unitType.AtlasCoords;
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

	public async void AssignPath(Vector2I cell, Direction direction)
	{
		EmitSignal(SignalName.PathfindingStarted);
		ActionQueue.ClearQueue();
		List<Vector2I> path = await World.Instance.GetPathfinder().FindPathAsync(currentCell, cell);
		for (int i = 0; i < path.Count - 1; i++)
		{
			MoveAction moveAction = new MoveAction(this, path[i], path[i + 1]);
			ActionQueue.EnqueueAction(moveAction);
		}

		// add in a final turn action
		ActionQueue.EnqueueAction(new TurnAction(this, direction));

		EmitSignal(SignalName.PathfindingComplete);
	}

	public void ExecuteActions()
	{
		EmitSignal(SignalName.StartExecutingActions);
		ActionQueue.ExecuteQueue();
	}

	public void ResetActionPoints()
	{
		ActionQueue.ResetPoints();
	}

	public List<Vector2I> GetTilePath()
	{
		UnitAction[] unitActions = ActionQueue.GetActions();
		List<Vector2I> path = new List<Vector2I>();
		foreach (UnitAction action in unitActions)
		{
			if (action is MoveAction moveAction)
			{
				path.Add(moveAction.GetTargetCell());
			}
		}
		return path;
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