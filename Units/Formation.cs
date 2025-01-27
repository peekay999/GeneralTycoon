using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Formation : Node2D, IDirectionAnchor
{
	[Export]
	public int FormationSize { get; set; }
	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }
	protected List<Unit> _units;
	protected Unit _commander;
	private int unitsPathfinding = 0;

	[Signal]
	public delegate void PathfindingCompleteEventHandler();

	[Export]
	public PackedScene Commander
	{
		get => Commander;
		set
		{
			if (value.Instantiate() is Unit)
			{
				Commander = value;
			}
			else
			{
				GD.PrintErr("Assigned scene is not of type Unit.");
			}
		}
	}

	[Export]
	public PackedScene Rankers
	{
		get => Rankers;
		set
		{
			if (value.Instantiate() is Unit)
			{
				Rankers = value;
			}
			else
			{
				GD.PrintErr("Assigned scene is not of type Unit.");
			}
		}
	}



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_units = new List<Unit>();

		if (Rankers == null || Commander == null)
		{
			GD.PrintErr("Ranks or Commander scene is not assigned.");
			return;
		}

		for (int i = 0; i < FormationSize; i++)
		{
			Unit unit = (Unit)Rankers.Instantiate();
			AddChild(unit);
			_units.Add(unit);
			unit.Name = "Unit " + _units.IndexOf(unit);
		}
		Unit commander = (Unit)Commander.Instantiate();
		AddChild(commander);
		commander.Name = "Commander";
		_commander = commander;

		foreach (Unit unit in _units)
		{
			unit.PathfindingStarted += () => unitsPathfinding++;
			unit.PathfindingComplete += () => _on_unit_pathfinding_complete();


		}

		UpdateDirection(Direction.NORTH);

	}

	private void _on_unit_pathfinding_complete()
	{
		unitsPathfinding--;
		if (unitsPathfinding <= 0)
		{
			EmitSignal(SignalName.PathfindingComplete);
			unitsPathfinding = 0;
		}
	}


	public Unit GetCommander()
	{
		return _commander;
	}

	public int GetWidth()
	{
		return _units.Count;
	}

	public Vector2I GetCurrentCell()
	{
		return _commander.GetCurrentCell();
	}
	public Vector2 GetCurrentPosition()
	{
		return _commander.Position;
	}

	public abstract Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction);


	public void SetWaypoint(Vector2I cell, Direction direction)
	{
		UpdateDirection(direction);
		_commander.AssignPath(cell, direction);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].AssignPath(targetCells[i], direction);
		}
	}

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		_commander.MoveToTile(cell);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].MoveToTile(targetCells[i]);
			_units[i].UpdateDirection(direction);
		}
		_commander.UpdateDirection(direction);
	}


	public void UpdateDirection(Direction direction)
	{
		Direction = direction;
		LocalisedDirections = Pathfinder.GetLocalisedDirections(Direction);
	}

	public Waypoint GetWaypoint()
	{
		Waypoint waypoint;
		Direction direction = Direction.NORTH; // or any default value
		Vector2I targetCell = new Vector2I(); // or any default value
		List<TurnAction> turnActions = new List<TurnAction>();
		List<MoveAction> moveActions = new List<MoveAction>();
		if (_commander.ActionQueue.GetActions() == null || _commander.ActionQueue.GetActions().Length == 0)
		{
			return null;
		}
		foreach (UnitAction action in _commander.ActionQueue.GetActions())
		{
			if (action is TurnAction turnAction)
			{
				turnActions.Add(turnAction);
			}
			else if (action is MoveAction moveAction)
			{
				moveActions.Add(moveAction);
			}
		}
		if (turnActions.Count > 0)
		{
			direction = turnActions[turnActions.Count - 1].GetDirection();
		}
		if (moveActions.Count > 0)
		{
			targetCell = moveActions[moveActions.Count - 1].GetTargetCell();
		}
		waypoint = new Waypoint(targetCell, direction);
		return waypoint;
	}

	public void ResetActionPoints()
	{
		_commander.ResetActionPoints();
		foreach (Unit unit in _units)
		{
			unit.ResetActionPoints();
		}
	}


}
