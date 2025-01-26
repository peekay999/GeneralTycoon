
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class FormationUIcontroller : Node2D
{
	private CanvasLayer _canvasLayer;
	private FormationUI_controls ui_controls;
	private FormationUI_confirmation ui_confirmation;
	private Button testButton;
	private Camera2D _worldCamera;
	private ControlledFormation _selectedFormation;
	private FormationController _parentController;
	private SelectionLayer _selectionLayer;
	private GhostFormation _ghostFormation;
	private Direction _ghostDirection;
	private bool _isAwaitConfirmation = false;
	private Vector2 _confirmationBearing;

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
		ui_controls = _canvasLayer.GetNode<FormationUI_controls>("UIcontrols");
		ui_confirmation = _canvasLayer.GetNode<FormationUI_confirmation>("UIconfirmation");
		ui_controls.b_Walk.Pressed += GrabGhostFormation;
		ui_confirmation.b_Cancel.Pressed += ClearSelectedFormation;
		ui_confirmation.b_Confirm.Pressed += () => ConfirmAction();

		ui_controls.Visible = false;
		ui_confirmation.Visible = false;

		Direction = Direction.CONTINUE;

		Visible = false;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		AdjustUIPosition();
		if (_ghostFormation != null && _ghostFormation.isGrabbed && !_isAwaitConfirmation)
		{
			_ghostFormation.MoveToTile(_selectionLayer.GetSelectedCell(), _ghostDirection);
			_ghostFormation.QueueRedraw();
		}
	}

	private void GrabGhostFormation()
	{
		if (_selectedFormation != null)
		{
			_ghostFormation = _selectedFormation.GhostFormation.Grab();
			ui_controls.Visible = false;
		}
	}

	private void AwaitConfirmation(Vector2 bearing)
	{
		_isAwaitConfirmation = true;
		_confirmationBearing = bearing;
		ui_confirmation.Visible = true;
	}

	private void CommenceActions()
	{
		if (!_isAwaitConfirmation && _selectedFormation != null)
		{
			_selectedFormation.ExecuteAllUnits();
		}
		ClearSelectedFormation();
	}

	private void ConfirmAction()
	{
		_ghostFormation.Place();
		_isAwaitConfirmation = false;
	}

	private void AdjustUIPosition()
	{
		if (_selectedFormation == null)
		{
			return;
		}
		ui_controls.Position = GetAdjustedPosition(_selectedFormation.GetCurrentPosition(), ui_controls.Size);

		if (_isAwaitConfirmation)
		{
			ui_confirmation.Position = GetAdjustedPosition(_confirmationBearing + new Vector2(0, 20), ui_confirmation.Size);
		}
	}

	private Vector2 GetAdjustedPosition(Vector2 targetPosition, Vector2 uiSize)
	{
		Vector2 positionDifference = targetPosition - _worldCamera.Position;
		Vector2 adjustedPosition = positionDifference * _worldCamera.Zoom;

		// Get viewport size and adjust position
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		adjustedPosition += viewportSize / 2;

		adjustedPosition -= new Vector2(uiSize.X / 2, -50 * _worldCamera.Zoom.Y);

		return adjustedPosition;
	}

	public void SetSelectedFormation(ControlledFormation formation)
	{
		ClearSelectedFormation();
		_selectedFormation = formation;
		if (_selectedFormation == null)
		{
			return;
		}
		_selectedFormation.PathfindingComplete += () => CommenceActions();
		_selectionLayer.Visible = false;
		ui_controls.Visible = true;
		Visible = true;
		_ghostFormation = formation.GhostFormation;
		_ghostDirection = formation.Direction;
		_ghostFormation.isHidden = false;
	}

	public void ClearSelectedFormation()
	{
		_isAwaitConfirmation = false;
		ui_controls.Visible = false;
		ui_confirmation.Visible = false;
		if (_selectedFormation != null)
		{
			_selectedFormation.PathfindingComplete -= () => CommenceActions();
			_selectedFormation = null;
		}
		if (_ghostFormation != null)
		{
			_ghostFormation.isHidden = true;
			_ghostFormation.Release();
			_ghostFormation = null;
		}
		_selectionLayer.Visible = true;
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
			if (_ghostFormation != null && _ghostFormation.isGrabbed)
			{
				if (mouseButton.ButtonIndex == MouseButton.Left)
				{
					if (!_isAwaitConfirmation)
					{
						AwaitConfirmation(_ghostFormation.GetCurrentPosition());
					}
				}
			}
		}
		if (@event is InputEventKey key && key.Pressed)
		{
			if (_selectedFormation != null)
			{
				if (key.Keycode == Key.Escape)
				{
					ClearSelectedFormation();
				}
			}

			if (_ghostFormation != null)
			{
				if (key.Keycode == Key.Q)
				{
					_ghostDirection = UnitUtil.GetAntiClockwiseDirection(_ghostDirection);
				}
				else if (key.Keycode == Key.E)
				{
					_ghostDirection = UnitUtil.GetClockwiseDirection(_ghostDirection);
				}
			}
		}
	}
}

