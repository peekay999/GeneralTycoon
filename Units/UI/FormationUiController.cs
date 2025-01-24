
using Godot;
using System.Collections.Generic;

public partial class FormationUiController : Node2D
{
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

		Direction = Direction.CONTINUE;

		Visible = false;
	}

	public void SetSelectedFormation(ControlledFormation formation)
	{
		_selectionLayer.Visible = false;
		_selectedFormation = formation;
		Visible = true;
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


	// public override void _Input(InputEvent @event)
	// {
	// 	if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
	// 	{
	// 		if (_ghostFormation != null && _selectedFormation != null)
	// 		{
	// 			_selectedFormation.SetWaypoint(_ghostFormation.GetCurrentCell(), _ghostDirection);
	// 			RemoveGhostCompany();
	// 			ClearSelectedFormation();
	// 		}
	// 	}

	// 	if (@event is InputEventKey key && key.Pressed)
	// 	{
	// 		if (_ghostFormation != null)
	// 		{
	// 			if (key.Keycode == Key.Q)
	// 			{
	// 				_ghostDirection = UnitUtil.GetAntiClockwiseDirection(_ghostDirection);
	// 			}
	// 			else if (key.Keycode == Key.E)
	// 			{
	// 				_ghostDirection = UnitUtil.GetClockwiseDirection(_ghostDirection);
	// 			}

	// 			else if (key.Keycode == Key.Escape)
	// 			{
	// 				RemoveGhostCompany();
	// 			}
	// 		}
	// 		else
	// 		{
	// 			if (key.Keycode == Key.Escape)
	// 			{
	// 				ClearSelectedFormation();
	// 			}
	// 			else if (key.Keycode == Key.Space)
	// 			{
	// 				AddGhostCompany();
	// 			}
	// 		}
	// 	}
	// }

	// public override void _Draw()
	// {
	// 	if (_ghostFormation != null && _selectedFormation != null)
	// 	{
	// 		// Unit commander = _selectedFormation.GetCommander();
	// 		Unit ghostCommander = _ghostFormation.GetCommander();
	// 		Vector2I commanderCell = _selectedFormation.GetCurrentCell();
	// 		Vector2 commanderPos = World.Instance.MapToWorld(commanderCell);
	// 		// DrawLine(commander.Position, ghostCommander.Position, new Color(1, 1, 1, 0.5f));
	// 		DrawDashedLine(commanderPos, ghostCommander.Position, new Color(1, 1, 1, 0.75f), 4.0f, 16.0f, false, false);
	// 	}
	// }

	// public override void _Process(double delta)
	// {
	// 	if (_ghostFormation != null && _selectionLayer.GetUsedCells().Count > 0)
	// 	{
	// 		_ghostFormation.MoveToTile(_selectionLayer.GetUsedCells()[0], _ghostDirection);
	// 		QueueRedraw();
	// 	}
	// }
	// private void AddGhostCompany()
	// {
	// 	if (_selectedFormation == null)
	// 	{
	// 		return;
	// 	}
	// 	RemoveGhostCompany();
	// 	PackedScene companyScene = (PackedScene)ResourceLoader.Load("res://Units/Ghost/ghost_formation.tscn");
	// 	_ghostFormation = (GhostFormation)companyScene.Instantiate();
	// 	_ghostDirection = _selectedFormation.Direction;
	// 	_ghostFormation.FormationSize = _selectedFormation.FormationSize;
	// 	_ghostFormation.Name = "GhostCompany";
	// 	// _ghostFormation.Commander = _selectedFormation.Commander;
	// 	// _ghostFormation.Ranks = _selectedFormation.Ranks;
	// 	AddChild(_ghostFormation);
	// }

	// private void RemoveGhostCompany()
	// {
	// 	if (_ghostFormation != null)
	// 	{
	// 		_ghostFormation.QueueFree();
	// 		_ghostFormation = null;
	// 		QueueRedraw();
	// 	}
	// }

	// public void ClearSelectedFormation()
	// {
	// 	_selectedFormation = null;
	// 	_selectionLayer.Visible = true;
	// 	Visible = false;
	// }

	// public ControlledFormation GetSelectedFormation()
	// {
	// 	return _selectedFormation;
	// }

	// public void SetSelectedFormation(ControlledFormation formation)
	// {
	// 	_selectionLayer.Visible = false;
	// 	_selectedFormation = formation;
	// 	Visible = true;
	// }
}