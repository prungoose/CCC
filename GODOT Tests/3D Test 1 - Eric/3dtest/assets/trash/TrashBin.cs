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
	private AudioStreamPlayer _SFX;
	private GpuParticles3D _particles;

	private StaticBody3D _majorObstacle;




	public override void _Ready()
	{
		_SFX = GetNode<AudioStreamPlayer>("SFX"); ;
		_SFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/THWOOM.wav");
		_ui = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI");
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_mesh = GetNode<MeshInstance3D>("StaticBody3D/BinColorMesh");
		_particles = GetNode<GpuParticles3D>("GPUParticles3D");

		// For starting animation in tutorial sequence
		_majorObstacle = GetTree().CurrentScene.GetNode<StaticBody3D>("SubViewportContainer/SubViewport/Level/Major Obstacle");

		Material m = (Material)_mesh.GetSurfaceOverrideMaterial(0).Duplicate(true);
		_mesh.SetSurfaceOverrideMaterial(0, m);
		switch (_trashId)
		{
			case 1: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(.99f, .39f, .32f, 1f)); break; //red
			case 2: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(0f, .67f, .89f, 1f)); break; //blue
			case 3: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(0f, .75f, .15f, 1f)); break; //green
			case 4: _mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(.99f, .73f, 0f, 1f)); break; //yellow
		}
	}


	public override void _Process(double delta)
	{
		if ((int)_player.Call("GetCurrentTrashID") == _trashId)
		{
			_particles.Emitting = true;
		}
		else
		{
			_particles.Emitting = false;
		}
	}

	void _body_entered(Node3D body)
	{
		if (body.IsInGroup("thrown") && (int)body.Call("GetThrownTrashID") == _trashId)
		{
			_trashCount++;
			body.QueueFree();
			_SFX.PitchScale = (float)GD.RandRange(0.8, 2.0); ;
			_SFX.Play();
			_player.Call("IncTrashID");

			if (_trashId == 1 && _trashCount > 0 && (int)_ui.Call("GetTutorialStep") == 2)
			{
				_ui.Call("NextTutorialStep");
			}
			if (_trashId == 4 && _trashCount > 0 && (int)_ui.Call("GetTutorialStep") == 3)
			{
				_ui.Call("NextTutorialStep");
				_majorObstacle.Call("StartAnimation");
			}
		}
	}





}
