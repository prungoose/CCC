using Godot;
using System;
using System.Text.RegularExpressions;

public partial class CleaningSphere : Area3D
{
	[Export] public float lifetime = 0.1f;
	[Export] public float cleanstrength = 0.1f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		SetDeferred("monitoring", true);
		Timer timer = new Timer
		{
			WaitTime = lifetime,
			OneShot = true,
			Autostart = true
		};
		timer.Timeout += QueueFree;
		AddChild(timer);
	}

	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body.IsInGroup("cleanable_vacuum"))
		{
			body.Call("CleanAt", GlobalPosition, GetRadius(), cleanstrength);
		}
	}

	private float GetRadius() {
		if (GetChild(0) is CollisionShape3D shape && shape.Shape is SphereShape3D sphere)
		{
			return sphere.Radius;
		}
		return 1f;
	}
}
