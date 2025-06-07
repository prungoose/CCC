using Godot;
using System;

public partial class CleaningSpawner : Node3D
{

	[Export] public PackedScene _cleaningscene;
	[Export] public Node3D _target;
	private float acc;
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("m1") && acc > 0.05)
		{
			var sphere = _cleaningscene.Instantiate<Area3D>();
			sphere.GlobalTransform = new Transform3D(Basis.Identity, _target.GlobalPosition);
			GetTree().CurrentScene.AddChild(sphere);
			acc = 0;
		}
		else
		{
			acc += (float)delta;
		}

	}
}
