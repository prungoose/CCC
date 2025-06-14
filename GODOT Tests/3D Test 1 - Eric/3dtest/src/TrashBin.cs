using Godot;
using System;

public partial class TrashBin : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	bool playerEntry = false;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(playerEntry);
		if (playerEntry && Input.IsActionPressed("jump"))
		{

			GetTree().ChangeSceneToFile("res://assets/level/minigame.tscn");
		}
	}

	private void _OnPlayerEntry(Node3D body)
	{
		playerEntry = true;
	}
	private void _OnPlayerExit(Node3D body)
	{
		playerEntry = false;
	}
}
