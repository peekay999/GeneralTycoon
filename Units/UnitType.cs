using Godot;

public class UnitType 
{
    private Vector2I atlasCoords;

    private UnitType(Vector2I atlasCoords)
    {
        this.atlasCoords = atlasCoords;
    }

    public Vector2I AtlasCoords => atlasCoords;

    public static UnitType BLUE_INF = new UnitType(new Vector2I(0, 0));
    public static UnitType OPF_INF = new UnitType(new Vector2I(1, 0));
}