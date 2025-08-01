using Godot;
using System;

public partial class TrashBin : Node3D
{

	//1: combustibles 2: plastic 3: aluminums 4: PET
	[Export] private int _trashId;
	// Each bin has a independent trash count
	private int _trashCount = 0;
	private Control _ui;
	private CharacterBody3D _player;
	private MeshInstance3D _mesh;
	private AudioStreamPlayer3D _SFX;
	private GpuParticles3D _particles;
	private GpuParticles3D _splode_particles;
	private Node3D _tutorial_hazard;

	private Sprite3D _minimapsprite;
	private float _lastSabotageTime = -999f;
	private const float SABOTAGE_COOLDOWN = 15f;



	public override void _Ready()
	{
		_SFX = GetNode<AudioStreamPlayer3D>("SFX"); ;
		_SFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/THWOOM.wav");
		_ui = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI");
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_mesh = GetNode<MeshInstance3D>("StaticBody3D/BinColorMesh");
		_particles = GetNode<GpuParticles3D>("GPUParticles3D");
		_splode_particles = GetNode<GpuParticles3D>("GPUParticles3D2");
		_minimapsprite = GetNode<Sprite3D>("Sprite3D");

		// For starting animation in tutorial sequence
		_tutorial_hazard = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/NavigationRegion3D/Level/HazSpawners/HazardSpawner");

		Material m = (Material)_mesh.GetSurfaceOverrideMaterial(0).Duplicate(true);
		_mesh.SetSurfaceOverrideMaterial(0, m);
		switch (_trashId)
		{
			case 1: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(.99f, .39f, .32f, 1f)); _minimapsprite.Modulate = new Color(.99f, .39f, .32f, 1f); break; //red
			case 2: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(0f, .75f, .15f, 1f)); _minimapsprite.Modulate = new Color(0f, .75f, .15f, 1f); break; //green
			case 3: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(.99f, .73f, 0f, 1f)); _minimapsprite.Modulate = new Color(.99f, .73f, 0f, 1f); break; //yellow
			case 4: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(0f, .67f, .89f, 1f)); _minimapsprite.Modulate = new Color(0f, .67f, .89f, 1f); break; //blue
		}
	}


	public override void _Process(double delta)
	{
	}

	void _body_entered(CollisionObject3D body)
	{
		if (body.IsInGroup("thrown") && (int)body.Call("GetThrownTrashID") == _trashId)
		{
			// Update Game Completion
			_ui.Call("IncreaseGameCompletion", 4);
			body.CollisionLayer = 0;
			body.CollisionMask = 0;
			_splode_particles.Restart();
			_trashCount++;
			var tween = GetTree().CreateTween();
			tween.TweenProperty(body, "global_position", GlobalPosition + Vector3.Up * 0.5f, .13f).SetTrans(Tween.TransitionType.Quad);
			_SFX.PitchScale = (float)GD.RandRange(0.8, 1.4); ;
			tween.Finished += () => body.QueueFree();
			tween.Finished += () => _SFX.Play();
			tween.Finished += () => _splode_particles.Emitting = true;


			if (_trashId == 1 && _trashCount > 0 && (int)_ui.Call("GetTutorialStep") == 2)
			{
				_ui.Call("NextTutorialStep");
			}
			if (_trashId == 4 && _trashCount > 0 && (int)_ui.Call("GetTutorialStep") == 3)
			{
				_ui.Call("NextTutorialStep");
				_tutorial_hazard.Call("_PlayPulseAnim");
			}


		}
	}

	public int GetBinType()
	{
		return _trashId;
	}
	public void _RaccoonSabotage()
	{
		float currentTime = Time.GetTicksMsec() / 1000f;
		if (currentTime - _lastSabotageTime < SABOTAGE_COOLDOWN)
		{
			return;
		}
		_lastSabotageTime = currentTime;

		_SFX.Play();
		double completion = (double)_ui.Call("GetGameCompletion");
		if (completion < 5)
		{
			_ui.Call("IncreaseGameCompletion", -1 * completion);
		}
		else
		{
			_ui.Call("IncreaseGameCompletion", -5);
		}

		// have trash spawn around the bin to simulate raccoon crashing into it
		for (int i = 0; i < 1; i++)
		{
			var trash = GD.Load<PackedScene>("res://assets/trash/thrown_trash.tscn").Instantiate<ThrownTrash>();

			float angle = GD.Randf() * Mathf.Tau;
			float distance = (float)GD.RandRange(2f, 4f);

			Vector3 spawnPos = GlobalPosition + new Vector3(
				Mathf.Cos(angle) * distance,
				1f,
				Mathf.Sin(angle) * distance
			);

			GetTree().CurrentScene.AddChild(trash);

			trash.GlobalPosition = spawnPos;
			trash.Call("_ChangeToType", _trashId);

			trash.CallDeferred("apply_impulse", Vector3.Up, new Vector3(
				(float)GD.RandRange(-3f, 3f),
				(float)GD.RandRange(2f, 5f),
				(float)GD.RandRange(-3f, 3f)
			));
		}

		if (_splode_particles != null)
		{
			_splode_particles.Restart();
			_splode_particles.Emitting = true;
		}
	}
}
