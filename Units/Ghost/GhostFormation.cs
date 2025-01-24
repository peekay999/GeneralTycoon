using System.Collections.Generic;
using Godot;

public partial class GhostFormation : Formation
{
    // private FormationUiController _foromationIUcontroller;
    private Formation _formation;
    public bool isPlaced = false;
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
        if (_formation == null || Visible == false)
        {
            return;
        }
        lineDrawer.SetPoints(_formation.GetCurrentPosition(), GetCurrentPosition());
        lineDrawer.QueueRedraw();
    }

    public void SetFormation(Formation formation)
    {
        _formation = formation;
    }

    public GhostFormation Grab()
    {
        isGrabbed = true;
        isPlaced = false;
        Visible = true;
        return this;
    }

    public void Release()
    {
        isGrabbed = false;
        if (_formation.GetWaypoint() == null)
        {
            Visible = false;
            return;
        }
        MoveToTile(_formation.GetWaypoint().Cell, _formation.GetWaypoint().Direction);
        isPlaced = true;
    }

    public void Place(Vector2I cell, Direction direction)
    {
        _formation.SetWaypoint(cell, direction);
        Release();
    }
}