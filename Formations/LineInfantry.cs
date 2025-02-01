using Godot;
using System;

public abstract partial class LineInfantry : ControlledFormation
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public override Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
	{
		UpdateDirection(direction);
		int width = Subordinates.Count;
		Vector2I[] targetCells = new Vector2I[Subordinates.Count];
		Vector2I cellForPlacement = commanderCell + LocalisedDirections.forward + LocalisedDirections.left * (width / 2);
		for (int i = 0; i < width; i++)
		{
			targetCells[i] = cellForPlacement + LocalisedDirections.right * i;
		}

		return targetCells;
	}

	public int GetWidth()
	{
		return Subordinates.Count;
	}
}
