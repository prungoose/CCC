using Godot;
using System;

public partial class TrashSpawner : Node3D
{

	private float acc = 0;
	[Export] private PackedScene _spawn;
	[Export] private int type;

	float min = .5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (type == 0)
		{
			min = .2f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		while (acc > min && GetChildCount() < 30) {
			var spawn = _spawn.Instantiate<RigidBody3D>();
			AddChild(spawn);
			spawn.Call("_ChangeToType", type);
			acc = 0;
		}
		acc += (float)delta;
	}
}
