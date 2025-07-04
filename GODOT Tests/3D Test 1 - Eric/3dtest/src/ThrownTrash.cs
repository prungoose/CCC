using Godot;
using System;

public partial class ThrownTrash : RigidBody3D
{
	[Export] private PackedScene _trashscene;
	private CharacterBody3D _player;
	private int _bounces;
	private int _id;

	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_id = (int)_player.Call("GetCurrentThrowing");
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
		_bounces++;
		if (_bounces >= 2)
		{
			for (int i = 0; i < 20; i++)
			{
				_spawntrash();
			}
			QueueFree();
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

	private int GetThrownTrashID(){
		return _id;
	}
}
