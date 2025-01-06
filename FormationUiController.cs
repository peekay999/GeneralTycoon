using Godot;

public partial class FormationUiController : Node2D, IDirectionAnchor
{
	private Control _advance;
	private Control _rightWheel;
	private Control _leftWheel;
	private Control _retire;
	private Control _blockLeft;
	private Control _blockRight;
	private Formation _selectedFormation;

	public Direction _direction { get; private set; }
	public LocalisedDirections _localisedDirections { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_advance = GetNode<Control>("Advance");
		_rightWheel = GetNode<Control>("RightWheel");
		_leftWheel = GetNode<Control>("LeftWheel");
		_retire = GetNode<Control>("Retire");
		_blockLeft = GetNode<Control>("BlockLeft");
		_blockRight = GetNode<Control>("BlockRight");
		_direction = Direction.CONTINUE;

		Visible = false;
		UpdateDirection(_direction);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetFormation(Formation formation)
	{
		_selectedFormation = formation;
		Visible = true;
		Position = formation.GetCurrentPosition();
	}

	public void UpdateDirection(Direction direction)
	{
		_direction = direction;
		_localisedDirections = Pathfinder.GetLocalisedDirections(_direction);
	}
}
