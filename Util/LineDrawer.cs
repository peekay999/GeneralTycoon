using Godot;
using System;

public partial class LineDrawer : Node2D
{
	Vector2 StartPoint { get; set; }
	Vector2 EndPoint { get; set; }

	public override void _Ready()
	{
		base._Ready();
		SetPoints(Vector2.Zero, Vector2.Zero);
		ZIndex = 1;
		ZAsRelative = true;
	}

	public override void _Draw()
	{
		DrawDashedLine(StartPoint, EndPoint, new Color(1, 1, 1, 0.75f), 4.0f, 16.0f, false, false);
	}

	public void SetPoints(Vector2 start, Vector2 end)
	{
		StartPoint = start;
		EndPoint = end;
	}

	public (Vector2 start, Vector2 end) GetPoints()
	{
		return (StartPoint, EndPoint);
	}
}
