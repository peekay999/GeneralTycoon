using System.Collections.Generic;
using Godot;

public partial class GhostFormation : Formation
{
    // private FormationUiController _foromationIUcontroller;
    private ControlledFormation _formation;
    public bool isHidden = true;
    public bool isGrabbed = false;

    private LineDrawer lineDrawer;

    public override void _Ready()
    {
        base._Ready();

        Modulate = new Color(1, 1, 1, 0.75f);
        YSortEnabled = true;

        lineDrawer = new LineDrawer();
        lineDrawer.SetPoints(Vector2.Zero, Vector2.Zero);
        lineDrawer.ZIndex = 1;
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

        lineDrawer.SetPoints(_formation.GetCurrentPosition(), GetCurrentPosition());
        lineDrawer.QueueRedraw();
    }

    public void SetFormation(ControlledFormation formation)
    {
        _formation = formation;
    }

    public GhostFormation Grab()
    {
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
        _formation.SetWaypoint(GetCurrentCell(), Direction);
        isGrabbed = false;
    }

    public override Vector2I[] DressOffCommander(Vector2I commanderCell, Direction direction)
    {
        UpdateDirection(direction);
        if (_formation == null)
        {
            return new Vector2I[0];
        }
        return _formation.DressOffCommander(commanderCell, direction);
    }
}