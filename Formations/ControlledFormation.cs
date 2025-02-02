using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;

public abstract partial class ControlledFormation : Formation<ControlledUnit>
{
    protected (int count, bool isHovered) _hoverStatus = (0, false);
    public GhostFormation GhostFormation { get; private set; }
    private int unitsExecutingActions = 0;
    private int unitsPathfinding = 0;

    [Export]
    public PackedScene CommanderScene
    {
        get;
        protected set;
    }

    [Export]
    public PackedScene RankerScene
    {
        get;
        protected set;
    }

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
        if (RankerScene == null || CommanderScene == null)
        {
            GD.PrintErr(Name + ": Ranks or Commander scene is not assigned.");
            return;
        }

        for (int i = 0; i < FormationSize; i++)
        {
            ControlledUnit unit = (ControlledUnit)RankerScene.Instantiate();
            AddChild(unit);
            Subordinates.Add(unit);
            AllUnits.Add(unit);
            unit.Name = "Unit " + Subordinates.IndexOf(unit);
        }
        ControlledUnit commander = (ControlledUnit)CommanderScene.Instantiate();
        AddChild(commander);
        AllUnits.Add(commander);
        commander.Name = "Commander";
        Commander = commander;

        FormationController formationController = World.Instance.GetFormationController();
        foreach (ControlledUnit unit in AllUnits)
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
        if (GhostFormation != null)
        {
            GhostFormation.Name = " ";
            GhostFormation.QueueFree();
        }
        GhostFormation = new GhostFormation();
        PackedScene ghostFormationScene = (PackedScene)ResourceLoader.Load("res://Formations/Units/Ghost/ghost_formation.tscn");
        if (ghostFormationScene == null)
        {
            GD.PrintErr("Failed to load ghost_formation.tscn");
            return;
        }
        GhostFormation ghostFormation = (GhostFormation)ghostFormationScene.Instantiate();
        GhostFormation = ghostFormation;
        GhostFormation.UpdateDirection(Direction.NORTH);
        GhostFormation.FormationSize = FormationSize;
        GhostFormation.Name = "GhostCompany";
        GhostFormation.UpdateDirection(Direction);
        GhostFormation.SetFormation(this);
        AddChild(GhostFormation);

        for (int i = 0; i < GhostFormation.Subordinates.Count; i++)
        {
            GhostFormation.Subordinates[i].SetControlledUnit(Subordinates[i]);
        }
        GhostFormation.Commander.SetControlledUnit(Commander);
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
            foreach (Unit unit in AllUnits)
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
            foreach (Unit unit in AllUnits)
            {
                unit.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void SetWaypoint(Waypoint waypoint)
    {
        UpdateDirection(waypoint.Direction);
        Commander.AssignPath(waypoint.Cell, waypoint.Direction);
        Vector2I[] targetCells = DressOffCommander(waypoint.Cell, waypoint.Direction);
        for (int i = 0; i < Subordinates.Count; i++)
        {
            Subordinates[i].AssignPath(targetCells[i], waypoint.Direction);
        }
    }

    public Waypoint GetWaypoint()
    {
        var actions = Commander.ActionQueue.GetActions();
        if (actions == null || actions.Length == 0)
        {
            return null;
        }

        TurnAction turnAction = actions.OfType<TurnAction>().LastOrDefault();
        MoveAction moveAction = actions.OfType<MoveAction>().LastOrDefault();

        Direction direction = turnAction?.GetDirection() ?? Direction.CONTINUE;
        Vector2I targetCell = moveAction?.GetTargetCell() ?? Vector2I.Zero;

        return new Waypoint(targetCell, direction);
    }

    public void ResetActionPoints()
    {
        Commander.ResetActionPoints();
        foreach (ControlledUnit unit in Subordinates)
        {
            unit.ResetActionPoints();
        }
    }

    public void ExecuteAllUnits()
    {

        Commander.ExecuteActions();
        foreach (ControlledUnit unit in Subordinates)
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

    public void _on_running_toggled(bool isRunning)
    {
        if (isRunning)
        {
            foreach (ControlledUnit unit in AllUnits)
            {
                unit.IsRunning = true;
            }
        }
        else
        {
            foreach (ControlledUnit unit in AllUnits)
            {
                unit.IsRunning = false;
            }
        }
    }

    public bool GetIsRunning()
    {
        bool isRunning = false;
        foreach (ControlledUnit unit in AllUnits)
        {
            if (unit.IsRunning)
            {
                isRunning = true;
                break;
            }
        }
        return isRunning;

    }
}