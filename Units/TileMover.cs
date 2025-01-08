using Godot;
using System;

public partial class TileMover : Node2D
{
	protected Vector2I currentCell = Vector2I.Zero;
	protected Vector2I target = Vector2I.Zero;

	[Signal]
	public delegate void MoveAttemptedEventHandler(Vector2I currentCell, Vector2I targetCell);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Moves the unit to a specific cell and updates its direction. Handles real position and tracking via the parent controller.
	/// </summary>
	/// <param name="cell">The target cell to move to.</param>
	/// <param name="direction">The direction to face after moving.</param>
	public void MoveToTile(Vector2I cellTo)
	{
		TileMapLayer topLayer = World.Instance.GetTopLayer(cellTo);
		if (topLayer == null)
		{
			return;
		}
		UpdateTransformPosition(cellTo, topLayer);
		EmitSignal(SignalName.MoveAttempted, currentCell, cellTo);
	}

	public virtual void UpdateTransformPosition(Vector2I cellTo, TileMapLayer tileMapLayer)
	{
		currentCell = cellTo;
		Vector2 worldPosition = tileMapLayer.MapToLocal(cellTo);
		Position = worldPosition + tileMapLayer.Position;
	}
}
