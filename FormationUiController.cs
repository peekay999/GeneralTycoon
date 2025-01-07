
using System.Numerics;
using Godot;

public partial class FormationUiController : Node2D, IDirectionAnchor
{
	private Formation _selectedFormation;
	private FormationController _unitController;

	private FormationUI _advance;
	private FormationUI _rightWheel;
	private FormationUI _leftWheel;
	private FormationUI _retire;
	private FormationUI _blockLeft;
	private FormationUI _blockRight;

	public Direction Direction { get; private set; }
	public LocalisedDirections LocalisedDirections { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_unitController = GetParent<FormationController>();

		_advance = GetNode<FormationUI>("Advance");
		_rightWheel = GetNode<FormationUI>("RightWheel");
		_leftWheel = GetNode<FormationUI>("LeftWheel");
		_retire = GetNode<FormationUI>("Retire");

		_advance.MoveAttempted += (currentCell, targetCell) => _unitController._on_tileMover_move_attempted(_advance, targetCell);
		_rightWheel.MoveAttempted += (currentCell, targetCell) => _unitController._on_tileMover_move_attempted(_rightWheel, targetCell);
		_leftWheel.MoveAttempted += (currentCell, targetCell) => _unitController._on_tileMover_move_attempted(_leftWheel, targetCell);
		_retire.MoveAttempted += (currentCell, targetCell) => _unitController._on_tileMover_move_attempted(_retire, targetCell);

		Direction = Direction.CONTINUE;

		Visible = false;
		UpdateDirection(Direction);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ClearSelection()
	{
		_selectedFormation = null;
		Visible = false;
	}

	public void SetFormation(Formation formation)
	{
		_selectedFormation = formation;
		Visible = true;
		UpdateDirection(formation.Direction);

		Vector2I commanderCell = formation.GetCurrentCell();
		Vector2I leftMarkerCell = formation.GetLeftMarkerCell();
		Vector2I rightMarkerCell = formation.GetRightMarkerCell();
		_advance.MoveToTile(commanderCell + LocalisedDirections.forward * 3);
		// _rightWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.right * formation.GetWidth() / 2);
		// _leftWheel.MoveToTile(commanderCell + LocalisedDirections.forward * 3 + LocalisedDirections.left * formation.GetWidth() / 2);
		_leftWheel.MoveToTile(leftMarkerCell + LocalisedDirections.forward * 2);
		_rightWheel.MoveToTile(rightMarkerCell + LocalisedDirections.forward * 2);
		_retire.MoveToTile(commanderCell + LocalisedDirections.back * 2);

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
				_rightWheel.UpdateButtonIcons(Assets.formationUI_NW_W);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_NW_N);
				_retire.UpdateButtonIcons(Assets.formationUI_SE);
				break;
			case Direction.NORTH:
				_advance.UpdateButtonIcons(Assets.formationUI_N);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_N_NW);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_N_NE);
				_retire.UpdateButtonIcons(Assets.formationUI_S);
				break;
			case Direction.NORTH_EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_NE);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_NE_N);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_NE_E);
				_retire.UpdateButtonIcons(Assets.formationUI_SW);
				break;
			case Direction.EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_E);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_E_NE);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_E_SE);
				_retire.UpdateButtonIcons(Assets.formationUI_W);
				break;
			case Direction.SOUTH_EAST:
				_advance.UpdateButtonIcons(Assets.formationUI_SE);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_SE_S);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_SE_E);
				_retire.UpdateButtonIcons(Assets.formationUI_NW);
				break;
			case Direction.SOUTH:
				_advance.UpdateButtonIcons(Assets.formationUI_S);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_S_SE);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_S_SW);
				_retire.UpdateButtonIcons(Assets.formationUI_N);
				break;
			case Direction.SOUTH_WEST:
				_advance.UpdateButtonIcons(Assets.formationUI_SW);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_SW_W);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_SW_S);
				_retire.UpdateButtonIcons(Assets.formationUI_NE);
				break;
			case Direction.WEST:
				_advance.UpdateButtonIcons(Assets.formationUI_W);
				_rightWheel.UpdateButtonIcons(Assets.formationUI_W_SW);
				_leftWheel.UpdateButtonIcons(Assets.formationUI_W_NW);
				_retire.UpdateButtonIcons(Assets.formationUI_E);
				break;
		}
	}
}
