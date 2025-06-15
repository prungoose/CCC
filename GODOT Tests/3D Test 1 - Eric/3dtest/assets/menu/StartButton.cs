using Godot;
using System;

public partial class StartButton : Button
{
	private Control _pauseScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent<Container>();
		_pauseScreen = parent.GetParent<Control>();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/level/testscene.tscn");
		_pauseScreen.Hide();
	}
}
