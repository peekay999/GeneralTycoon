using Godot;
using System;

public partial class FormationUI_controls : Control
{
	public Button b_Move;
	public Button b_Run;
	public Button b_Fire;
	public Button b_Charge;
	public Button b_Cancel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PanelContainer panelContainer = GetNode<PanelContainer>("PanelContainer");
		HBoxContainer formationButtonsHBox = panelContainer.GetNode<HBoxContainer>("HBox");
		b_Move = formationButtonsHBox.GetNode<Button>("b_Move");
		b_Run = formationButtonsHBox.GetNode<Button>("b_Run");
		b_Fire = formationButtonsHBox.GetNode<Button>("b_Fire");
		b_Charge = formationButtonsHBox.GetNode<Button>("b_Charge");
		b_Cancel = formationButtonsHBox.GetNode<Button>("b_Cancel");
	}
}
