using System.Collections.Generic;
using Godot;

/// <summary>
/// Controller class for all formations on the map. Handles unit creation and positioning.
/// </summary>
public partial class FormationController : Node2D
{
	private Node2D _world;
	private TileMapController _tileMapController;
	private SelectionLayer _selectionLayer;
	private FormationUIcontroller _formationUiController;
	private HashSet<ControlledFormation> _formations;
	private Dictionary<Unit, Vector2I> _units;

	public override void _Ready()
	{
		base._Ready();
		_world = GetParent<Node2D>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
		_selectionLayer = _world.GetNode<SelectionLayer>("SelectionLayer");
		_formationUiController = GetNode<FormationUIcontroller>("FormationUiController");
		_units = new Dictionary<Unit, Vector2I>();

		_formations = new HashSet<ControlledFormation>();
		// _formations = new Dictionary<Formation, Vector2I>();

		AddCompany(new Vector2I(30, 30));

		AddCompany(new Vector2I(50, 40));

		AddCompany(new Vector2I(60, 50));

		AllUnitsPerformActions();
	}

	/// <summary>
	/// Adds a unit to the unit layer at the specified cell.
	/// </summary>
	/// <param name="cell">The cell on the map where the unit should be placed.</param>
	public void AddCompany(Vector2I cell)
	{
		PackedScene companyScene = (PackedScene)ResourceLoader.Load("res://Formations/company.tscn");
		Company company = (Company)companyScene.Instantiate();
		company.FormationSelected += () => _on_formation_selected(company);
		AddChild(company);
		_formations.Add(company);
		company.Name = "Formation " + _formations.Count;
		company.MoveToTile(cell, Direction.NORTH);
	}

	public ControlledFormation GetSelectedFormation()
	{
		return _formationUiController.GetSelectedFormation();
	}

	public void AllUnitsPerformActions()
	{
		foreach (ControlledFormation formation in _formations)
		{
			formation.ExecuteAllUnits();
		}
	}

	public void _on_formation_selected(ControlledFormation formation)
	{
		_formationUiController.SetSelectedFormation(formation);
	}

	public void _on_unit_moved(Unit unit, Vector2I cellFrom, Vector2I cellTo)
	{
		_units[unit] = cellTo;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key)
		{
			if (key.Pressed)
			{
				if (key.Keycode == Key.C)
				{
					AllUnitsPerformActions();
					_formationUiController.ClearSelectedFormation();
				}
				if (key.Keycode == Key.Space)
				{
					foreach (ControlledFormation formation in _formations)
					{
						formation.RevealGhosts();
					}
				}

			}
			if (key.IsReleased())
			{
				if (key.Keycode == Key.Space)
				{
					foreach (ControlledFormation formation in _formations)
					{
						if (formation == _formationUiController.GetSelectedFormation())
						{
							continue;
						}
						formation.HideGhosts();
					}
				}
			}
		}
	}
}
