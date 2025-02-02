using Godot;
using System;

public abstract partial class LineInfantry : ControlledFormation
{
	public Flankers LeftFlanker { get; protected set; }
	public Flankers RightFlanker { get; protected set; }
	[Export]
	public PackedScene FlankerScene
	{
		get;
		protected set;
	}
	public override void _Ready()
	{
		base._Ready();
	}

	protected override void InitialiseUnits()
	{
		if (FlankerScene == null)
		{
			GD.PrintErr(Name + ": Flanker scene is not assigned.");
			return;
		}
		Flankers leftFlanker = (Flankers)FlankerScene.Instantiate();
		leftFlanker.LeftOrRight = Flankers.FlankType.LEFT;
		Flankers rightFlanker = (Flankers)FlankerScene.Instantiate();
		rightFlanker.LeftOrRight = Flankers.FlankType.RIGHT;
		leftFlanker.Name = "Left Flanker";
		rightFlanker.Name = "Right Flanker";
		AllUnits.Add(leftFlanker);
		AllUnits.Add(rightFlanker);
		LeftFlanker = leftFlanker;
		RightFlanker = rightFlanker;
		AddChild(leftFlanker);
		AddChild(rightFlanker);

		base.InitialiseUnits();

		Subordinates.Insert(0, leftFlanker);
		Subordinates.Add(rightFlanker);

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
