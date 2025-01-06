using Godot;
using System;
using System.Collections.Generic;

public partial class Formation : Node2D, IDirectionAnchor
{
	public Direction _direction { get; private set; }
	public LocalisedDirections _localisedDirections { get; private set; }
	private List<Unit> _units;
	private Unit commander;
	private UnitController _parentController;
	private (int hoverCount, bool isHovered) _hoverStatus = (0, false);
	private bool _isSelected = false;

	[Signal]
	public delegate void FormationSelectedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		YSortEnabled = true;
		_parentController = GetParent<UnitController>();
		_units = new List<Unit>();
		for (int i = 0; i < 11; i++)
		{
			PackedScene unitScene = (PackedScene)ResourceLoader.Load("res://Units/British_troops_01.tscn");
			Unit unit = (Unit)unitScene.Instantiate();
			_units.Add(unit);
		}
		foreach (Unit unit in _units)
		{
			AddChild(unit);
			unit.Name = "Unit " + _units.IndexOf(unit);
			unit.MoveAttempted += (currentCell, targetCell) => _parentController._on_unit_move_attempted(unit, currentCell, targetCell);
			unit.WaypointUpdated += (currentCell, targetCell, direction) => _parentController._on_unit_waypoint_updated(unit, currentCell, targetCell, direction);
			unit.MouseEntered += () => _on_mouse_entered();
			unit.MouseExited += () => _on_mouse_exited();
		}

		commander = _units[0];
		_units.RemoveAt(0);

		UpdateDirection(Direction.NORTH);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public override void _Input(InputEvent @event)
	{
		if (_hoverStatus.isHovered == true && _isSelected == false)
		{
			if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
			{
				SelectFormation(this);
				_hoverStatus = (0, false);
			}
		}
	}


	private void _on_mouse_entered()
	{
		_hoverStatus.hoverCount++;
		if (_hoverStatus.hoverCount > 0)
		{
			_hoverStatus.isHovered = true;
			Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		}
	}
	private void _on_mouse_exited()
	{
		_hoverStatus.hoverCount--;
		if (_hoverStatus.hoverCount <= 0)
		{
			_hoverStatus.isHovered = false;
			Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
		}
	}


	private static void SelectFormation(Formation formation)
	{
		formation._isSelected = true;
		formation.EmitSignal("FormationSelected");
	}

	public void PivotRight()
	{
		UpdateDirection(UnitUtil.GetClockwiseDirection(_direction));
		Unit rightMarker = _units[_units.Count - 1];
		Vector2I markerPos = rightMarker.GetCurrentCell();

		int files = 1;
		for (int i = _units.Count - 2; i >= 0; i--)
		{
			Unit unit = _units[i];
			Vector2I target = markerPos + _localisedDirections.left * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + _localisedDirections.back + (_localisedDirections.left * (files / 2));
		commander.SetWaypoint(commanderTarget, _direction);
	}

	public void PivotLeft()
	{
		UpdateDirection(UnitUtil.GetAntiClockwiseDirection(_direction));
		Unit leftMarker = _units[0];
		Vector2I markerPos = leftMarker.GetCurrentCell();

		int files = 1;
		for (int i = 1; i < _units.Count; i++)
		{
			Unit unit = _units[i];
			Vector2I target = markerPos + _localisedDirections.right * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + _localisedDirections.back + (_localisedDirections.right * (files / 2));
		commander.SetWaypoint(commanderTarget, _direction);
	}

	public void RetireLeft()
	{
		UpdateDirection(UnitUtil.GetClockwiseDirection(_direction));
		Unit leftMarker = _units[0];
		Vector2I markerPos = leftMarker.GetCurrentCell();

		int files = 1;
		for (int i = 1; i < _units.Count; i++)
		{
			Unit unit = _units[i];
			Vector2I target = markerPos + _localisedDirections.right * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + _localisedDirections.back + (_localisedDirections.right * (files / 2));
		commander.SetWaypoint(commanderTarget, _direction);
	}

	public void RetireRight()
	{
		UpdateDirection(UnitUtil.GetAntiClockwiseDirection(_direction));
		Unit rightMarker = _units[_units.Count - 1];
		Vector2I markerPos = rightMarker.GetCurrentCell();

		int files = 1;
		for (int i = _units.Count - 2; i >= 0; i--)
		{
			Unit unit = _units[i];
			Vector2I target = markerPos + _localisedDirections.left * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + _localisedDirections.back + (_localisedDirections.left * (files / 2));
		commander.SetWaypoint(commanderTarget, _direction);
	}

	public void Advance()
	{
		Vector2I commanderCell = commander.GetCurrentCell();
		Vector2I targetCell = commanderCell + _localisedDirections.forward * 3;
		SetWaypoint(targetCell, _direction);
	}

	public void Retire()
	{
		Vector2I commanderCell = commander.GetCurrentCell();
		Vector2I targetCell = commanderCell + _localisedDirections.back;
		SetWaypoint(targetCell, Direction.CONTINUE);
	}

	public Vector2I GetCurrentCell()
	{
		return commander.GetCurrentCell();
	}

	public Vector2 GetCurrentPosition()
	{
		return commander.Position;
	}

	public void SetWaypoint(Vector2I cell, Direction direction)
	{
		UpdateDirection(direction);
		commander.SetWaypoint(cell, direction);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].SetWaypoint(targetCells[i], direction);
		}
	}

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		commander.MoveToTile(cell);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].MoveToTile(targetCells[i]);
		}
	}

	public void MoveUnitsOnPath()
	{
		foreach (Unit unit in _units)
		{
			unit.MoveOnPath();
		}
		commander.MoveOnPath();
	}

	public void UpdateDirection(Direction direction)
	{
		_direction = direction;
		_localisedDirections = Pathfinder.GetLocalisedDirections(_direction);
	}

	public Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
	{
		UpdateDirection(direction);
		int width = _units.Count;
		Vector2I cellForPlacement = commanderCell + _localisedDirections.forward + _localisedDirections.left * (width / 2);
		Vector2I[] targetCells = new Vector2I[_units.Count];
		for (int i = 0; i < _units.Count; i++)
		{
			targetCells[i] = cellForPlacement + _localisedDirections.right * i;
		}

		return targetCells;
	}
}
