using Godot;
using System;

public partial class InGameOptionsButton : Button
{
	private Control IGOptions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IGOptions = GetParent().GetParent().GetParent().GetNode<Control>("IGOptions");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void InGameOptions()
	{
		IGOptions.Show();
	}
}
