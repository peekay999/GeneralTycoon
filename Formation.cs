using Godot;
using System;
using System.Collections.Generic;

public partial class Formation : Node2D
{
	private Direction _direction;
	private List<Unit> _units;
	private Unit commander;
	private UnitController _parentController;
	private (int hoverCount, bool isHovered) _hoverStatus = (0, false);
	private bool _isSelected = false;

	private Vector2I left = Vector2I.Zero;
	private Vector2I right = Vector2I.Zero;
	private Vector2I front = Vector2I.Zero;
	private Vector2I back = Vector2I.Zero;

	private Vector2I front_left = Vector2I.Zero;
	private Vector2I front_right = Vector2I.Zero;
	private Vector2I back_left = Vector2I.Zero;
	private Vector2I back_right = Vector2I.Zero;


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
			unit.MouseEntered += () => _on_mouse_entered(unit);
			unit.MouseExited += () => _on_mouse_exited(unit);
		}

		commander = _units[0];
		_units.RemoveAt(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public override void _Input(InputEvent @event)
	{
		if (_hoverStatus.isHovered == true)
		{
			if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
			{
				SelectUnit();
			}
		}
	}


	private void _on_mouse_entered(Unit unit)
	{
		_hoverStatus.hoverCount++;
		if (_hoverStatus.hoverCount > 0)
		{
			_hoverStatus.isHovered = true;
			Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		}
	}
	private void _on_mouse_exited(Unit unit)
	{
		_hoverStatus.hoverCount--;
		if (_hoverStatus.hoverCount <= 0)
		{
			_hoverStatus.isHovered = false;
			Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
		}
	}


	private void SelectUnit()
	{
		_isSelected = true;
		GD.Print("Formation " + Name + " selected");
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
			Vector2I target = markerPos + left * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + back + (left * (files / 2));
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
			Vector2I target = markerPos + right * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + back + (right * (files / 2));
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
			Vector2I target = markerPos + right * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + back + (right * (files / 2));
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
			Vector2I target = markerPos + left * files;
			unit.SetWaypoint(target, _direction);
			files++;
		}

		Vector2I commanderTarget = markerPos + back + (left * (files / 2));
		commander.SetWaypoint(commanderTarget, _direction);
	}

	public void Advance()
	{
		Vector2I commanderCell = commander.GetCurrentCell();
		Vector2I targetCell = commanderCell + front * 3;
		SetWaypoint(targetCell, _direction);
	}

	public void Retire()
	{
		Vector2I commanderCell = commander.GetCurrentCell();
		Vector2I targetCell = commanderCell + back;
		SetWaypoint(targetCell, Direction.CONTINUE);
	}

	public Vector2I GetCurrentCell()
	{
		return commander.GetCurrentCell();
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

	private void UpdateDirection(Direction direction)
	{
		_direction = direction;
		SetDirectionBearings(direction);
	}

	public Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
	{
		UpdateDirection(direction);
		int width = _units.Count;
		Vector2I cellForPlacement = commanderCell + front + left * (width / 2);
		Vector2I[] targetCells = new Vector2I[_units.Count];
		for (int i = 0; i < _units.Count; i++)
		{
			targetCells[i] = cellForPlacement + right * i;
		}

		return targetCells;
	}

	private void SetDirectionBearings(Direction direction)
	{
		switch (direction)
		{
			case Direction.NORTH:
				left = new Vector2I(-1, 0);
				right = new Vector2I(1, 0);
				front = new Vector2I(0, -1);
				back = new Vector2I(0, 1);
				front_left = new Vector2I(-1, -1);
				front_right = new Vector2I(1, -1);
				back_left = new Vector2I(-1, 1);
				back_right = new Vector2I(1, 1);

				break;
			case Direction.NORTH_EAST:
				left = new Vector2I(-1, -1);
				right = new Vector2I(1, 1);
				front = new Vector2I(1, -1);
				back = new Vector2I(-1, 1);
				front_left = new Vector2I(0, -1);
				front_right = new Vector2I(1, 0);
				back_left = new Vector2I(-1, 0);
				back_right = new Vector2I(0, 1);

				break;
			case Direction.EAST:
				left = new Vector2I(0, -1);
				right = new Vector2I(0, 1);
				front = new Vector2I(1, 0);
				back = new Vector2I(-1, 0);
				front_left = new Vector2I(-1, 1);
				front_right = new Vector2I(1, 1);
				back_left = new Vector2I(-1, -1);
				back_right = new Vector2I(-1, 1);

				break;
			case Direction.SOUTH_EAST:
				left = new Vector2I(1, -1);
				right = new Vector2I(-1, 1);
				front = new Vector2I(1, 1);
				back = new Vector2I(-1, -1);
				front_left = new Vector2I(1, 0);
				front_right = new Vector2I(0, 1);
				back_left = new Vector2I(0, -1);
				back_right = new Vector2I(-1, 0);

				break;
			case Direction.SOUTH:
				left = new Vector2I(1, 0);
				right = new Vector2I(-1, 0);
				front = new Vector2I(0, 1);
				back = new Vector2I(0, -1);
				front_left = new Vector2I(1, 1);
				front_right = new Vector2I(-1, 1);
				back_left = new Vector2I(1, -1);
				back_right = new Vector2I(-1, -1);
				break;
			case Direction.SOUTH_WEST:
				left = new Vector2I(1, 1);
				right = new Vector2I(-1, -1);
				front = new Vector2I(-1, 1);
				back = new Vector2I(1, -1);
				front_left = new Vector2I(0, 1);
				front_right = new Vector2I(-1, 0);
				back_left = new Vector2I(1, 0);
				back_right = new Vector2I(0, -1);
				break;
			case Direction.WEST:
				left = new Vector2I(0, 1);
				right = new Vector2I(0, -1);
				front = new Vector2I(-1, 0);
				back = new Vector2I(1, 0);
				front_left = new Vector2I(1, 1);
				front_right = new Vector2I(-1, -1);
				back_left = new Vector2I(1, 1);
				back_right = new Vector2I(1, -1);
				break;
			case Direction.NORTH_WEST:
				left = new Vector2I(-1, 1);
				right = new Vector2I(1, -1);
				front = new Vector2I(-1, -1);
				back = new Vector2I(1, 1);
				front_left = new Vector2I(-1, 0);
				front_right = new Vector2I(0, -1);
				back_left = new Vector2I(0, 1);
				back_right = new Vector2I(1, 0);
				break;
		}
	}

}
