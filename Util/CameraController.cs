using Godot;

public partial class CameraController : Camera2D
{
    [Export] public float MoveSpeed { get; set; } = 200.0f; // Movement speed

    private Vector2 _lastMousePosition; // Stores the last mouse position during dragging

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        HandleKeyboardInput((float)delta);
    }

    private void HandleKeyboardInput(float delta)
    {
        Vector2 direction = Vector2.Zero;

        // Check for movement input
        if (Input.IsActionPressed("ui_left"))
            direction.X -= 1;
        if (Input.IsActionPressed("ui_right"))
            direction.X += 1;
        if (Input.IsActionPressed("ui_up"))
            direction.Y -= 1;
        if (Input.IsActionPressed("ui_down"))
            direction.Y += 1;

        if (Input.IsActionJustPressed("zoom_in"))
            Zoom *= 1.1f;
        if (Input.IsActionJustPressed("zoom_out"))
            Zoom /= 1.1f;

        if (Zoom < new Vector2(0.5f, 0.5f))
        {
            Zoom = new Vector2(0.5f, 0.5f);
        }
        else if (Zoom > new Vector2(2.0f, 2.0f))
        {
            Zoom = new Vector2(2.0f, 2.0f);
        }

        // Normalize direction to ensure consistent speed
        if (direction != Vector2.Zero)
            direction = direction.Normalized();

        // Move the camera
        GlobalPosition += direction * MoveSpeed * delta;
    }
}
