using Godot;
using System;

public partial class SfXslider : HSlider
{
	private int sfx_index;
	private AudioStreamPlayer SampleSFX;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sfx_index = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb((float)this.Value));
		SampleSFX = GetParent().GetParent().GetNode<AudioStreamPlayer>("SampleSFX");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void adjustVol(float value)
	{
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(value));
	}

	public void stopAdjustVol(bool value)
	{
		SampleSFX.Play();
	}
}
