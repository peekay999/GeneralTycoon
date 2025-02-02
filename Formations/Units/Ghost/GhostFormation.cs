using System.Collections.Generic;
using Godot;

public partial class GhostFormation : Formation<GhostUnit>
{
    private ControlledFormation _formation;
    public bool isHidden = true;
    public bool isGrabbed = false;
    private LineDrawer lineDrawer;

    private PackedScene GhostUnitScene => ResourceLoader.Load<PackedScene>(Assets.GhostUnitScenePath);

    public override void _Ready()
    {
        base._Ready();
        Modulate = new Color(1, 1, 1, 0.75f);

        lineDrawer = new LineDrawer();
        AddChild(lineDrawer);


    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isHidden || _formation == null)
        {
            Visible = false;
            return;
        }
        if (isGrabbed || _formation.GetWaypoint() != null)
        {
            Visible = true;
        }
        else
        {
            Visible = false;
        }
        if (Visible && !isGrabbed)
        {
            float DistanceToFormation = GetCurrentPosition().DistanceTo(_formation.GetCurrentPosition());
            Modulate = new Color(1, 1, 1, Mathf.Clamp(DistanceToFormation / 200, 0, 0.75f));
        }

        lineDrawer.SetPoints(_formation.GetCurrentPosition(), GetCurrentPosition());
        lineDrawer.QueueRedraw();
    }

    public void SetFormation(ControlledFormation formation)
    {
        _formation = formation;
    }

    public GhostFormation Grab()
    {
        Modulate = new Color(1, 1, 1, 0.75f);
        isGrabbed = true;
        return this;
    }

    public void Release()
    {
        isGrabbed = false;
        if (_formation.GetWaypoint() == null)
        {
            return;
        }
        MoveToTile(_formation.GetWaypoint().Cell, _formation.GetWaypoint().Direction);
    }

    public void Place()
    {
        if (isHidden)
        {
            return;
        }
        Waypoint waypoint = new Waypoint(GetCurrentCell(), Direction);
        _formation.SetWaypoint(waypoint);
        isGrabbed = false;
    }

    public override Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
    {
        if (_formation == null)
        {
            return new Vector2I[0];
        }
        UpdateDirection(direction);
        return _formation.DressOffCommander(commanderCell, direction);
    }

    protected override void InitialiseUnits()
    {
        for (int i = 0; i < FormationSize; i++)
        {
            GhostUnit unit = GhostUnitScene.Instantiate<GhostUnit>();
            AddChild(unit);
            Subordinates.Add(unit);
            AllUnits.Add(unit);
            unit.Name = "Ghost Unit " + Subordinates.IndexOf(unit);
        }
        GhostUnit commander = GhostUnitScene.Instantiate<GhostUnit>();
        AddChild(commander);
        AllUnits.Add(commander);
        commander.Name = "Ghost Commander";
        Commander = commander;
    }
}