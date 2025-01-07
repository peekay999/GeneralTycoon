using Godot;

public partial class FormationUiController : Node2D, IDirectionAnchor
{
	private Node2D _advance;
	private Node2D _rightWheel;
	private Node2D _leftWheel;
	private Node2D _retire;
	private Node2D _blockLeft;
	private Node2D _blockRight;
	private Formation _selectedFormation;

	public Direction _direction { get; private set; }
	public LocalisedDirections _localisedDirections { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_advance = GetNode<Node2D>("Advance");
		_rightWheel = GetNode<Node2D>("RightWheel");
		_leftWheel = GetNode<Node2D>("LeftWheel");
		_retire = GetNode<Node2D>("Retire");
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
