using Godot;
using System;

public partial class Unit : Node2D
{
	private Node2D _Ysort;
	private Node2D _sprites;
    public override void _Ready()
    {
		_Ysort = GetNode<Node2D>("YSort");
		_sprites = _Ysort.GetNode<Node2D>("Sprites");
    }

	public void updateYoffset(int value)
	{
		_Ysort.Position = new Vector2(_Ysort.Position.X, _Ysort.Position.Y - value );
		_sprites.Position = new Vector2(_sprites.Position.X, _sprites.Position.Y + value );
		_Ysort.MoveLocalY(-1);
		_sprites.MoveLocalY(1);
	}
}
