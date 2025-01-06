using Godot;

public interface IDirectionAnchor
{
    Direction _direction { get; }
    LocalisedDirections _localisedDirections { get; }
    void UpdateDirection(Direction direction);
}