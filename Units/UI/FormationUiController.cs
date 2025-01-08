
using System.Numerics;
using Godot;

public partial class FormationUiController : Node2D, IDirectionAnchor
{
	private ControlledFormation _selectedFormation;
	private FormationController _parentController;
	private SelectionLayer _selectionLayer;

	private FormationUI _advance;
	private FormationUI _leftWheel;
	private FormationUI _rightWheel;
	private FormationUI _retire;
	private FormationUI _blockLeft;
	private FormationUI _blockRight;
	private GhostFormation _ghostFormation;
	private Direction _ghostDirection;

	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }

	[Signal]
	public delegate void MouseCellRequestedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parentController = GetParent<FormationController>();
		_selectionLayer = World.Instance.GetSelectionLayer();

		_advance = GetNode<FormationUI>("Advance");
		_leftWheel = GetNode<FormationUI>("RightWheel");
		_rightWheel = GetNode<FormationUI>("LeftWheel");
		_retire = GetNode<FormationUI>("Retire");

		_advance.MoveAttempted += (currentCell, targetCell) => _parentController._on_tileMover_move_attempted(_advance, targetCell);
		_leftWheel.MoveAttempted += (currentCell, targetCell) => _parentController._on_tileMover_move_attempted(_leftWheel, targetCell);
		_rightWheel.MoveAttempted += (currentCell, targetCell) => _parentController._on_tileMover_move_attempted(_rightWheel, targetCell);
		_retire.MoveAttempted += (currentCell, targetCell) => _parentController._on_tileMover_move_attempted(_retire, targetCell);

		Direction = Direction.CONTINUE;

		Visible = false;
		UpdateDirection(Direction);
	}

	public override void _Input(InputEvent @event)
	{
		//if is a left mouse click
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			// Vector2I cell = _selectionLayer.GetSelectedCell();
			// if (cell.X != -1 && cell.Y != -1)
			// {
			// 	AddFormation(cell);
			// }
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
					// if (_ghostFormation != null)
					// {
					// 	RemoveGhostCompany();
					// 	return;
					// }
					RemoveGhostCompany();
					ClearSelection();

				}
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_ghostFormation != null && _selectionLayer.GetUsedCells().Count > 0)
		{
			_ghostFormation.MoveToTile(_selectionLayer.GetUsedCells()[0], _ghostDirection);

		}
	}

	private void AddGhostCompany()
	{
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
		}
	}

	public void ClearSelection()
	{
		_selectedFormation = null;
		Visible = false;
		_selectionLayer.Visible = true;
	}

	public ControlledFormation GetSelectedFormation()
	{
		return _selectedFormation;
	}

	public void SetFormation(ControlledFormation formation)
	{
		RemoveGhostCompany();
		_selectedFormation = formation;
		Visible = true;
		UpdateDirection(formation.Direction);

		Vector2I commanderCell = formation.GetCurrentCell();
		_advance.MoveToTile(commanderCell + LocalisedDirections.forward * 3);
		_rightWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.right * 3);
		_leftWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.left * 3);
		_retire.MoveToTile(commanderCell + LocalisedDirections.back * 2);

		AddGhostCompany();

	}

	public void UpdateDirection(Direction direction)
	{
		Direction = direction;
		LocalisedDirections = Pathfinder.GetLocalisedDirections(Direction);
		UpdateButtonIcons(Direction);
	}

	public void UpdateButtonIcons(Direction direction)
	{
		switch (direction)
		{
			case Direction.CONTINUE:

				break;
			case Direction.NORTH_WEST:
				_advance.UpdateButtonIcons(Assets.formationUI_NW);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_NW_W);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_NW_N);
				_retire.UpdateButtonIcons(Assets.formationUI_SE);
				break;
			case Direction.NORTH:
				_advance.UpdateButtonIcons(Assets.formationUI_N);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_N_NW);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_N_NE);
				_retire.UpdateButtonIcons(Assets.formationUI_S);
				break;
			case Direction.NORTH_EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_NE);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_NE_N);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_NE_E);
				_retire.UpdateButtonIcons(Assets.formationUI_SW);
				break;
			case Direction.EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_E);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_E_NE);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_E_SE);
				_retire.UpdateButtonIcons(Assets.formationUI_W);
				break;
			case Direction.SOUTH_EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_SE);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_SE_E);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_SE_S);
				_retire.UpdateButtonIcons(Assets.formationUI_NW);
				break;
			case Direction.SOUTH:
				_advance.UpdateButtonIcons(Assets.formationUI_S);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_S_SE);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_S_SW);
				_retire.UpdateButtonIcons(Assets.formationUI_N);
				break;
			case Direction.SOUTH_WEST:
				_advance.UpdateButtonIcons(Assets.formationUI_SW);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_SW_W);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_SW_S);
				_retire.UpdateButtonIcons(Assets.formationUI_NE);
				break;
			case Direction.WEST:
				_advance.UpdateButtonIcons(Assets.formationUI_W);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_W_SW);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_W_NW);
				_retire.UpdateButtonIcons(Assets.formationUI_E);
				break;
		}
	}
}
