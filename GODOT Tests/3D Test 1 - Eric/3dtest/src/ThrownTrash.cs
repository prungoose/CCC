using Godot;
using System;

public partial class ThrownTrash : RigidBody3D
{
	private Node3D _lightpivot;
	public override void _Ready()
	{
		_lightpivot = GetNode<Node3D>("SpotlightPivot");
	}

	public override void _Process(double delta)
	{
		_lightpivot.GlobalPosition = GlobalPosition;
	}

}
