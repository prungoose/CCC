using Godot;
using System;
using System.Threading.Tasks;

public partial class ThrownTrash : RigidBody3D
{
	[Export] private PackedScene _trashscene;
	private CharacterBody3D _player;
	private Sprite3D _sprite;
	private int _bounces;
	private int _id;
	private AudioStreamPlayer3D boingSFX;
	private AudioStreamPlayer3D splodeSFX;
	private bool _isSpawnedPiece = false;


	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		if (!_isSpawnedPiece)
		{
			_id = (int)_player.Call("GetCurrentThrowing");
		}
		_sprite = GetNode<Sprite3D>("Sprite3D");

		switch (_id)
		{
			case 1: _sprite.Frame = 2; break;
			case 2: _sprite.Frame = 1; break;
			case 3: _sprite.Frame = 0; break;
			case 4: _sprite.Frame = 3; break;
		}
		boingSFX = GetNode<AudioStreamPlayer3D>("BoingSFX");
		splodeSFX = GetNode<AudioStreamPlayer3D>("SplodeSFX");
	}

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		GetNode<SpringArm3D>("SpringArm3D").GlobalRotation = new Godot.Vector3(Mathf.DegToRad(90), 0, 0);
	}

	private void _collision(Node body)
	{
		if (body.IsInGroup("cleanable_vacuum")) return;
		_bounces++;
		if (_bounces == 1) boingSFX.Play();
		else if (_bounces == 2)
		{
			splodeSFX.Play();
			_sprite.Hide();
			CollisionLayer = 0;
			CollisionMask = 0;
			Freeze = true;
			GetNode<SpringArm3D>("SpringArm3D").Hide();
			for (int i = 0; i < 20; i++) _spawntrash();
			splodeSFX.Finished += () => QueueFree();
		}

	}

	private void _spawntrash()
	{
		float radius = 0.5f;
		float randomTheta = (float)GD.RandRange(0.0, Mathf.Tau);
		float randomPhi = Mathf.Acos((float)GD.RandRange(-1.0, 1.0));
		float randomRadius = (float)GD.RandRange(0.0, radius);

		float x = randomRadius * Mathf.Sin(randomPhi) * Mathf.Cos(randomTheta);
		float y = randomRadius * Mathf.Sin(randomPhi) * Mathf.Sin(randomTheta);
		float z = randomRadius * Mathf.Cos(randomPhi);

		RigidBody3D newObject = _trashscene.Instantiate<RigidBody3D>();
		newObject.Position = new Vector3(x, y, z) + GlobalPosition;
		newObject.LinearVelocity = LinearVelocity + new Vector3(x, y, z);
		GetTree().CurrentScene.AddChild(newObject);
		newObject.Call("_ChangeToType", _id);
	}

	private int GetThrownTrashID()
	{
		return _id;
	}

	public void _ChangeToType(int id)
	{
		_id = id;
		_isSpawnedPiece = true;
		if (_sprite == null)
		{
			_sprite = GetNode<Sprite3D>("Sprite3D");
		}

		switch (_id)
		{
			case 1: _sprite.Frame = 2; break;
			case 2: _sprite.Frame = 1; break;
			case 3: _sprite.Frame = 0; break;
			case 4: _sprite.Frame = 3; break;
			default: GD.PrintErr("Invalid trash type ID: " + _id); break;
		}
	}
}
