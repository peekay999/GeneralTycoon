
using Godot;
using System.Collections.Generic;

public partial class FormationUiController : Node2D, IDirectionAnchor
{
	private ControlledFormation _selectedFormation;
	private FormationController _parentController;
	private SelectionLayer _selectionLayer;

	// private FormationUI _advance;
	// private FormationUI _leftWheel;
	// private FormationUI _rightWheel;
	// private FormationUI _retire;
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

		Direction = Direction.CONTINUE;

		Visible = false;
		UpdateDirection(Direction);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (_ghostFormation != null && _selectedFormation != null)
			{
				_selectedFormation.SetWaypoint(_ghostFormation.GetCurrentCell(), _ghostDirection);
				RemoveGhostCompany();
				ClearSelectedFormation();
			}
		}

		if (@event is InputEventKey key && key.Pressed)
		{
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

				else if (key.Keycode == Key.Escape)
				{
					RemoveGhostCompany();
					ShowUI();
				}
			}
			else
			{
				if (key.Keycode == Key.Escape)
				{
					ClearSelectedFormation();
				}
				else if (key.Keycode == Key.Space)
				{
					AddGhostCompany();
					HideUI();
				}
			}
		}
	}

	public override void _Draw()
	{
		if (_ghostFormation != null && _selectedFormation != null)
		{
			Unit commander = _selectedFormation.GetCommander();
			Unit ghostCommander = _ghostFormation.GetCommander();
			Vector2I commanderCell = _selectedFormation.GetCurrentCell();
			Vector2 commanderPos = World.Instance.MapToWorld(commanderCell);
			// DrawLine(commander.Position, ghostCommander.Position, new Color(1, 1, 1, 0.5f));
			DrawDashedLine(commanderPos, ghostCommander.Position, new Color(1, 1, 1, 0.75f), 4.0f, 16.0f, false, false);
		}
	}

	public override void _Process(double delta)
	{
		if (_ghostFormation != null && _selectionLayer.GetUsedCells().Count > 0)
		{
			_ghostFormation.MoveToTile(_selectionLayer.GetUsedCells()[0], _ghostDirection);
			QueueRedraw();
		}
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
		// _ghostFormation.Commander = _selectedFormation.Commander;
		// _ghostFormation.Ranks = _selectedFormation.Ranks;
		AddChild(_ghostFormation);
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

	public void ClearSelectedFormation()
	{
		_selectedFormation = null;
		_selectionLayer.Visible = true;
		Visible = false;
		ShowUI();
	}

	public void HideUI()
	{
		// _advance.Visible = false;
		// _leftWheel.Visible = false;
		// _rightWheel.Visible = false;
		// _retire.Visible = false;
	}

	public void ShowUI()
	{
		// _advance.Visible = true;
		// _leftWheel.Visible = true;
		// _rightWheel.Visible = true;
		// _retire.Visible = true;
	}

	public ControlledFormation GetSelectedFormation()
	{
		return _selectedFormation;
	}

	public void SetSelectedFormation(ControlledFormation formation)
	{
		_selectionLayer.Visible = false;
		_selectedFormation = formation;
		Visible = true;
		UpdateDirection(formation.Direction);

		// Vector2I commanderCell = formation.GetCurrentCell();
		// _advance.MoveToTile(commanderCell + LocalisedDirections.forward * 3);
		// _rightWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.right * 3);
		// _leftWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.left * 3);
		// _retire.MoveToTile(commanderCell + LocalisedDirections.back * 2);
	}

	public void UpdateDirection(Direction direction)
	{
		Direction = direction;
		LocalisedDirections = Pathfinder.GetLocalisedDirections(Direction);
		// UpdateButtonIcons(Direction);
	}
}

	// public void UpdateButtonIcons(Direction direction)
	// {
	// 	switch (direction)
	// 	{
	// 		case Direction.CONTINUE:

	// 			break;
	// 		case Direction.NORTH_WEST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_NW);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_NW_W);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_NW_N);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_SE);
	// 			break;
	// 		case Direction.NORTH:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_N);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_N_NW);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_N_NE);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_S);
	// 			break;
	// 		case Direction.NORTH_EAST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_NE);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_NE_N);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_NE_E);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_SW);
	// 			break;
	// 		case Direction.EAST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_E);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_E_NE);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_E_SE);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_W);
	// 			break;
	// 		case Direction.SOUTH_EAST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_SE);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_SE_E);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_SE_S);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_NW);
	// 			break;
	// 		case Direction.SOUTH:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_S);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_S_SE);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_S_SW);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_N);
	// 			break;
	// 		case Direction.SOUTH_WEST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_SW);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_SW_W);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_SW_S);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_NE);
	// 			break;
	// 		case Direction.WEST:
	// 			_advance.UpdateButtonIcons(Assets.formationUI_W);
	// 			_leftWheel.UpdateButtonIcons(Assets.formationUI_W_SW);
	// 			_rightWheel.UpdateButtonIcons(Assets.formationUI_W_NW);
	// 			_retire.UpdateButtonIcons(Assets.formationUI_E);
	// 			break;
	// 	}
