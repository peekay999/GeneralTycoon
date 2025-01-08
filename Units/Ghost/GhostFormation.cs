using Godot;

public partial class GhostFormation : Formation
{
    private FormationUiController _foromationIUcontroller;

    public override void _Ready()
    {
        base._Ready();
        Modulate = new Color(1, 1, 1, 0.75f);
        YSortEnabled = true;
        _foromationIUcontroller = GetParent<FormationUiController>();
        _formationController = _foromationIUcontroller.GetParent<FormationController>();

        foreach (Unit unit in _units)
        {
            unit.MoveAttempted += (currentCell, targetCell) => _formationController._on_unit_move_attempted(unit, currentCell, targetCell);
            unit.WaypointUpdated += (currentCell, targetCell, direction) => _formationController._on_unit_waypoint_updated(unit, currentCell, targetCell, direction);
        }
        _commander.MoveAttempted += (currentCell, targetCell) => _formationController._on_unit_move_attempted(_commander, currentCell, targetCell);
        _commander.WaypointUpdated += (currentCell, targetCell, direction) => _formationController._on_unit_waypoint_updated(_commander, currentCell, targetCell, direction);
    }
}