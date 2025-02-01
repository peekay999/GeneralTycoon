using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class Formation : Node2D, IDirectionAnchor
{
	[Export]
	public int FormationSize { get; set; }
	public Direction Direction { get; protected set; }
	public LocalisedDirections LocalisedDirections { get; protected set; }
	protected List<Unit> _subordinates;
	protected List<Unit> _allUnits;
	protected Unit _commander;

	[Export]
	public PackedScene Commander
	{
		get;
		protected set;
	}

	[Export]
	public PackedScene Rankers
	{
		get;
		protected set;
	}

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

	public Vector2I GetCurrentCell()
	{
		return _commander.GetCurrentCell();
	}
	public Vector2 GetCurrentPosition()
	{
		return _commander.Position;
	}

	public abstract Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction);

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
}
