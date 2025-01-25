using System;
using Godot;

public partial class Rankers : Unit
{
    private Vector2[] spriteRandoms = new Vector2[9];
    [Export(PropertyHint.Range, "0, 5.0")]
    private float randomness = 2.5f;
    public override void _Ready()
    {
        base._Ready();
        for (int i = 0; i < unitCount; i++)
        {
            spriteRandoms[i] = new Vector2((float)GD.RandRange(0, randomness), (float)GD.RandRange(0, randomness));
        }
    }

    public override void UpdateDirection(Direction direction)
    {
        base.UpdateDirection(direction);
        if (_animatedSprite2Ds.Count > 1)
        {
            Vector2[,] positionsGrid = UnitUtil.GetSpritePositions(direction);
            Vector2[] positions = new Vector2[9];

            bool isVertical = direction == Direction.NORTH || direction == Direction.SOUTH ||
                              direction == Direction.NORTH_WEST || direction == Direction.SOUTH_EAST;
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (isVertical)
                    {
                        positions[index++] = positionsGrid[j, i];
                    }
                    else
                    {
                        positions[index++] = positionsGrid[i, j];
                    }
                }
            }
            for (int i = 0; i < _animatedSprite2Ds.Count; i++)
            {
                _animatedSprite2Ds[i].Position = positions[i] + spriteRandoms[i];
            }
        }
    }
}