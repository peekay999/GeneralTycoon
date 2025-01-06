using Godot;
using System;

public partial class FormationUiController : Node2D
{
	private Control _advance;
	private Control _rightWheel;
	private Control _leftWheel;
	private Control _retire;
	private Control _blockLeft;
	private Control _blockRight;
	private Formation _selectedFormation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_advance = GetNode<Control>("Advance");
		_rightWheel = GetNode<Control>("RightWheel");
		_leftWheel = GetNode<Control>("LeftWheel");
		_retire = GetNode<Control>("Retire");
		_blockLeft = GetNode<Control>("BlockLeft");
		_blockRight = GetNode<Control>("BlockRight");

		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
