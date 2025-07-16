using Godot;
using System;

public partial class Hazardspawner : Node3D
{

	[Export] int[] _possible_types;
	[Export] bool _enabled;
	[Export] bool _tutorial;
	bool _active = false;
	private Godot.Collections.Array _possible_types_arr = new();
	private CharacterBody3D _player;
	private AnimatedSprite3D _minimapsprite;
	private float _time_since_last;
	private float _time_until_next = 100;
	private bool _initial_spawn = false;

	private Control _ui;


	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_minimapsprite = GetNode<AnimatedSprite3D>("MinimapSprite");
		_ui = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI");
		_time_until_next += GD.RandRange(-10, 20);
		foreach (int i in _possible_types)
		{
			_possible_types_arr.Add(i);
		}
		var r = GD.Randf();
		if ((r > 0.8 && _enabled) | _tutorial) _SpawnAHazard();
	}

	public override void _Process(double delta)
	{
		var player_dis = GlobalPosition.DistanceTo(_player.GlobalPosition);
		if (!_enabled) return;
		if (_initial_spawn && !_active && (int)_player.Call("GetActiveHazardCount") < 3) _time_since_last += (float)delta;
		if (!_initial_spawn)
		{
			if (player_dis < 75)
			{
				_SpawnAHazard();
				_initial_spawn = true;
			}
		}
		else if (player_dis > 75 && !_active && _time_since_last >= _time_until_next)
		{
			var r = GD.Randf();
			if (r > 0.75) _SpawnAHazard();
			else _time_since_last = 70;
		}
		if (_tutorial && player_dis < 8 && (int)_ui.Call("GetTutorialStep") == 4) _ui.Call("NextTutorialStep");

		if (!_active) _minimapsprite.Hide(); else _minimapsprite.Show();

	}

	void _HazardIsDealtWith()
	{
		_active = false;
		_time_since_last = 0;
		_player.Call("DecActiveHazardCount");
		// Update Game Progress
		_ui.Call("updateGameCompletionBar", 10);

		// Gift the player animal bait
	}

	void _SpawnAHazard()
	{
		if (_active) return;
		_active = true;
		_time_until_next = 100 + GD.RandRange(-10, 20);
		_player.Call("IncActiveHazardCount");
		int rand_haz = (int)_possible_types_arr.PickRandom();
		var haz_scene = GD.Load<PackedScene>("assets/hazards/hazard_" + rand_haz + ".tscn");
		var _hazard = haz_scene.Instantiate<Area3D>();
		AddChild(_hazard);
		_hazard.Call("_UpdateID", rand_haz);
	}

	void _PlayPulseAnim()
	{
		_minimapsprite.Play("warning_pulse");
	}

	void _StopPulseAnim()
	{
		_minimapsprite.Frame = 0;
		_minimapsprite.Pause();
	}
}
