using Godot;
using System;
using System.Collections.Generic;

public partial class Formation : Node2D
{
	private Direction _direction;
	private List<Unit> _units;
	private Unit commander;
	private UnitController _parentController;

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
		int X = 0;
		foreach (Unit unit in _units)
		{
			AddChild(unit);
			unit.Name = "Unit " + X;
			X++;
			unit.MoveAttempted += (unit, targetCell) => _parentController._on_unit_move_attempted(unit, targetCell);
			unit.WaypointUpdated += (currentCell, targetCell, direction) => _parentController._on_unit_waypoint_updated(unit, currentCell, targetCell, direction);
		}

		commander = _units[0];
		_units.RemoveAt(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector2I GetCurrentCell()
	{
		return commander.GetCurrentCell();
	}

	public void SetWaypoint(Vector2I cell, Direction direction)
	{
		commander.SetWaypoint(cell, direction);
		Vector2I[] targetCells = GetPositionTargets(cell, direction);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].SetWaypoint(targetCells[i], direction);
		}
	}

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		commander.MoveToTile(cell);
		Vector2I[] targetCells = GetPositionTargets(cell, direction);
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

	public Vector2I[] GetPositionTargets(Vector2I commanderCell, Direction direction)
	{
		Vector2I left = Vector2I.Zero;
		Vector2I right = Vector2I.Zero;
		Vector2I front = Vector2I.Zero;
		Vector2I back = Vector2I.Zero;

		switch (direction)
		{
			case Direction.NORTH:
				left = new Vector2I(-1, 0);
				right = new Vector2I(1, 0);
				front = new Vector2I(0, -1);
				back = new Vector2I(0, 1);
				break;
			case Direction.NORTH_EAST:
				left = new Vector2I(-1, -1);
				right = new Vector2I(1, 1);
				front = new Vector2I(1, -1);
				back = new Vector2I(-1, 1);
				break;
			case Direction.EAST:
				left = new Vector2I(0, -1);
				right = new Vector2I(0, 1);
				front = new Vector2I(1, 0);
				back = new Vector2I(-1, 0);
				break;
			case Direction.SOUTH_EAST:
				left = new Vector2I(1, -1);
				right = new Vector2I(-1, 1);
				front = new Vector2I(1, 1);
				back = new Vector2I(-1, -1);
				break;
			case Direction.SOUTH:
				left = new Vector2I(1, 0);
				right = new Vector2I(-1, 0);
				front = new Vector2I(0, 1);
				back = new Vector2I(0, -1);
				break;
			case Direction.SOUTH_WEST:
				left = new Vector2I(1, 1);
				right = new Vector2I(-1, -1);
				front = new Vector2I(-1, 1);
				back = new Vector2I(1, -1);
				break;
			case Direction.WEST:
				left = new Vector2I(0, 1);
				right = new Vector2I(0, -1);
				front = new Vector2I(-1, 0);
				back = new Vector2I(1, 0);
				break;
			case Direction.NORTH_WEST:
				left = new Vector2I(-1, 1);
				right = new Vector2I(1, -1);
				front = new Vector2I(-1, -1);
				back = new Vector2I(1, 1);
				break;
		}
		int width = _units.Count;
		Vector2I cellForPlacement = commanderCell + front + left * (width / 2);
		Vector2I[] targetCells = new Vector2I[_units.Count];
		for (int i = 0; i < _units.Count; i++)
		{
			targetCells[i] = cellForPlacement + right * i;
			// _units[i].MoveToTile(cellForPlacement + right * i, direction);
		}

		return targetCells;
	}
}
