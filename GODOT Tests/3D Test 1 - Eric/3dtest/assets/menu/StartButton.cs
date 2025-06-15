using Godot;
using System;

public partial class StartButton : Button
{
	private Control _pauseScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pauseScreen = GetNode<Control>("res://assets/ui/pause_screen.tscn");
		_pauseScreen.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/level/testscene.tscn");
	}
}
