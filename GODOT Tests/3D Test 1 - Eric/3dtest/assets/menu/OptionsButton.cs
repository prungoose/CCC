using Godot;
using System;

public partial class OptionsButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OptionsButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/menu/Options.tscn");
	}
}
