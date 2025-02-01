using Godot;
using System;
using System.Collections.Generic;

public partial class ControlledUnit : Unit
{
	public ActionQueue ActionQueue { get; private set; }
	private Area2D _area2D;
	public float SkewAmplitude;
	public float SkewPhaseOffset;

	[Export]
	public SpriteFrames GhostSprite { get; protected set; }
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
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.MouseShapeEntered += (id) => EmitSignal(SignalName.MouseEntered);
		_area2D.MouseShapeExited += (id) => EmitSignal(SignalName.MouseExited);
		ActionQueue = new ActionQueue(100);
		AddChild(ActionQueue);

		RandomNumberGenerator _rng = new RandomNumberGenerator();

		SkewAmplitude = _rng.RandfRange(0.075f, 0.125f);
		SkewPhaseOffset = _rng.RandfRange(0.0f, Mathf.Pi * 0.5f);
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

	public void ExecuteActions()
	{
		EmitSignal(SignalName.StartExecutingActions);
		ActionQueue.ExecuteQueue();
	}

	public void ResetActionPoints()
	{
		ActionQueue.ResetPoints();
	}
}
