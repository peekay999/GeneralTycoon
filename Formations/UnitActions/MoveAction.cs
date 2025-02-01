using Godot;

public partial class MoveAction : UnitAction
{
    private Vector2I _targetCell;
    private Vector2I _startCell;
    private Vector2 _targetPos;
    private Vector2 _startPos;

    public MoveAction(ControlledUnit unit, Vector2I startCell, Vector2I targetCell) : base(unit)
    {
        _t = 0.0f;
        _cost = Pathfinder.GetMovementCost(startCell, targetCell);
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

        _unit.SetAnimation(Animations.WALK);
        _unit.Skew = Mathf.Sin(_t * Mathf.Pi * 2 + _unit.SkewPhaseOffset) * _unit.SkewAmplitude;
        
        _unit.LerpToTile(_startCell, _targetCell, _t);
        _unit.UpdateDirection(TileMapUtil.DetermineDirection(_startCell, _targetCell));
        // _unit.LerpToDirection(UnitUtil.DetermineDirection(_startCell, _targetCell), _t);
        if (_t >= 1.0f)
        {
            _unit.MoveToTile(_targetCell);
            Complete();
        }
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