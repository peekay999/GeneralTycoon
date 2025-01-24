using Godot;

public class Waypoint{
    public Vector2I cell;
    public Direction direction;

    public Waypoint(Vector2I cell, Direction direction){
        this.cell = cell;
        this.direction = direction;
    }
}