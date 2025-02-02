using Godot;
using System;
using System.Collections.Generic;

public partial class GhostUnit : Unit
{
	private ControlledUnit _controlledUnit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UnitCount = 1;
		base._Ready();
	}

	public void SetControlledUnit(ControlledUnit controlledUnit)
	{
		_controlledUnit = controlledUnit;
		SetSprite(_controlledUnit.GhostSprite);
	}
}
