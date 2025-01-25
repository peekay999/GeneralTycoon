using Godot;

public abstract partial class ControlledFormation : Formation
{
    private FormationController _formationController;
    protected (int count, bool isHovered) _hoverStatus = (0, false);
    public GhostFormation GhostFormation { get; private set; }


    public override void _Ready()
    {
        base._Ready();
        YSortEnabled = true;

        _formationController = World.Instance.GetFormationController();
        foreach (Unit unit in _units)
        {
            unit.UnitMoved += (currentCell, targetCell) => _formationController._on_unit_moved(unit, currentCell, targetCell);
            unit.MouseEntered += () => _on_mouse_entered();
            unit.MouseExited += () => _on_mouse_exited();
        }
        _commander.UnitMoved += (currentCell, targetCell) => _formationController._on_unit_moved(_commander, currentCell, targetCell);
        _commander.MouseEntered += () => _on_mouse_entered();
        _commander.MouseExited += () => _on_mouse_exited();

        // Ensure the resource path is correct
        PackedScene ghostFormationScene = (PackedScene)ResourceLoader.Load("res://Units/Ghost/ghost_formation.tscn");
        if (ghostFormationScene == null)
        {
            GD.PrintErr("Failed to load ghost_formation.tscn");
            return;
        }

        // Instantiate and cast to GhostFormation
        GhostFormation ghostFormation = (GhostFormation)ghostFormationScene.Instantiate();

        GhostFormation = ghostFormation;
        GhostFormation.UpdateDirection(Direction.NORTH);
        GhostFormation.FormationSize = FormationSize;
        GhostFormation.Name = "GhostCompany";
        GhostFormation.UpdateDirection(Direction);
        GhostFormation.SetFormation(this);

        AddChild(GhostFormation);

        // SetWaypoint(GetCurrentCell(), Direction);

    }

    public override void _Input(InputEvent @event)
    {
        if (_hoverStatus.isHovered == true && _formationController.GetSelectedFormation() != this)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
            {
                SelectFormation();
                _hoverStatus.isHovered = false;
            }
        }
    }

    private void SelectFormation()
    {
        EmitSignal("FormationSelected");

    }

    public void RevealGhosts()
    {
        GhostFormation.isHidden = false;
    }

    public void HideGhosts()
    {
        if (!GhostFormation.isGrabbed)
        {
            GhostFormation.isHidden = true;
        }
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