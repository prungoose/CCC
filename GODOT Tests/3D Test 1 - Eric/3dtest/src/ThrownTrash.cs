using Godot;
using System;

public partial class ThrownTrash : RigidBody3D
{
	[Export] private PackedScene _trashscene;
	private Node3D _lightpivot;
	private CharacterBody3D _player;
	private int _bounces;

	public override void _Ready()
	{
		_lightpivot = GetNode<Node3D>("SpotlightPivot");
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
	}

	public override void _Process(double delta)
	{
		_lightpivot.GlobalPosition = GlobalPosition;
	}

	private void _collision(Node body)
	{
		if (body.IsInGroup("ground"))
		{
			_bounces++;
			if (_bounces >= 2)
			{
				for (int i = 0; i < 25; i++)
				{
					_spawntrash();
				}
				QueueFree();
			}

		}

	}

	void _spawntrash()
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
	}

	private int GetThrownTrashID()
	{
		return (int)_player.Call("GetCurrentTrashID");
	}

}
