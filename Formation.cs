using Godot;
using System.Collections.Generic;

public partial class Formation : Node2D
{
	private Direction _direction;
	private List<Unit> _units;
	private UnitController _parentController;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		YSortEnabled = true;
		_parentController = GetParent<UnitController>();
		_units = new List<Unit>();
		for (int i = 0; i < 10; i++)
		{
			PackedScene unitScene = (PackedScene)ResourceLoader.Load("res://Units/British_troops_01.tscn");
			Unit unit = (Unit)unitScene.Instantiate();
			_units.Add(unit);
		}
		foreach (Unit unit in _units)
		{
			AddChild(unit);
			unit.PositionUpdated += (oldPosition, newPosition) => _parentController._on_unit_position_updated(unit, oldPosition, newPosition);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		Vector2I left = new Vector2I(0, 0);
		Vector2I right = new Vector2I(0, 0);
		Vector2I front = new Vector2I(0, 0);

		switch (direction)
		{
			case Direction.NORTH:
				left = new Vector2I(-1, 0);
				right = new Vector2I(1, 0);
				front = new Vector2I(0, -1);
				break;
			case Direction.NORTH_EAST:
				left = new Vector2I(-1, -1);
				right = new Vector2I(1, 1);
				front = new Vector2I(1, -1);
				break;
			case Direction.EAST:
				left = new Vector2I(0, -1);
				right = new Vector2I(0, 1);
				front = new Vector2I(1, 0);
				break;
			case Direction.SOUTH_EAST:
				left = new Vector2I(1, -1);
				right = new Vector2I(-1, 1);
				front = new Vector2I(1, 1);
				break;
			case Direction.SOUTH:
				left = new Vector2I(1, 0);
				right = new Vector2I(-1, 0);
				front = new Vector2I(0, 1);
				break;
			case Direction.SOUTH_WEST:
				left = new Vector2I(1, 1);
				right = new Vector2I(-1, -1);
				front = new Vector2I(-1, 1);
				break;
			case Direction.WEST:
				left = new Vector2I(0, 1);
				right = new Vector2I(0, -1);
				front = new Vector2I(-1, 0);
				break;
			case Direction.NORTH_WEST:
				left = new Vector2I(-1, 1);
				right = new Vector2I(1, -1);
				front = new Vector2I(-1, -1);
				break;
		}

		int width = _units.Count;
		Vector2I cellForPlacement = cell + front + left * (width / 2);
		for (int i = 0; i < _units.Count; i++)
		{
			_units[i].MoveToTile(cellForPlacement + right * i, direction);
		}
		_direction = direction;
	}
}
