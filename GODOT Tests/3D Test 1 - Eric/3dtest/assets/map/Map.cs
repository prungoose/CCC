using Godot;
using System;

public partial class Map : Node3D
{
	[Export] private bool _title = false;
	private WorldEnvironment _worldenvironment;
	private DirectionalLight3D _directionallight;
	private GpuParticles3D _rain;
	private int _day_phase = 0;
	private float _game_time = 0;
	private Tween _tween;
	private CharacterBody3D _player;
	private SpotLight3D _flashlight;
	private Control _textbox;

	//first time flags for evening, rain
	private bool[] _flags = { false, false };
	private AudioStreamPlayer rainSFX;

	float transition_time = 5.0f;

	public override void _Ready()
	{
		rainSFX = GetNode<AudioStreamPlayer>("RainSFX");
		_worldenvironment = GetNode<WorldEnvironment>("WorldEnvironment");
		_directionallight = GetNode<DirectionalLight3D>("DirectionalLight3D");
		_rain = GetNode<GpuParticles3D>("GPUParticles3D");
		if (!_title)
		{
			_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
			_textbox = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI/Textbox");
		}
		if (_player != null)
		{
			_flashlight = _player.GetNode<SpotLight3D>("LightPivot/Light");
		}
	}

	public override void _Process(double delta)
	{
		if (!_title) _game_time += (float)delta;
		if (_player != null) _rain.GlobalPosition = _player.GlobalPosition;

		if (_game_time > 75 && _day_phase == 0) 	  EveningTransition();
		else if (_game_time > 125 && _day_phase == 1) StormyNightTransition();
		else if (_game_time > 170 && _day_phase == 2) MorningTransition();
		
	}


	void EveningTransition()
	{
		rainSFX.Stop();
		if (_textbox != null && !_flags[0])
		{
			_flags[0] = true;
			_textbox.Call("PopUp", (string[])["Looks like evening time is rolling around...", "The reports show it might get [b]stormy[/b] around nighttime, so be careful."], 1);
		}

		_day_phase += 1;
		var sky = _worldenvironment.Environment.Sky.SkyMaterial as ProceduralSkyMaterial;

		_tween = GetTree().CreateTween();
		_tween.SetParallel();

		var skytop_f = sky.SkyTopColor;
		var skytop_t = new Color("#ffa67f");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyTopColor = c),
			skytop_f,
			skytop_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var skyh_f = sky.SkyHorizonColor;
		var skyh_t = new Color("#e0a5c1");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyHorizonColor = c),
			skyh_f,
			skyh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndbot_f = sky.GroundBottomColor;
		var grndbot_t = new Color("#9c768d");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundBottomColor = c),
			grndbot_f,
			grndbot_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndh_f = sky.GroundHorizonColor;
		var grndh_t = new Color("#dbe7fe");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundHorizonColor = c),
			grndh_f,
			grndh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		_tween.TweenProperty(_directionallight, "light_energy", .5f, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_directionallight, "rotation", new Godot.Vector3(Mathf.DegToRad(-28), Mathf.DegToRad(125), 0), transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_worldenvironment.Environment, "ambient_light_energy", .2, transition_time).SetEase(Tween.EaseType.InOut);
	}

	void StormyNightTransition()
	{
		if (_textbox != null && !_flags[1])
		{
			_flags[1] = true;
			_textbox.Call("PopUp", (string[])["A storm is starting in your area!", "Your vacuum should have a built-in flashlight, you can use it to light your surroundings."], 1);
		}

		if (!rainSFX.Playing) rainSFX.Play();
		_day_phase += 1;
		var sky = _worldenvironment.Environment.Sky.SkyMaterial as ProceduralSkyMaterial;

		_tween = GetTree().CreateTween();
		_tween.SetParallel();

		var skytop_f = sky.SkyTopColor;
		var skytop_t = new Color("#004051");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyTopColor = c),
			skytop_f,
			skytop_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var skyh_f = sky.SkyHorizonColor;
		var skyh_t = new Color("#000000");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyHorizonColor = c),
			skyh_f,
			skyh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndbot_f = sky.GroundBottomColor;
		var grndbot_t = new Color("#005879");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundBottomColor = c),
			grndbot_f,
			grndbot_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndh_f = sky.GroundHorizonColor;
		var grndh_t = new Color("#000117");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundHorizonColor = c),
			grndh_f,
			grndh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		_tween.TweenProperty(_directionallight, "light_energy", 0, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_directionallight, "rotation", new Godot.Vector3(Mathf.DegToRad(0), Mathf.DegToRad(125), 0), transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_worldenvironment.Environment, "ambient_light_energy", 0.04, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_worldenvironment.Environment, "background_energy_multiplier", .5, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_flashlight, "light_energy", .5, transition_time);
		_tween.TweenProperty(_flashlight, "spot_range", 12, transition_time);
		_tween.TweenProperty(_flashlight, "spot_angle_attenuation", .2, transition_time);
		_tween.Finished += () => _rain.Emitting = true;
	}
	
		void MorningTransition()
	{
		rainSFX.Stop();
		_day_phase = 0;
		_game_time = 0;
		_directionallight.Rotation = new Godot.Vector3(0, Mathf.DegToRad(-23), 0);
		_rain.Emitting = false;
		var sky = _worldenvironment.Environment.Sky.SkyMaterial as ProceduralSkyMaterial;

		_tween = GetTree().CreateTween();
		_tween.SetParallel();

		var skytop_f = sky.SkyTopColor;
		var skytop_t = new Color("#3bd2ff");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyTopColor = c),
			skytop_f,
			skytop_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var skyh_f = sky.SkyHorizonColor;
		var skyh_t = new Color("#dbe7fe");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.SkyHorizonColor = c),
			skyh_f,
			skyh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndbot_f = sky.GroundBottomColor;
		var grndbot_t = new Color("#005879");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundBottomColor = c),
			grndbot_f,
			grndbot_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		var grndh_f = sky.GroundHorizonColor;
		var grndh_t = new Color("#000117");
		_tween.TweenMethod(
			Callable.From<Color>(c => sky.GroundHorizonColor = c),
			grndh_f,
			grndh_t,
			transition_time
		).SetEase(Tween.EaseType.InOut);

		_tween.TweenProperty(_directionallight, "light_energy", 1, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_directionallight, "rotation", new Godot.Vector3(Mathf.DegToRad(-63), Mathf.DegToRad(60), 0), transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_worldenvironment.Environment, "ambient_light_energy", .5, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_worldenvironment.Environment, "background_energy_multiplier", 1, transition_time).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(_flashlight, "light_energy", .1, transition_time);
		_tween.TweenProperty(_flashlight, "spot_range", 8.5, transition_time);
		_tween.TweenProperty(_flashlight, "spot_angle_attenuation", .002, transition_time);
	}

}
