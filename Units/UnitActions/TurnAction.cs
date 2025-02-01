using Godot;

public partial class TurnAction : UnitAction
{
    private Direction _direction;
    public TurnAction(ControlledUnit unit, Direction direction) : base(unit)
    {
        _cost = 5;
        _direction = direction;
    }

    public Direction GetDirection()
    {
        return _direction;
    }

    protected override void ProcessAction(double delta)
    {
        _t += (float)delta * _unit.GetWalkSpeed();
        _unit.Skew = Mathf.Sin(_t * Mathf.Pi * 2 + (_unit.SkewPhaseOffset / 2)) * (_unit.SkewAmplitude / 2);
        // _unit.LerpToDirection(_direction, _t);

        if (_t > 0.5f)
        {
            _unit.SetAnimation(Animations.STAND);
            if (_direction != Direction.CONTINUE)
            _unit.UpdateDirection(_direction);
        }
        if (_t > 1.0f)
        {
            _unit.Skew = 0.0f;
            // _unit.UpdateSpritesYoffset(1.0f);
            Complete();
        }
    }
}
