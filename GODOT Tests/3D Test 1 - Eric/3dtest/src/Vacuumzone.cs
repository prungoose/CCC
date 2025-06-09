using Godot;
using System;
using System.Collections.Generic;

public partial class Vacuumzone : Area3D
{

	private CharacterBody3D _player;
	private List<RigidBody3D> _bodies;
	[Export] private float _deletedistance = 1.5f;



	public override void _Ready()
	{
		_player = GetParent<CharacterBody3D>();
		_bodies = new List<RigidBody3D>();
	}


	public override void _Process(double delta)
	{
		//GD.Print(_bodies.Count);
	}

	public override void _PhysicsProcess(double delta)
	{
		
		for (int i = 0; i < _bodies.Count; i++)
		{
			RigidBody3D body = _bodies[i];
			Vector3 vec = body.GlobalPosition - _player.GlobalPosition;
			if (vec.Length() <= _deletedistance && (int)_player.Call("_gettankpercent") < 100000)
			{
				_bodies.Remove(body);
				body.QueueFree();
				_player.Call("_addpercent", 1);
			}
		}
		
	}

	private void _on_body_entered(Node3D body)
	{
		_bodies.Add((RigidBody3D)body);
	}

	private void _on_body_exited(Node3D body)
	{
		_bodies.Remove((RigidBody3D)body);
	}
	
}
