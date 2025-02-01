using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class Formation<TUnit> : Node2D, IDirectionAnchor where TUnit : Unit
{
	[Export]
	public int FormationSize { get; set; }
	public Direction Direction { get; protected set; }
	public LocalisedDirections LocalisedDirections { get; protected set; }
	protected List<TUnit> _subordinates;
	protected List<TUnit> _allUnits;
	protected TUnit _commander;

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
		if (Commander == null || Rankers == null)
		{
			GD.PrintErr(Name + " : Commander or Rankers scene is not assigned.");
			return;
		}
		YSortEnabled = true;
		InitialiseUnits();
		UpdateDirection(Direction.NORTH);
	}

	protected virtual void InitialiseUnits()
	{
		_subordinates = new List<TUnit>();
		_allUnits = new List<TUnit>();

		if (Rankers == null || Commander == null)
		{
			GD.PrintErr("Ranks or Commander scene is not assigned.");
			return;
		}

		for (int i = 0; i < FormationSize; i++)
		{
			TUnit unit = (TUnit)Rankers.Instantiate();
			AddChild(unit);
			_subordinates.Add(unit);
			_allUnits.Add(unit);
			unit.Name = "Unit " + _subordinates.IndexOf(unit);
		}
		TUnit commander = (TUnit)Commander.Instantiate();
		AddChild(commander);
		_allUnits.Add(commander);
		commander.Name = "Commander";
		_commander = commander;
	}


	public TUnit GetCommander()
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
