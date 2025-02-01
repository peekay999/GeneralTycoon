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
	public List<TUnit> Subordinates {get; protected set;}
	public List<TUnit> AllUnits {get; protected set;}
	public TUnit Commander {get; protected set;}

	public override void _Ready()
	{
		Subordinates = new List<TUnit>();
		AllUnits = new List<TUnit>();
		YSortEnabled = true;
		InitialiseUnits();
		UpdateDirection(Direction.NORTH);
	}

	protected abstract void InitialiseUnits();


	public TUnit GetCommander()
	{
		return Commander;
	}

	public Vector2I GetCurrentCell()
	{
		return Commander.GetCurrentCell();
	}
	public Vector2 GetCurrentPosition()
	{
		return Commander.Position;
	}

	public abstract Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction);

	public void MoveToTile(Vector2I cell, Direction direction)
	{
		Commander.MoveToTile(cell);
		Vector2I[] targetCells = DressOffCommander(cell, direction);
		for (int i = 0; i < Subordinates.Count; i++)
		{
			Subordinates[i].MoveToTile(targetCells[i]);
			Subordinates[i].UpdateDirection(direction);
		}
		Commander.UpdateDirection(direction);
	}


	public void UpdateDirection(Direction direction)
	{
		Direction = direction;
		LocalisedDirections = Pathfinder.GetLocalisedDirections(Direction);
	}
}
