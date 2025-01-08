using Godot;

public partial class GhostFormation : Formation
{
    private FormationUiController _parentController;

    public override void _Ready()
    {
        base._Ready();
        _parentController = GetParent<FormationUiController>();
    }
}