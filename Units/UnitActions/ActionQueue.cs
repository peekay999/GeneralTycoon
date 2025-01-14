using Godot;
using System;
using System.Collections.Generic;

public partial class ActionQueue : Node
{
	private Queue<UnitAction> _actionQueue = new Queue<UnitAction>();
	private UnitAction _currentAction = null;
	private (int current, int reset) _actionPoints;

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

	public Queue<UnitAction> GetActionQueue()
	{
		return _actionQueue;
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
			_actionPoints.current -= _currentAction._cost;
			_currentAction.Execute();
		}
		else
		{
			_currentAction = null;
		}
	}

	private void OnActionCompleted()
	{
		_currentAction.ActionComplete -= OnActionCompleted;
		_currentAction = null;
		ExecuteQueue();
	}
}