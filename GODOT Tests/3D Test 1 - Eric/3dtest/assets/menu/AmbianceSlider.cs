using Godot;
using System;

public partial class AmbianceSlider : HSlider
{
	private int ambiance_index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ambiance_index = AudioServer.GetBusIndex("Ambiance");
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb((float)this.Value));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void adjustVol(float value)
	{
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb(value));
	}
}
