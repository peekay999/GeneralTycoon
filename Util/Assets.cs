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

    public static string UnitScene = "res://Units/Unit.tscn";
    public static string Captain1_scene = "res://Units/British/British_captain_01.tscn";
}