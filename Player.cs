using System.Collections.Generic;
using Godot;

public partial class Player : Node
{
    private List<Formation> _formations = new List<Formation>();

    private int formationsExecutingActions = 0; 

    [Signal]
    public delegate void AllFormationsCompletedActionsEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public List<Formation> Formations
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

    public void AddFormation(Formation formation)
    {
        _formations.Add(formation);
        formation.StartExecutingActions += () => formationsExecutingActions++;
        formation.AllPointsExpended += _on_formation_AllPointsExpended;
    }

    public void ExecuteAllFormations()
    {
        foreach (Formation formation in _formations)
        {
            formation.ExecuteAllUnits();
        }
    }

}