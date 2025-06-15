using Godot;
using System;

public partial class QuitLevelButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void QuitLevel()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://assets/menu/MainMenu.tscn");
	}
}
