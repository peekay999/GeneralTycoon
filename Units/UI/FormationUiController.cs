
using Godot;
using System.Collections.Generic;

public partial class FormationUiController : Node2D
{
	private CanvasLayer _canvasLayer;
	private Control UIcontrol;
	private Button b_Walk;
	private Button b_Run;
	private Button b_Fire;
	private Button b_Charge;
	private Button b_Cancel;
	private HBoxContainer _formationButtonsHBox;
	private Button testButton;
	private Camera2D _worldCamera;
	private ControlledFormation _selectedFormation;
	private FormationController _parentController;
	private SelectionLayer _selectionLayer;
	private GhostFormation _grabbedGhostFormation;
	private Direction _ghostDirection;

	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ZIndex = 1;
		_parentController = GetParent<FormationController>();
		_selectionLayer = World.Instance.GetSelectionLayer();

		_worldCamera = World.Instance.GetNode<Camera2D>("Camera2D");

		_canvasLayer = GetNode<CanvasLayer>("FormationUI");
		UIcontrol = _canvasLayer.GetNode<Control>("UIcontrol");
		_formationButtonsHBox = UIcontrol.GetNode<HBoxContainer>("FormationButtonsHBox");

		b_Walk = _formationButtonsHBox.GetNode<Button>("b_Walk");
		b_Walk.Pressed += _on_walk_pressed;
		b_Run = _formationButtonsHBox.GetNode<Button>("b_Run");
		b_Fire = _formationButtonsHBox.GetNode<Button>("b_Fire");
		b_Charge = _formationButtonsHBox.GetNode<Button>("b_Charge");
		b_Cancel = _formationButtonsHBox.GetNode<Button>("b_Cancel");

		_canvasLayer.Visible = false;

		Direction = Direction.CONTINUE;

		Visible = false;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		AdjustUIPosition();
		if (_grabbedGhostFormation != null && _grabbedGhostFormation.isGrabbed) 
		{
			_grabbedGhostFormation.MoveToTile(_selectionLayer.GetSelectedCell(), _ghostDirection);
			_grabbedGhostFormation.QueueRedraw();
		}
	}

	private void _on_walk_pressed()
	{
		if (_selectedFormation != null)
		{
			_grabbedGhostFormation = _selectedFormation.GhostFormation.Grab();
		}
	}

	private void AdjustUIPosition()
	{
		if (_selectedFormation == null)
		{
			return;
		}
		// Adjust position to account for camera's zoom level
		Vector2 positionDifference = _selectedFormation.GetCurrentPosition() - _worldCamera.Position;
		Vector2 adjustedPosition = positionDifference * _worldCamera.Zoom;

		// Get viewport size and adjust position
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		adjustedPosition += viewportSize / 2;

		adjustedPosition -= new Vector2(_formationButtonsHBox.Size.X / 2, -50 * _worldCamera.Zoom.Y);

		UIcontrol.Position = adjustedPosition;
	}

	public void SetSelectedFormation(ControlledFormation formation)
	{
		_selectionLayer.Visible = false;
		_selectedFormation = formation;
		Visible = true;
		_canvasLayer.Visible = true;
	}

	public void ClearSelectedFormation()
	{
		_selectedFormation = null;
		_selectionLayer.Visible = true;
		_canvasLayer.Visible = false;
		Visible = false;
	}
	public ControlledFormation GetSelectedFormation()
	{
		return _selectedFormation;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (_grabbedGhostFormation != null && _grabbedGhostFormation.isGrabbed)
			{
				if (mouseButton.ButtonIndex == MouseButton.Left)
				{
					_grabbedGhostFormation.Place(_selectionLayer.GetSelectedCell(), _ghostDirection);
					_grabbedGhostFormation = null;
				}
			}
		}
		if (@event is InputEventKey key && key.Pressed)
		{
			if (key.Keycode == Key.Escape)
			{
				ClearSelectedFormation();
			}

			if (_grabbedGhostFormation != null)
			{
				if (key.Keycode == Key.Q)
				{
					_ghostDirection = UnitUtil.GetAntiClockwiseDirection(_ghostDirection);
				}
				else if (key.Keycode == Key.E)
				{
					_ghostDirection = UnitUtil.GetClockwiseDirection(_ghostDirection);
				}
				else if (key.Keycode == Key.Escape)
				{
					_selectedFormation.GhostFormation.Release();
					_grabbedGhostFormation = null;
				}
			}
		}
	}
}