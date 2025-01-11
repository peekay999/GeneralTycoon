using Godot;
using System;

public abstract partial class UnitAction : Node
{
	public int _cost;
	protected Unit _unit;
	private bool _isExecuting = false;
	protected float _t = 0.0f;

    public override void _Process(double delta)
    {
        if (_isExecuting)
		{
			ProcessAction(delta);
		}
    }

    public UnitAction(Unit unit)
	{
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
