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
	private PackedScene _commanderScene;
	private PackedScene _ranksScene;
	private int unitsPathfinding = 0;
	private int unitsExecutingActions = 0;

	[Signal]
	public delegate void PathfindingCompleteEventHandler();
	[Signal]
	public delegate void StartExecutingActionsEventHandler();
	[Signal]
	public delegate void AllPointsExpendedEventHandler();
	[Signal]
	public delegate void FormationSelectedEventHandler();

	[Export]
	public PackedScene Commander
	{
		get => _commanderScene;
		set
		{
			if (value.Instantiate() is Unit)
			{
				_commanderScene = value;
			}
			else
			{
				GD.PrintErr("Assigned scene is not of type Unit.");
			}
		}
	}

	[Export]
	public PackedScene Ranks
	{
		get => _ranksScene;
		set
		{
			if (value.Instantiate() is Unit)
			{
				_ranksScene = value;
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

		if (_ranksScene == null || _commanderScene == null)
		{
			GD.PrintErr("Ranks or Commander scene is not assigned.");
			return;
		}

		for (int i = 0; i < FormationSize; i++)
		{
			Unit unit = (Unit)Ranks.Instantiate();
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
			unit.StartExecutingActions += () => unitsExecutingActions++;
			unit.ActionQueue.FinishedExecuting += () => EmitSignal(SignalName.AllPointsExpended);
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

	private void _on_unit_expend_all_points()
	{
		unitsExecutingActions--;
		if (unitsExecutingActions <= 0)
		{
			EmitSignal(SignalName.AllPointsExpended);
			unitsExecutingActions = 0;
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

	private static void SelectFormation(Formation formation)
	{
		formation.EmitSignal("FormationSelected");
	}

	public Vector2I GetCurrentCell()
	{
		return _commander.GetCurrentCell();
	}

	public Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
	{
		UpdateDirection(direction);
		int width = _units.Count;
		Vector2I[] targetCells = new Vector2I[_units.Count];
		Vector2I cellForPlacement = commanderCell + LocalisedDirections.forward + LocalisedDirections.left * (width / 2);
		for (int i = 0; i < width; i++)
		{
			targetCells[i] = cellForPlacement + LocalisedDirections.right * i;
		}

		return targetCells;
	}

	public Vector2I GetLeftMarkerCell()
	{
		return _commander.GetCurrentCell() + LocalisedDirections.forward + LocalisedDirections.left * (GetWidth() / 2);
	}

	public Vector2I GetRightMarkerCell()
	{
		return GetLeftMarkerCell() + LocalisedDirections.right * (GetWidth() - 1);
	}

	public Vector2 GetCurrentPosition()
	{
		return _commander.Position;
	}

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

	public void ExecuteAllUnits()
	{
		_commander.ExecuteActions();
		foreach (Unit unit in _units)
		{
			unit.ExecuteActions();
		}
		EmitSignal(SignalName.StartExecutingActions);
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
