using Godot;
using System;

public partial class FormationUI_confirmation : Control
{
	public Button b_Confirm;
	public Button b_Cancel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PanelContainer panelContainer = GetNode<PanelContainer>("PanelContainer");
		HBoxContainer confirmationButtonsHBox = panelContainer.GetNode<HBoxContainer>("HBox");
		b_Confirm = confirmationButtonsHBox.GetNode<Button>("b_Confirm");
		b_Cancel = confirmationButtonsHBox.GetNode<Button>("b_Cancel");
	}
}
