using Godot;
using System;

public partial class AmbianceSFX : Node3D
{
	public AudioStreamPlayer ambianceSFX;
	private int sfx_index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ambianceSFX = GetNode<AudioStreamPlayer>("AmbianceSFX");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!ambianceSFX.Playing)
			ambianceSFX.Play();
	}
}
