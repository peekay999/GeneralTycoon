using Godot;
using System;

public partial class World : Node2D
{
	private TileMapController _tileMapController;
	private FormationController _formationController;
	private Pathfinder _pathfinder;
	private SelectionLayer _selectionLayer;
	private static World _instance;
	// Called when the node enters the scene tree for the first time.

	public override void _EnterTree()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			GD.PrintErr("Multiple instances of World detected. This should be a singleton.");
		}
		_tileMapController = GetNode<TileMapController>("TileMapController");
		_formationController = GetNode<FormationController>("FormationController");
		_pathfinder = GetNode<Pathfinder>("Pathfinder");
	}
	public override void _Ready()
	{
		// _tileMapController = GetNode<TileMapController>("TileMapController");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public static World Instance
	{
		get
		{
			if (_instance == null)
			{
				GD.PrintErr("World instance is not initialized yet.");
			}
			return _instance;
		}
	}

	public SelectionLayer GetSelectionLayer()
	{
		if (_selectionLayer == null)
		{
			_selectionLayer = GetNode<SelectionLayer>("SelectionLayer");
		}
		return _selectionLayer;
	}

	public Pathfinder GetPathfinder()
	{
		if (_pathfinder == null)
		{
			_pathfinder = GetNode<Pathfinder>("Pathfinder");
		}
		return _pathfinder;
	}

	public Vector2 MapToWorld(Vector2I cell)
	{
		return _tileMapController.GetTopLayer(cell).MapToLocal(cell) + _tileMapController.GetTopLayer(cell).Position;
	}

	public TileMapLayer GetTopLayer(Vector2I cell)
	{
		return _tileMapController.GetTopLayer(cell);
	}
}
