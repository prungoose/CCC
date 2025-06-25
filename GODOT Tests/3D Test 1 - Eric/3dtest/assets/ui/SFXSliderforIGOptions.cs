using Godot;
using System;

public partial class SFXSliderforIGOptions : HSlider
{
	private int sfx_index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sfx_index = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb((float)this.Value));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void adjustVol(float value)
	{
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(value));
		//GD.Print(Mathf.LinearToDb(value));
	}
}
