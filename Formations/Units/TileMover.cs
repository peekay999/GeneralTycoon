using Godot;
using System;

public abstract partial class TileMover : Node2D
{
	protected Vector2I currentCell = Vector2I.Zero;
	protected Node2D _ysortOrigin;
	protected Node2D _sprites;
	protected Direction _direction;

	[Signal]
	public delegate void UnitMovedEventHandler(Vector2I currentCell, Vector2I targetCell);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ysortOrigin = GetNode<Node2D>("YSort");
		_sprites = _ysortOrigin.GetNode<Node2D>("Sprites");
	}

	public virtual void MoveToTile(Vector2I cellTo)
	{
		TileMapLayer topLayer = World.Instance.GetTopLayer(cellTo);
		if (topLayer == null)
		{
			return;
		}
		currentCell = cellTo;
		Position = World.Instance.MapToWorld(cellTo);
		float offset = DetermineSpritesYoffset(cellTo);
		UpdateSpritesYoffset(offset - 1);
		EmitSignal(SignalName.UnitMoved, currentCell, cellTo);
	}

	public virtual void LerpToTile(Vector2I cellFrom, Vector2I cellTo, float T)
	{
		TileMapLayer topLayer = World.Instance.GetTopLayer(cellTo);
		if (topLayer == null)
		{
			return;
		}
		Vector2 startPos = World.Instance.MapToWorld(cellFrom);
		Vector2 endPos = World.Instance.MapToWorld(cellTo);
		float offsetStart = DetermineSpritesYoffset(cellFrom);
		float offsetEnd = DetermineSpritesYoffset(cellTo);
		UpdateSpritesYoffset(Mathf.Lerp(offsetStart, offsetEnd, T) + 1);
		Position = startPos.Lerp(endPos, T);
	}

	public virtual void UpdateSpritesYoffset(float offset)
	{
		_ysortOrigin.Position = new Vector2(0, offset);
		_sprites.Position = new Vector2(0, -offset);
	}

	public static float DetermineSpritesYoffset(Vector2I cell)
	{
		return World.Instance.GetCellHeight(cell);
	}

	/// <summary>
	/// Updates the direction the unit is facing.
	/// </summary>
	/// <param name="direction">The new direction to face.</param>
	public virtual void UpdateDirection(Direction direction)
	{
		if (direction == Direction.CONTINUE)
		{
			return;
		}
		_direction = direction;
	}
}
