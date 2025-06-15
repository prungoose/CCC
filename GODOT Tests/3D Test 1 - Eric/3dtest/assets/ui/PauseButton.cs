using Godot;
using System;

public partial class PauseButton : Button
{
	private Control _pauseScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent<Control>();
		_pauseScreen = parent.GetNode<Control>("PauseScreen");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Paused()
	{
		GetTree().Paused = true;
		_pauseScreen.Show();
	}
}
