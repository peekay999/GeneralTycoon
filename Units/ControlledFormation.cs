using Godot;

public abstract partial class ControlledFormation : Formation
{
    protected (int count, bool isHovered) _hoverStatus = (0, false);

    public override void _Ready()
    {
        base._Ready();
        YSortEnabled = true;
        _formationController = GetParent<FormationController>();
        foreach (Unit unit in _units)
        {
            unit.MoveAttempted += (currentCell, targetCell) => _formationController._on_unit_move_attempted(unit, currentCell, targetCell);
            unit.WaypointUpdated += (currentCell, targetCell, direction) => _formationController._on_unit_waypoint_updated(unit, currentCell, targetCell, direction);
            unit.MouseEntered += () => _on_mouse_entered();
            unit.MouseExited += () => _on_mouse_exited();
        }
        _commander.MoveAttempted += (currentCell, targetCell) => _formationController._on_unit_move_attempted(_commander, currentCell, targetCell);
        _commander.WaypointUpdated += (currentCell, targetCell, direction) => _formationController._on_unit_waypoint_updated(_commander, currentCell, targetCell, direction);
        _commander.MouseEntered += () => _on_mouse_entered();
        _commander.MouseExited += () => _on_mouse_exited();
    }

    public override void _Input(InputEvent @event)
    {
        if (_hoverStatus.isHovered == true && _formationController.GetSelectedFormation() == null)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
            {
                SelectFormation(this);
                _hoverStatus.isHovered = false;
            }
        }
    }

    private static void SelectFormation(ControlledFormation formation)
    {
        formation.EmitSignal("FormationSelected");
    }

    private void _on_mouse_entered()
    {
        _hoverStatus.count++;
        if (_hoverStatus.count > 0 && _formationController.GetSelectedFormation() == null)
        {
            _hoverStatus.isHovered = true;
            Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
            World.Instance.GetSelectionLayer().Visible = false;
        }
    }
    private void _on_mouse_exited()
    {
        _hoverStatus.count--;
        if (_hoverStatus.count <= 0)
        {
            _hoverStatus.isHovered = false;
            Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
        }
    }
}