using Godot;
using System;

public partial class IGVolumeSlider : HSlider
{
	private int main_index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		main_index = AudioServer.GetBusIndex("Main");
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb((float)this.Value));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void adjustVol(float value)
	{
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb(value));
	}
}
