using Godot;

public interface IDirectionAnchor
{
    Direction Direction { get; }
    LocalisedDirections LocalisedDirections { get; }
    void UpdateDirection(Direction direction);
}