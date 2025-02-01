using Godot;

public struct Assets
{
    public static PackedScene LoadScene(string path)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load(path);
        if (scene == null)
        {
            GD.PrintErr("Failed to load scene at path: " + path);
        }
        return scene;
    }

    public static string GhostUnitScenePath => "res://Formations/Units/Ghost/ghost_unit.tscn";
}