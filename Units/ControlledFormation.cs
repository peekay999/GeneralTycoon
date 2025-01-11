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
            unit.UnitMoved += (currentCell, targetCell) => _formationController._on_unit_moved(unit, currentCell, targetCell);
            unit.MouseEntered += () => _on_mouse_entered();
            unit.MouseExited += () => _on_mouse_exited();
        }
        _commander.UnitMoved += (currentCell, targetCell) => _formationController._on_unit_moved(_commander, currentCell, targetCell);
        _commander.MouseEntered += () => _on_mouse_entered();
        _commander.MouseExited += () => _on_mouse_exited();
    }

    public override void _Input(InputEvent @event)
    {
        if (_hoverStatus.isHovered == true && _formationController.GetSelectedFormation() != this)
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
        if (_hoverStatus.count > 0 && _formationController.GetSelectedFormation() != this)
        {
            _hoverStatus.isHovered = true;
            Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
            foreach (Unit unit in _units)
            {
                unit.Modulate = new Color(1, 1, 1, 0.75f);
            }
        }
    }
    private void _on_mouse_exited()
    {
        _hoverStatus.count--;
        if (_hoverStatus.count <= 0)
        {
            _hoverStatus.isHovered = false;
            Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
            foreach (Unit unit in _units)
            {
                unit.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
}