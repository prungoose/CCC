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
	private CheckButton fsbutton;
	public ConfigFile CF = new ConfigFile();
	[Export] public VBoxContainer parent;

	private int main_index;
	private int sfx_index;
	private int ambiance_index;

	private Label volLabel;
	private Label sfxLabel;
	private Label ambLabel;
	private Label langLabel;
	private Label fsLabel;
	private Label OptionsTitle;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parentLabel = GetNode<VBoxContainer>("Labels");
		volLabel = parentLabel.GetNode<Label>("Volume");
		sfxLabel = parentLabel.GetNode<Label>("Sound Effects");
		ambLabel = parentLabel.GetNode<Label>("Ambiance");
		langLabel = parentLabel.GetNode<Label>("Language");
		fsLabel = parentLabel.GetNode<Label>("Fullscreen");
		OptionsTitle = GetNode<Label>("OptionsLabel");

		SampleSFX = GetNode<AudioStreamPlayer>("SampleSFX");
		SampleAMB = GetNode<AudioStreamPlayer>("SampleAMB");

		var parent = GetNode<VBoxContainer>("Adjustments");
		//GD.Print(parent.Name);
		volslider = parent.GetNode<HSlider>("VolumeSlider");
		sfxslider = parent.GetNode<HSlider>("SoundEffectsSlider");
		ambslider = parent.GetNode<HSlider>("AmbianceSlider");
		langbutton = parent.GetNode<OptionButton>("LanguageButton");
		fsbutton = GetNode<CheckButton>("FullscreenButton");

		ambiance_index = AudioServer.GetBusIndex("Ambiance");
		AudioServer.SetBusVolumeDb(ambiance_index, Mathf.LinearToDb((float)ambslider.Value));
		sfx_index = AudioServer.GetBusIndex("SFX");
		AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb((float)sfxslider.Value));
		main_index = AudioServer.GetBusIndex("Main");
		AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb((float)volslider.Value));

		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			LangSelected(langbutton.Selected);
		}
		else
		{
			LangSelected((int)CF.GetValue("playersettings", "lang"));
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

	public void Save()
	{
		CF.SetValue("playersettings", "vol", volslider.Value);
		CF.SetValue("playersettings", "sfx", sfxslider.Value);
		CF.SetValue("playersettings", "amb", ambslider.Value);
		CF.SetValue("playersettings", "lang", langbutton.Selected);
		CF.SetValue("playersettings", "screen", fsbutton.ButtonPressed);
		CF.Save(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg");
		//GD.Print(OS.GetUserDataDir());
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
			langbutton.Selected = (int)CF.GetValue("playersettings", "lang", langbutton.Selected); // 0 = Eng, 1 = Jap
			fsbutton.ButtonPressed = (bool)CF.GetValue("playersettings", "screen", fsbutton.ButtonPressed);
			//timebutton.Selected = (int)CF.GetValue("playersettings", "time", timebutton.Selected); // 3 = 2:00, 2 = 1:30, 1 = 1:00, 0 = 0:30
		}
	}

	private void _MainMenuButtonPressed()
	{
		Save();
		GetTree().ChangeSceneToFile("res://TitleScreen.tscn");
	}

	public void LangSelected(int lang)
	{
		if (lang == 0)
		{
			volLabel.Text = "Volume";
			sfxLabel.Text = "Sound Effects";
			ambLabel.Text = "Ambience";
			langLabel.Text = "Language";
			fsLabel.Text = "Fullscreen";
			OptionsTitle.Text = " Options ";
		}
		else if (lang == 1)
		{
			volLabel.Text = "音量";
			sfxLabel.Text = "効果音";
			ambLabel.Text = "雰囲気";
			langLabel.Text = "言語";
			fsLabel.Text = "全画面表示";
			OptionsTitle.Text = "設定";
		}

	}
}
