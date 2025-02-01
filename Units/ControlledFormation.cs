using Godot;

public abstract partial class ControlledFormation : Formation
{
    protected (int count, bool isHovered) _hoverStatus = (0, false);
    public GhostFormation GhostFormation { get; private set; }
    private int unitsExecutingActions = 0;
    private int unitsPathfinding = 0;

    [Signal]
    public delegate void PathfindingCompleteEventHandler();

    [Signal]
    public delegate void StartExecutingActionsEventHandler();
    [Signal]
    public delegate void AllPointsExpendedEventHandler();
    [Signal]
    public delegate void FormationSelectedEventHandler();


    public override void _Ready()
    {
        base._Ready();
        CreateGhostFormation();
    }

    protected override void InitialiseUnits()
    {
        base.InitialiseUnits();
        FormationController formationController = World.Instance.GetFormationController();
        foreach (Unit unit in _allUnits)
        {
            unit.UnitMoved += (currentCell, targetCell) => formationController._on_unit_moved(unit, currentCell, targetCell);
            unit.MouseEntered += () => _on_mouse_entered();
            unit.MouseExited += () => _on_mouse_exited();
            unit.StartExecutingActions += () => unitsExecutingActions++;
            unit.ActionQueue.FinishedExecuting += () => EmitSignal(SignalName.AllPointsExpended);
            unit.PathfindingStarted += () => unitsPathfinding++;
            unit.PathfindingComplete += () => _on_unit_pathfinding_complete();
        }
    }

    protected virtual void CreateGhostFormation()
    {
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
    }

    public override void _Input(InputEvent @event)
    {
        if (_hoverStatus.isHovered == true && World.Instance.GetFormationController().GetSelectedFormation() != this)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
            {
                SelectFormation();
            }
        }
    }

    private void _on_unit_pathfinding_complete()
    {
        unitsPathfinding--;
        if (unitsPathfinding <= 0)
        {
            EmitSignal(SignalName.PathfindingComplete);
            unitsPathfinding = 0;
        }
    }

    public void SelectFormation()
    {
        EmitSignal(SignalName.FormationSelected);
        _hoverStatus.isHovered = false;
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
        if (_hoverStatus.count > 0 && World.Instance.GetFormationController().GetSelectedFormation() != this)
        {
            _hoverStatus.isHovered = true;
            Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
            foreach (Unit unit in _allUnits)
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
            foreach (Unit unit in _allUnits)
            {
                unit.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void ExecuteAllUnits()
    {
        _commander.ExecuteActions();
        foreach (Unit unit in _subordinates)
        {
            unit.ExecuteActions();
        }
        EmitSignal(SignalName.StartExecutingActions);
    }

    private void _on_unit_expend_all_points()
    {
        unitsExecutingActions--;
        if (unitsExecutingActions <= 0)
        {
            EmitSignal(SignalName.AllPointsExpended);
            unitsExecutingActions = 0;
        }
    }
}