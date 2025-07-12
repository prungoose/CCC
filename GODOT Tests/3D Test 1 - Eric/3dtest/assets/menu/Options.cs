using Godot;
using System;

public partial class Options : Control
{
	private AudioStreamPlayer SampleSFX;
	private AudioStreamPlayer SampleAMB;
	private HSlider volslider;
	private HSlider sfxslider;
	private HSlider ambslider;
	private OptionButton langbutton;
	private OptionButton timebutton;
	public ConfigFile CF = new ConfigFile();
	[Export] public VBoxContainer parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SampleSFX = GetNode<AudioStreamPlayer>("SampleSFX");
		SampleAMB = GetNode<AudioStreamPlayer>("SampleAMB");

		var parent = GetNode<VBoxContainer>("Adjustments");
		//GD.Print(parent.Name);
		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		ambslider = parent.GetNode<HSlider>("AmbianceSlider");
		langbutton = parent.GetNode<OptionButton>("LanguageButton");
		timebutton = parent.GetNode<OptionButton>("TimerButton");

		Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void stopAdjustSFX(bool value)
	{
		SampleSFX.Play();
	}

	public void stopAdjustAMB(bool value)
	{
		SampleAMB.Play();
	}

	public void Save()
	{
		CF.SetValue("playersettings", "vol", volslider.Value);
		CF.SetValue("playersettings", "sfx", sfxslider.Value);
		CF.SetValue("playersettings", "amb", ambslider.Value);
		CF.SetValue("playersettings", "lang", langbutton.Selected);
		CF.SetValue("playersettings", "time", timebutton.Selected);
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
			timebutton.Selected = (int)CF.GetValue("playersettings", "time", timebutton.Selected);
		}
	}

	private void _MainMenuButtonPressed()
	{
		Save();
		GetTree().ChangeSceneToFile("res://assets/menu/MainMenu.tscn");
	}
}
