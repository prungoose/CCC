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
			_ui.Call("IncreaseGameCompletion", 5);
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

}
