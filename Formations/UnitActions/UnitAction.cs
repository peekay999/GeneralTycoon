using Godot;
using System;

public abstract partial class UnitAction : Node
{
	public float _cost;
	protected ControlledUnit _unit;
	private bool _isExecuting = false;
	protected float _t = 0.0f;

    public override void _Process(double delta)
    {
        if (_isExecuting)
		{
			ProcessAction(delta);
		}
    }

    public UnitAction(ControlledUnit unit)
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
