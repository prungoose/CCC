using Godot;
using System;

public partial class TrashBin : Area3D
{

	[Export] public AudioStreamPlayer SFX;

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
			GD.Print("trash entered");
			body.QueueFree();
			SFX.PitchScale = (float)GD.RandRange(1.0, 1.7);;
			SFX.Play();
		}
	}




	
}
