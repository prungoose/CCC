using Godot;
using Microsoft.VisualBasic;
using System;
using System.IO;

public partial class Global : Node
{
	private AudioStreamPlayer mainVol;
	[Export] private Control Options;
	public ConfigFile CF = new ConfigFile();
    private float val;
	private int main_index;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		main_index = AudioServer.GetBusIndex("Main");

		mainVol = GetNode<AudioStreamPlayer>("Main");

		var playlist = new AudioStreamPlaylist();
		playlist.Shuffle = true;
		playlist.Loop = true;

		var list = new string[]
		{
			"res://assets/Audios/Global Music/1. soiree.wav",
			"res://assets/Audios/Global Music/2. dewwy.wav",
			"res://assets/Audios/Global Music/3. puddleworld.wav",
			"res://assets/Audios/Global Music/4. alright apothecary.wav",
			"res://assets/Audios/Global Music/5. chachuu.wav",
			"res://assets/Audios/Global Music/6. beamrider.wav",
			"res://assets/Audios/Global Music/7. mirth.wav",
			"res://assets/Audios/Global Music/8. comeaux.wav",
			"res://assets/Audios/Global Music/9. wub time.wav",
			"res://assets/Audios/Global Music/10. sighonara.wav",
		};

		playlist.SetStreamCount(list.Length);
		for (int i = 0; i < list.Length; i++)
		{
			var audio = GD.Load<AudioStreamWav>(list[i]);
			playlist.SetListStream(i, audio);
		}

		mainVol.Stream = playlist;
		mainVol.Play();

		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb(1f));
			CF.SetValue("playersettings", "vol", 1f);
			CF.SetValue("playersettings", "sfx", 1f);
			CF.SetValue("playersettings", "amb", 1f);
			CF.SetValue("playersettings", "lang", 0);
			CF.SetValue("playersettings", "screen", false);
			CF.Save(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg");
		}
		else
		{
			val = (float)CF.GetValue("playersettings", "vol");
			AudioServer.SetBusVolumeDb(main_index, Mathf.LinearToDb(val));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
