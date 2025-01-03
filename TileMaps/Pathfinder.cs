using Godot;
using System;
using System.Collections.Generic;

public partial class Pathfinder : Node2D
{
	private TileMapController _tileMapController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ZIndex = 1;
		_tileMapController = GetParent<TileMapController>();
	}

	public List<Vector2I> FindPath(Vector2I start, Vector2I goal)
	{
		List<Vector2I> openList = new List<Vector2I>();
		HashSet<Vector2I> closedList = new HashSet<Vector2I>();
		Dictionary<Vector2I, Vector2I> cameFrom = new Dictionary<Vector2I, Vector2I>();
		Dictionary<Vector2I, int> gScore = new Dictionary<Vector2I, int>();
		Dictionary<Vector2I, int> fScore = new Dictionary<Vector2I, int>();

		openList.Add(start);
		gScore[start] = 0;
		fScore[start] = Heuristic(start, goal);

		while (openList.Count > 0)
		{
			Vector2I current = GetLowestFScore(openList, fScore);
			if (current == goal)
			{
				
				QueueRedraw();
				return ReconstructPath(cameFrom, current);
			}

			openList.Remove(current);
			closedList.Add(current);

			foreach (Vector2I neighbour in GetNeighbours(current))
			{
				if (closedList.Contains(neighbour) || !IsWalkable(neighbour))
				{
					continue;
				}

				int tentativeGScore = gScore[current] + 1;
				if (!openList.Contains(neighbour))
				{
					openList.Add(neighbour);
				}
				else if (tentativeGScore >= gScore[neighbour])
				{
					continue;
				}
				cameFrom[neighbour] = current;
				gScore[neighbour] = tentativeGScore;
				fScore[neighbour] = gScore[neighbour] + Heuristic(neighbour, goal);
			}
		}

		return null;
	}

	private int Heuristic(Vector2I start, Vector2I goal)
	{
		return Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y);
	}

	private Vector2I GetLowestFScore(List<Vector2I> openList, Dictionary<Vector2I, int> fScore)
	{
		Vector2I lowest = openList[0];
		foreach (Vector2I node in openList)
		{
			if (fScore[node] < fScore[lowest])
			{
				lowest = node;
			}
		}
		return lowest;
	}

	private List<Vector2I> ReconstructPath(Dictionary<Vector2I, Vector2I> cameFrom, Vector2I current)
	{
		List<Vector2I> path = new List<Vector2I> { current };
		while (cameFrom.ContainsKey(current))
		{
			current = cameFrom[current];
			path.Add(current);
		}
		path.Reverse();
		return path;
	}

	private List<Vector2I> GetNeighbours(Vector2I cell)
	{
		Vector2I NW = new Vector2I(cell.X - 1, cell.Y - 1);
		Vector2I N = new Vector2I(cell.X, cell.Y - 1);
		Vector2I NE = new Vector2I(cell.X + 1, cell.Y - 1);
		Vector2I E = new Vector2I(cell.X + 1, cell.Y);
		Vector2I SE = new Vector2I(cell.X + 1, cell.Y + 1);
		Vector2I S = new Vector2I(cell.X, cell.Y + 1);
		Vector2I SW = new Vector2I(cell.X - 1, cell.Y + 1);
		Vector2I W = new Vector2I(cell.X - 1, cell.Y);
		List<Vector2I> neighbors = new List<Vector2I>
		{
			NW,
			N,
			NE,
			E,
			SE,
			S,
			SW,
			W
		};

		return neighbors;
	}

	private bool IsWalkable(Vector2I cell)
	{
		if (_tileMapController.GetTopLayer(cell) == null)
		{
			return false;
		}
		return true;
	}
}
