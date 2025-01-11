using Godot;
using System;

public abstract partial class UnitAction : Node
{
	public int _cost;
	protected Unit _unit;
	private bool _isExecuting = false;

    public override void _Process(double delta)
    {
        if (_isExecuting)
		{
			ProcessAction(delta);
		}
    }

    public UnitAction(int cost, Unit unit)
	{
		_cost = cost;
		_unit = unit;
	}
	
	public void Execute()
	{
		_isExecuting = true;
	}

	public void Complete()
	{
		_isExecuting = false;
		EmitSignal(SignalName.ActionComplete);
		QueueFree();
	}
	protected abstract void ProcessAction(double delta);

	[Signal]
	public delegate void ActionCompleteEventHandler();
}
