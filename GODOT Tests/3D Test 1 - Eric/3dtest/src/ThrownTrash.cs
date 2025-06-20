using Godot;
using System;

public partial class ThrownTrash : RigidBody3D
{
	[Export] private PackedScene _trashscene;
	private Node3D _lightpivot;
	private float radius = 0.5f;

	public override void _Ready()
	{
		_lightpivot = GetNode<Node3D>("SpotlightPivot");
	}

	public override void _Process(double delta)
	{
		_lightpivot.GlobalPosition = GlobalPosition;
	}

	private void _collision(Node body)
	{
		if (body.IsInGroup("ground"))
		{
			for (int i = 0; i < 25; i++)
			{
				_spawntrash();
			}
			this.QueueFree();
		}
		
	}

	void _spawntrash()
	{
        float randomTheta = (float)GD.RandRange(0.0, Mathf.Tau);
        float randomPhi = Mathf.Acos((float)GD.RandRange(-1.0, 1.0));
        float randomRadius = (float)GD.RandRange(0.0, radius);

        float x = randomRadius * Mathf.Sin(randomPhi) * Mathf.Cos(randomTheta);
        float y = randomRadius * Mathf.Sin(randomPhi) * Mathf.Sin(randomTheta);
        float z = randomRadius * Mathf.Cos(randomPhi);

        RigidBody3D newObject = _trashscene.Instantiate<RigidBody3D>();
        newObject.Position = new Vector3(x, y, z) + GlobalPosition;
		newObject.LinearVelocity = LinearVelocity + new Vector3(x, y, z) * 3;
		GetTree().CurrentScene.AddChild(newObject);
	}

}
