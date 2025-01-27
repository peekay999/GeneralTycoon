using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class Formation : Node2D, IDirectionAnchor
{
	[Export]
	public int FormationSize { get; set; }
	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }
	protected List<Unit> _subordinates;
	protected List<Unit> _allUnits;
	protected Unit _commander;

	private PackedScene _commanderScene;

	private PackedScene _rankersScene;

	[Export]
	public PackedScene Commander
	{
		get => _commanderScene;
		protected set
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
	public PackedScene Rankers
	{
		get => _rankersScene;
		protected set
		{
			if (value.Instantiate() is Unit)
			{
				_rankersScene = value;
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
		YSortEnabled = true;
		InitialiseUnits();
		UpdateDirection(Direction.NORTH);
	}

	protected virtual void InitialiseUnits()
	{
		_subordinates = new List<Unit>();
		_allUnits = new List<Unit>();

		if (Rankers == null || Commander == null)
		{
			GD.PrintErr("Ranks or Commander scene is not assigned.");
			return;
		}

		for (int i = 0; i < FormationSize; i++)
		{
			Unit unit = (Unit)Rankers.Instantiate();
			AddChild(unit);
			_subordinates.Add(unit);
			_allUnits.Add(unit);
			unit.Name = "Unit " + _subordinates.IndexOf(unit);
		}
		Unit commander = (Unit)Commander.Instantiate();
		AddChild(commander);
		_allUnits.Add(commander);
		commander.Name = "Commander";
		_commander = commander;
	}


	public Unit GetCommander()
	{
		return _commander;
	}

	public int GetWidth()
	{
		return _subordinates.Count;
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


	public void SetWaypoint(Waypoint waypoint)
	{
		UpdateDirection(waypoint.Direction);
		_commander.AssignPath(waypoint.Cell, waypoint.Direction);
		Vector2I[] targetCells = DressOffCommander(waypoint.Cell, waypoint.Direction);
		for (int i = 0; i < _subordinates.Count; i++)
		{
			_subordinates[i].AssignPath(targetCells[i], waypoint.Direction);
		}
	}

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		_commander.MoveToTile(cell);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _subordinates.Count; i++)
		{
			_subordinates[i].MoveToTile(targetCells[i]);
			_subordinates[i].UpdateDirection(direction);
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
		var actions = _commander.ActionQueue.GetActions();
		if (actions == null || actions.Length == 0)
		{
			return null;
		}

		TurnAction turnAction = actions.OfType<TurnAction>().LastOrDefault();
		MoveAction moveAction = actions.OfType<MoveAction>().LastOrDefault();

		Direction direction = turnAction?.GetDirection() ?? Direction.CONTINUE;
		Vector2I targetCell = moveAction?.GetTargetCell() ?? Vector2I.Zero;

		return new Waypoint(targetCell, direction);
	}

	public void ResetActionPoints()
	{
		_commander.ResetActionPoints();
		foreach (Unit unit in _subordinates)
		{
			unit.ResetActionPoints();
		}
	}


}
