using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Pathfinder : Node2D
{
	private TileMapController _tileMapController;
	private UnitController _unitController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ZIndex = 1;
		_tileMapController = GetParent<World>().GetNode<TileMapController>("TileMapController");
		_unitController = GetParent<World>().GetNode<UnitController>("UnitController");
	}

	public async Task<List<Vector2I>> FindPathAsync(Vector2I start, Vector2I goal)
	{
		return await Task.Run(() => FindPath(start, goal));
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

		Vector2I closestPoint = start;
		int closestPointHeuristic = Heuristic(start, goal);

		while (openList.Count > 0)
		{
			Vector2I current = GetLowestFScore(openList, fScore);

			if (current == goal)
			{
				List<Vector2I> path = ReconstructPath(cameFrom, current);
				path.RemoveAt(0);
				return path;
			}

			openList.Remove(current);
			closedList.Add(current);

			foreach (Vector2I neighbour in GetNeighbours(current))
			{
				if (closedList.Contains(neighbour) || !IsWalkable(current, neighbour))
				{
					continue;
				}

				int tentativeGScore = gScore[current] + GetMovementCost(current, neighbour);
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

				// Update closest point if this neighbour is closer to the goal
				int neighbourHeuristic = Heuristic(neighbour, goal);
				if (neighbourHeuristic < closestPointHeuristic)
				{
					closestPoint = neighbour;
					closestPointHeuristic = neighbourHeuristic;
				}
			}
		}

		List<Vector2I> closestPath = ReconstructPath(cameFrom, closestPoint);
		closestPath.RemoveAt(0);
		return closestPath;
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

	private int GetMovementCost(Vector2I from, Vector2I to)
	{
		int movementCost;
		// Check if the movement is diagonal
		if (from.X != to.X && from.Y != to.Y)
		{
			movementCost = 14; // Diagonal movement cost (1.4 * 10)
		}
		else
		{
			movementCost = 10; // Horizontal or vertical movement cost (1 * 10)
		}
		if (_unitController.GetUnitLayer().GetCellSourceId(to) != -1)
		{
			movementCost += 100;
		}

		return movementCost;
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

	private bool IsWalkable(Vector2I cellTo)
	{
		if (_tileMapController.GetTopLayer(cellTo) == null || _tileMapController.GetTopLayer(cellTo).GetCellSourceId(cellTo) == -1)
		{
			return false;
		}
		return true;
	}

	private bool IsWalkable(Vector2I cellFrom, Vector2I cellTo)
	{
		if (!IsWalkable(cellTo))
		{
			return false;
		}
		if (_tileMapController.GetTopLayerOffset(cellTo) - _tileMapController.GetTopLayerOffset(cellFrom) > 16 || _tileMapController.GetTopLayerOffset(cellTo) - _tileMapController.GetTopLayerOffset(cellFrom) < -16)
		{
			return false;
		}
		return true;
	}
}
