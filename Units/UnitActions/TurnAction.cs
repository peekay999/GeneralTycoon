using Godot;

public partial class TurnAction : UnitAction
{
    private Direction _direction;
    public TurnAction(Unit unit, Direction direction) : base(unit)
    {
        _cost = 5;
        _direction = direction;
    }

    protected override void ProcessAction(double delta)
    {
        _t += (float)delta * _unit.GetWalkSpeed();
        _unit.Skew = Mathf.Sin(_t * Mathf.Pi * 2 + (_unit._skewPhaseOffset / 2)) * (_unit._skewAmplitude / 2);

        if (_t > 0.5f)
        {
            _unit.UpdateDirection(_direction);
            _unit.SetAnimation(Animations.STAND);
        }
        if (_t > 1.0f)
        {
            _unit.Skew = 0.0f;
            Complete();
        }
    }
}
