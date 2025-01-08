using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Formation : Node2D, IDirectionAnchor
{
	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }
	protected FormationController _formationController;
	protected List<Unit> _units;
	protected Unit _commander;
	[Export]
	public int FormationSize { get; set; }
	private PackedScene _commanderScene;
	private PackedScene _ranksScene;

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

	[Signal]
	public delegate void FormationSelectedEventHandler();

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

		UpdateDirection(Direction.NORTH);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Unit GetCommander()
	{
		return _commander;
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
		Vector2I cellForPlacement = commanderCell + LocalisedDirections.forward + LocalisedDirections.left * (width / 2);
		Vector2I[] targetCells = new Vector2I[_units.Count];
		for (int i = 0; i < _units.Count; i++)
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
		_commander.SetWaypoint(cell, direction);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].SetWaypoint(targetCells[i], direction);
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

	public void MoveUnitsOnPath()
	{
		foreach (Unit unit in _units)
		{
			unit.MoveOnPath();
		}
		_commander.MoveOnPath();
	}

	public void UpdateDirection(Direction direction)
	{
		Direction = direction;
		LocalisedDirections = Pathfinder.GetLocalisedDirections(Direction);
	}

	public int GetWidth()
	{
		return _units.Count;
	}
}
