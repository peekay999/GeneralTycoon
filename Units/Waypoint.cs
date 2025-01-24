using Godot;

public class Waypoint{
    public Vector2I Cell;
    public Direction Direction;

    public Waypoint(Vector2I cell, Direction direction){
        Cell = cell;
        Direction = direction;
    }
}