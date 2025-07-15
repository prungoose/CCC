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
		if (!_enabled) return;
		if (_initial_spawn && !_active && (bool)_player.Call("GetActiveHazardCount")) _time_since_last += (float)delta;
		if (!_initial_spawn)
		{
			if (GlobalPosition.DistanceTo(_player.GlobalPosition) < 75)
			{
				_SpawnAHazard();
				_initial_spawn = true;
			}
		}
		else if (GlobalPosition.DistanceTo(_player.GlobalPosition) > 75 && !_active && _time_since_last >= _time_until_next)
		{
			var r = GD.Randf();
			if (r > 0.75) _SpawnAHazard();
			else _time_since_last = 70;
		}
		if (GlobalPosition.DistanceTo(_player.GlobalPosition) < 8 && (int)_ui.Call("GetTutorialStep") == 4) _ui.Call("NextTutorialStep");

	}

	void _HazardIsDealtWith()
	{
		_active = false;
		_time_since_last = 0;
		_player.Call("DecActiveHazardCount");
	}

	void _SpawnAHazard()
	{
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
