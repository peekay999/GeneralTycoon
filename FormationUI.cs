using Godot;
using System;

public partial class FormationUI : TileMover
{
	Button button;

	[Signal]
	public delegate void SelectedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button = GetNode<Button>("Button");
		button.MouseEntered += () => button.Modulate = new Color(0.7f, 0.7f, 0.7f, 1.0f);
		button.MouseEntered += () => Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
		button.MouseExited += () => button.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		button.MouseExited += () => Input.SetDefaultCursorShape(Input.CursorShape.Arrow);

		button.Pressed += ButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ButtonPressed()
	{
		EmitSignal(SignalName.Selected);
	}

	public void UpdateButtonIcons(CompressedTexture2D normal)
	{
		// button.AddThemeIconOverride("normal", normal);
		button.Icon = normal;
	}
}
