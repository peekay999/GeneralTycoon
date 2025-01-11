using System;
using System.Collections.Generic;
using Godot;

public partial class MoveAction : UnitAction
{
    private Vector2I _targetCell;
    private Vector2I _startCell;
    private Vector2 _targetPos;
    private Vector2 _startPos;
    private float _t = 0.0f;
    private float pauseTime = 1.20f;

    public MoveAction(int cost, Unit unit, Vector2I startCell, Vector2I targetCell) : base(cost, unit)
    {
        _targetCell = targetCell;
        _startCell = startCell;
        _targetPos = World.Instance.MapToWorld(targetCell);
        _startPos = World.Instance.MapToWorld(startCell);
    }

    public override void _Ready()
    {
        base._Ready();
    }

    protected override void ProcessAction(double delta)
    {
        _t += (float)delta * _unit.GetWalkSpeed();

        _unit.Position = _startPos.Lerp(_targetPos, _t);
        _unit.UpdateSpritesYoffset(_startCell);
        _unit.UpdateDirection(UnitUtil.DetermineDirection(_startCell, _targetCell));
        if (_t >= 1.0f)
        {
            _unit.MoveToTile(_targetCell);
            Complete();
            // pauseTime -= (float)delta;
            // if (pauseTime <= 0)
            // {
            // }
        }

        // pauseTime -= (float)delta;
        // if (pauseTime <= 0)
        // {
        //     _unit.MoveToTile(_targetCell);
        //     Complete();
        // }
    }

    public Vector2I GetTargetCell()
    {
        return _targetCell;
    }

    // public static List<MoveAction> PathToMoveList(List<Vector2I> path)
    // {
    //     List<MoveAction> moveActions = new List<MoveAction>();
    //     for (int i = 0; i < path.Count - 1; i++)
    //     {
    //     int cost = Pathfinder.GetMovementCost(path[i], path[i + 1]);
    //         moveActions.Add(new MoveAction(cost, unit) { targetCell = path[i + 1] });
    //     }
    //     return moveActions;
    // }

    // public static List<Vector2I> MoveListToPath(List<MoveAction> moveActions)
    // {
    //     List<Vector2I> path = new List<Vector2I>();
    //     foreach (MoveAction moveAction in moveActions)
    //     {
    //         path.Add(moveAction._targetPos);
    //     }
    //     return path;
    // }
}