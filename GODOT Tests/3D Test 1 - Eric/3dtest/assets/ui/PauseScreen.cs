using Godot;
using System;

public partial class PauseScreen : Control
{
	private Control IGOptions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IGOptions = GetParent().GetNode<Control>("IGOptions");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play()
	{
		GetTree().Paused = false;
		this.Hide();
	}

	public void InGameOptions()
	{
		IGOptions.Show();
	}

	public void QuitLevel()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://assets/menu/MainMenu.tscn");
	}
}
