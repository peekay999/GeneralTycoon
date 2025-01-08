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
		button.MouseEntered += _on_mouse_entered;
		button.MouseExited += _on_mouse_exited;
		button.Pressed += _on_button_pressed;
		button.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		EmitSignal(SignalName.Selected);
	}

	private void _on_mouse_entered()
	{
		GD.Print("Mouse entered button");
		button.Modulate = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	}

	private void _on_mouse_exited()
	{
		GD.Print("Mouse exited button");
		button.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	public void UpdateButtonIcons(CompressedTexture2D normal)
	{
		// button.AddThemeIconOverride("normal", normal);
		button.Icon = normal;
	}
}
