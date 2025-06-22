using Godot;
using System;

public partial class TrashBin : Area3D
{

	[Export] public AudioStreamPlayer SFX;
	private int _trashCount = 0;

	public override void _Ready()
	{
		SFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/THWOOM.wav");
	}


	public override void _Process(double delta)
	{

	}

	void _body_entered(Node3D body)
	{
		if (body.IsInGroup("thrown"))
		{

			_trashCount++;

			body.QueueFree();
			SFX.PitchScale = (float)GD.RandRange(0.8, 2.0); ;
			SFX.Play();

			// Failing to get UI here
			if (_trashCount >= 2)
			{
				// Go to next step in tutorial
				var ui = GetTree().GetRoot().GetNode<Control>("SubViewportContainer/UI");
				ui.Call("NextTutorialStep");
			}
		}
	}





}
