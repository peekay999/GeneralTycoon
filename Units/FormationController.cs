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
	private FormationUiController _formationUiController;
	private HashSet<Formation> _formations;
	private Dictionary<Unit, Vector2I> _units;

	public override void _Ready()
	{
		_world = GetParent<Node2D>();
		_tileMapController = _world.GetNode<TileMapController>("TileMapController");
		_selectionLayer = _world.GetNode<SelectionLayer>("SelectionLayer");
		_formationUiController = GetNode<FormationUiController>("FormationUiController");
		_units = new Dictionary<Unit, Vector2I>();

		_formations = new HashSet<Formation>();
		// _formations = new Dictionary<Formation, Vector2I>();

		AddCompany(new Vector2I(30, 30));

		AddCompany(new Vector2I(50, 40));

		AddCompany(new Vector2I(60, 50));
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
			if (key.Keycode == Key.C)
			{
				AllUnitsPerformActions();
				_formationUiController.ClearSelectedFormation();
			}
		}
	}

	/// <summary>
	/// Adds a unit to the unit layer at the specified cell.
	/// </summary>
	/// <param name="cell">The cell on the map where the unit should be placed.</param>
	public void AddCompany(Vector2I cell)
	{
		PackedScene companyScene = (PackedScene)ResourceLoader.Load("res://Units/company.tscn");
		Company company = (Company)companyScene.Instantiate();
		company.FormationSelected += () => _on_formation_selected(company);
		AddChild(company);
		_formations.Add(company);
		company.Name = "Formation " + _formations.Count;
		company.MoveToTile(cell, Direction.NORTH);
	}

	public Formation GetSelectedFormation()
	{
		return _formationUiController.GetSelectedFormation();
	}

	public void AllUnitsPerformActions()
	{
		foreach (Formation formation in _formations)
		{
			formation.ExecuteAllUnitActions();
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
}
