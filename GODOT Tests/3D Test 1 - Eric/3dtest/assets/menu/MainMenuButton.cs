using Godot;
using System;

public partial class MainMenuButton : Button
{
	private HSlider volslider;
	private HSlider sfxslider;
	private OptionButton langbutton;
	private OptionButton timebutton;
	public ConfigFile CF = new ConfigFile();
	[Export] public VBoxContainer parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//var parent = GetParent<Control>().GetNode<VBoxContainer>("Adjustments");
		GD.Print(parent.Name);
		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		langbutton = parent.GetNode<OptionButton>("LanguageButton");
		timebutton = parent.GetNode<OptionButton>("TimerButton");

		Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _MainMenuButtonPressed()
	{
		Save();
		GetTree().ChangeSceneToFile("res://assets/menu/MainMenu.tscn");
	}

	public void Save()
	{
		CF.SetValue("playersettings", "vol", volslider.Value);
		CF.SetValue("playersettings", "sfx", sfxslider.Value);
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
			langbutton.Selected = (int)CF.GetValue("playersettings", "lang", langbutton.Selected);
			timebutton.Selected = (int)CF.GetValue("playersettings", "time", timebutton.Selected);
		}
	}
}
