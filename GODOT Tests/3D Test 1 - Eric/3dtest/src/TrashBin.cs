using Godot;
using System;

public partial class TrashBin : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Trashbin ready: ", Name, " | In tree: ", IsInsideTree());

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
		GD.Print($"{body.Name} entered the trash bin area.");
		if (body.Name == "Player")
		{
			playerEntry = true;
		}
	}
	private void _OnPlayerExit(Node3D body)
	{
		GD.Print($"{body.Name} exited the trash bin area.");
		if (body.Name == "Player")
		{
			playerEntry = false;
		}
	}
}
