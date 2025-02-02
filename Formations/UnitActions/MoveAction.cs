using Godot;

public partial class MoveAction : UnitAction
{
    private Vector2I _targetCell;
    private Vector2I _startCell;
    private Vector2 _targetPos;
    private Vector2 _startPos;
    private float _fatigueEffect;

    public MoveAction(ControlledUnit unit, Vector2I startCell, Vector2I targetCell, float fatigueEffect) : base(unit)
    {
        _t = 0.0f;
        _cost = Pathfinder.GetMovementCost(startCell, targetCell);
        _targetCell = targetCell;
        _startCell = startCell;
        _targetPos = World.Instance.MapToWorld(targetCell);
        _startPos = World.Instance.MapToWorld(startCell);
        _fatigueEffect = fatigueEffect;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    protected override void ProcessAction(double delta)
    {
        _t += (float)delta * _unit.GetMoveSpeed();
        _unit.SetAnimation(_unit.GetMoveAnimation());

        _unit.Skew = Mathf.Sin(_t * Mathf.Pi * 2 + _unit.SkewPhaseOffset) * _unit.SkewAmplitude;
        
        _unit.LerpToTile(_startCell, _targetCell, _t);
        _unit.UpdateDirection(TileMapUtil.DetermineDirection(_startCell, _targetCell));
        // _unit.LerpToDirection(UnitUtil.DetermineDirection(_startCell, _targetCell), _t);
        if (_t >= 1.0f)
        {
            float multiplier = 0;
            if (_unit.IsRunning)
            {
                _cost *= 0.8f;
                multiplier = _unit.RunFatigueMultiplier;
            }
            _unit.Fatigue -= _fatigueEffect * multiplier;
            _unit.MoveToTile(_targetCell);
            Complete();
        }
    }

    public Vector2I GetTargetCell()
    {
        return _targetCell;
    }
}