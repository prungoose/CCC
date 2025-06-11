using Godot;
using System;

public partial class TrashBin : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _TouchBin()
	{
		GetTree().ChangeSceneToFile("res://assets/level/minigame.tscn");
	}
}
