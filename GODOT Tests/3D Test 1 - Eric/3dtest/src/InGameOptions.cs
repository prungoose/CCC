using Godot;
using System;

public partial class InGameOptions : Control
{
	public ConfigFile CF = new ConfigFile();
	private HSlider volslider;
	private HSlider sfxslider;
	private HSlider ambslider;
	private OptionButton langbutton;
	[Export] public VBoxContainer parent;
	private AudioStreamPlayer SampleSFX;
	private AudioStreamPlayer SampleAMB;

	private int main_index;
	private int sfx_index;
	private int ambiance_index;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SampleSFX = GetNode<AudioStreamPlayer>("IGSampleSFX");
		SampleAMB = GetNode<AudioStreamPlayer>("IGSampleAMB");

		var parent = GetNode<VBoxContainer>("Adjustments");

		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		ambslider = parent.GetNode<HSlider>("AmbianceSlider");
		langbutton = parent.GetNode<OptionButton>("LanguageButton");

		main_index = AudioServer.GetBusIndex("Main");
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb((float)volslider.Value));
		sfx_index = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb((float)sfxslider.Value));
		ambiance_index = AudioServer.GetBusIndex("Ambiance");
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb((float)ambslider.Value));

		Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void adjustVol(float value)
	{
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb(value));
	}
	public void adjustSFX(float value)
	{
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(value));
		//GD.Print(Mathf.LinearToDb(value));
	}
	public void adjustAMB(float value)
	{
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb(value));
	}

	public void stopAdjustSFX(bool value)
	{
		SampleSFX.Play();
	}
	public void stopAdjustAMB(bool value)
	{
		SampleAMB.Play();
	}

	public void BacktoGame()
	{
		Save();
		this.Hide();
	}
	
		public void Save()
	{
		CF.SetValue("playersettings", "vol", volslider.Value);
		CF.SetValue("playersettings", "sfx", sfxslider.Value);
		CF.SetValue("playersettings", "amb", ambslider.Value);
		CF.SetValue("playersettings", "lang", langbutton.Selected);
		CF.Save(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg");
		GD.Print(OS.GetUserDataDir());
	}
	
		public void Load()
	{
		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			Save();
		}
		else
		{
			volslider.Value = (float)CF.GetValue("playersettings", "vol", volslider.Value);
			sfxslider.Value = (float)CF.GetValue("playersettings", "sfx", sfxslider.Value);
			ambslider.Value = (float)CF.GetValue("playersettings", "amb", ambslider.Value);
			langbutton.Selected = (int)CF.GetValue("playersettings", "lang", langbutton.Selected);
		}
	}
}

