using System.Collections.Generic;
using Godot;

public partial class GhostFormation : Formation<GhostUnit>
{
    private ControlledFormation _formation;
    public bool isHidden = true;
    public bool isGrabbed = false;
    private LineDrawer lineDrawer;

    private PackedScene GhostUnitScene => ResourceLoader.Load<PackedScene>(Assets.GhostUnitScenePath);
    private Color _defaultColor = new Color(1, 1, 1, 0.75f);
    private Color _color;

    public override void _Ready()
    {
        base._Ready();
        Modulate = new Color(1, 1, 1, 0.75f);

        _color = _defaultColor;

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
        if (Visible)
        {
            (float low, float high) clamp = (0.00f, 0.75f);
            if (isGrabbed)
            {
                clamp.low = 0.5f;
            }
            float DistanceToFormation = GetCurrentPosition().DistanceTo(_formation.GetCurrentPosition());
            _color.A = Mathf.Clamp(DistanceToFormation / 100, clamp.low, clamp.high);
            Modulate = _color;
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
        ResetColour();
        isGrabbed = true;
        return this;
    }

    private void ResetColour()
    {
        _color = _defaultColor;
        Modulate = _color;
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