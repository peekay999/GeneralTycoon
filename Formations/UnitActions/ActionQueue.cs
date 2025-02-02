using Godot;
using System;
using System.Collections.Generic;

public partial class ActionQueue : Node
{
	private Queue<UnitAction> _actionQueue = new Queue<UnitAction>();
	private UnitAction _currentAction = null;
	private (float current, float reset) _actionPoints;
	[Signal]
	public delegate void FinishedExecutingEventHandler();

	public void ResetPoints()
	{
		_actionPoints.current = _actionPoints.reset;
	}

	public void ClearQueue()
	{
		_actionQueue.Clear();
	}

	public ActionQueue(int actionPoints)
	{
		_actionPoints = (actionPoints, actionPoints);
	}

	public UnitAction[] GetActions()
	{
		return _actionQueue.Count > 0 ? _actionQueue.ToArray() : null;
	}

	public UnitAction GetLastAction()
	{
		return _actionQueue.Count > 0 ? _actionQueue.ToArray()[_actionQueue.Count - 1] : null;
	}

	public void EnqueueAction(UnitAction action)
	{
		AddChild(action);
		_actionQueue.Enqueue(action);
	}

	public void ExecuteQueue()
	{
		if (_actionQueue.Count > 0 && _actionPoints.current >= _actionQueue.Peek()._cost)
		{
			_currentAction = _actionQueue.Dequeue();
			_currentAction.ActionComplete += OnActionCompleted;
			_currentAction.Execute();
			_actionPoints.current -= _currentAction._cost;
		}
		else
		{
			_currentAction = null;
			EmitSignal(SignalName.FinishedExecuting);
		}
	}

	private void OnActionCompleted()
	{
		if (_currentAction != null)
		{
			_currentAction.ActionComplete -= OnActionCompleted;
			_currentAction = null;
		}
		ExecuteQueue();
	}
}