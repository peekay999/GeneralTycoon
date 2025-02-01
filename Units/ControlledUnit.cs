using Godot;
using System;
using System.Collections.Generic;

public partial class ControlledUnit : Unit
{
	public ActionQueue ActionQueue { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ActionQueue = new ActionQueue(100);
		AddChild(ActionQueue);
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
