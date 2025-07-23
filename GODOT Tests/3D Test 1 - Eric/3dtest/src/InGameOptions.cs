using Godot;
using System;
using System.Diagnostics.SymbolStore;

public partial class InGameOptions : Control
{
	public ConfigFile CF = new ConfigFile();
	private HSlider volslider;
	private HSlider sfxslider;
	private HSlider ambslider;
	private CheckButton fsbutton;
	[Export] public VBoxContainer parent;
	private AudioStreamPlayer SampleSFX;
	private AudioStreamPlayer SampleAMB;

	private int main_index;
	private int sfx_index;
	private int ambiance_index;
	private int lang;
	private Label volLabel;
	private Label sfxLabel;
	private Label ambLabel;
	private Label fsLabel;
	private Label OptionsTitle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SampleSFX = GetNode<AudioStreamPlayer>("IGSampleSFX");
		SampleAMB = GetNode<AudioStreamPlayer>("IGSampleAMB");

		var parent = GetNode<VBoxContainer>("Adjustments");

		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		ambslider = parent.GetNode<HSlider>("AmbianceSlider");
		fsbutton = GetNode<CheckButton>("FullscreenButton");

		var parentLabel = GetNode<VBoxContainer>("Labels");

		volLabel = parentLabel.GetNode<Label>("Volume");
		sfxLabel = parentLabel.GetNode<Label>("Sound Effects");
		ambLabel = parentLabel.GetNode<Label>("Ambiance");
		fsLabel = parentLabel.GetNode<Label>("Fullscreen");
		OptionsTitle = GetNode<Label>("OptionsLabel");

		main_index = AudioServer.GetBusIndex("Main");
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb((float)volslider.Value));
		sfx_index = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb((float)sfxslider.Value));
		ambiance_index = AudioServer.GetBusIndex("Ambiance");
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb((float)ambslider.Value));

		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			lang = 0;
		}
		else
		{
			lang = (int)CF.GetValue("playersettings", "lang");
		}

		if (lang == 0)
		{
			volLabel.Text = "Volume";
			sfxLabel.Text = "Sound Effects";
			ambLabel.Text = "Ambience";
			fsLabel.Text = "Fullscreen";
			OptionsTitle.Text = " Options ";
		}
		else if (lang == 1)
		{
			volLabel.Text = "音量";
			sfxLabel.Text = "効果音";
			ambLabel.Text = "雰囲気";
			fsLabel.Text = "全画面表示";
			OptionsTitle.Text = "設定";
		}

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
	public void toggledFS(bool isFS)
	{
		if (isFS)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
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
		CF.SetValue("playersettings", "screen", fsbutton.ButtonPressed);
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
			fsbutton.ButtonPressed = (bool)CF.GetValue("playersettings", "screen", fsbutton.ButtonPressed);
		}
	}
}

