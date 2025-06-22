using Godot;
using System;

public partial class BacktoGameButton : Button
{
	private Control IGOptions;
	public ConfigFile CF = new ConfigFile();
	private HSlider volslider;
	private HSlider sfxslider;
	private OptionButton langbutton;
	[Export] public VBoxContainer parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IGOptions = GetParent().GetParent().GetNode<Control>("IGOptions");
		
		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		langbutton = parent.GetNode<OptionButton>("LanguageButton");

		Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void BacktoGame()
	{
		Save();
		IGOptions.Hide();
	}
	
		public void Save()
	{
		CF.SetValue("playersettings", "vol", volslider.Value);
		CF.SetValue("playersettings", "sfx", sfxslider.Value);
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
			langbutton.Selected = (int)CF.GetValue("playersettings", "lang", langbutton.Selected);
		}
	}
}
