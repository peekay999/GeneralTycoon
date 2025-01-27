using System.Collections.Generic;
using Godot;

public partial class Player : Node
{
    private List<ControlledFormation> _formations = new List<ControlledFormation>();

    private int formationsExecutingActions = 0; 

    [Signal]
    public delegate void AllFormationsCompletedActionsEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public List<ControlledFormation> Formations
    {
        get => _formations;
    }

    private void _on_formation_AllPointsExpended()
    {
        formationsExecutingActions--;
        if (formationsExecutingActions == 0)
        {
            EmitSignal(SignalName.AllFormationsCompletedActions);
        }
    }

    public void AddFormation(ControlledFormation formation)
    {
        _formations.Add(formation);
        formation.StartExecutingActions += () => formationsExecutingActions++;
        formation.AllPointsExpended += _on_formation_AllPointsExpended;
    }

    public void ExecuteAllFormations()
    {
        foreach (ControlledFormation formation in _formations)
        {
            formation.ExecuteAllUnits();
        }
    }

}