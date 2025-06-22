using Godot;
using System;

public partial class TrashSpawner : Node3D
{

	private float acc = 0;
	[Export] private PackedScene _spawn;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		while (acc > .2) {
			this.AddChild(_spawn.Instantiate<RigidBody3D>());
			acc -= 0.2f;
		}
		acc += (float)delta;
	}
}
