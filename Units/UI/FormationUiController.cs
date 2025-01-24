
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
	private GhostFormation _ghostFormation;
	private Direction _ghostDirection;
	private bool _isGhostPlaced;
	private bool _isGhostGrabbed;

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
		b_Walk.Pressed += () => _isGhostGrabbed = true;
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

		if (_isGhostGrabbed)
		{
			if (_ghostFormation != null)
			{
				_ghostFormation.Visible = true;
				_ghostFormation.MoveToTile(_selectionLayer.GetSelectedCell(), _ghostDirection);
				QueueRedraw();
			}
		}
	}

	public override void _Draw()
	{
		if ((_isGhostGrabbed || _isGhostPlaced) && _selectedFormation != null && _ghostFormation != null)
		{
			// Unit commander = _selectedFormation.GetCommander();
			Unit ghostCommander = _ghostFormation.GetCommander();
			Vector2I commanderCell = _selectedFormation.GetCurrentCell();
			Vector2 commanderPos = World.Instance.MapToWorld(commanderCell);
			// DrawLine(commander.Position, ghostCommander.Position, new Color(1, 1, 1, 0.5f));
			DrawDashedLine(commanderPos, ghostCommander.Position, new Color(1, 1, 1, 0.75f), 4.0f, 16.0f, false, false);
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
		_isGhostGrabbed = false;
		_isGhostPlaced = false;
		Visible = true;
		_canvasLayer.Visible = true;
		AddGhostCompany();
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

	private void AddGhostCompany()
	{
		if (_selectedFormation == null)
		{
			return;
		}
		RemoveGhostCompany();
		PackedScene companyScene = (PackedScene)ResourceLoader.Load("res://Units/Ghost/ghost_formation.tscn");
		_ghostFormation = (GhostFormation)companyScene.Instantiate();
		_ghostDirection = _selectedFormation.Direction;
		_ghostFormation.FormationSize = _selectedFormation.FormationSize;
		_ghostFormation.Name = "GhostCompany";
		_ghostFormation.Visible = false;

		AddChild(_ghostFormation);

		if (_selectedFormation.GetWaypoint() != null)
		{
			Waypoint waypoint = _selectedFormation.GetWaypoint();
			_ghostFormation.MoveToTile(waypoint.cell, waypoint.direction);
		}

	}

	private void RemoveGhostCompany()
	{
		if (_ghostFormation != null)
		{
			_ghostFormation.QueueFree();
			_ghostFormation = null;
			QueueRedraw();
		}
	}



	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (_isGhostGrabbed && _selectedFormation != null)
			{
				_selectedFormation.SetWaypoint(_ghostFormation.GetCurrentCell(), _ghostDirection);
				_isGhostGrabbed = false;
				_isGhostPlaced = true;
			}
		}

		if (@event is InputEventKey key && key.Pressed)
		{
			if (_isGhostGrabbed)
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
					RemoveGhostCompany();
				}
			}
			else
			{
				if (key.Keycode == Key.Escape)
				{
					ClearSelectedFormation();
				}
			}
		}
	}


}