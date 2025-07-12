using Godot;
using System;
using System.Collections.Generic;

public partial class Vacuumzone : Area3D
{
	[Export] private float _deletedistance = 1.5f;
	private CharacterBody3D _player;
	private List<RigidBody3D> _bodies;
	private Area3D _suckhitbox;
	private GpuParticles3D _particles1;
	private GpuParticles3D _particles2;
	private AudioStreamPlayer pickupSFX;
	private bool _active = false;
	private float _time_active = 0;

	public override void _Ready()
	{
		_player = GetParent<CharacterBody3D>();
		_bodies = new List<RigidBody3D>();
		_suckhitbox = GetNode<Area3D>("suck");
		_particles1 = GetNode<GpuParticles3D>("GPUParticles3D");
		_particles2 = GetNode<GpuParticles3D>("GPUParticles3D2");
		pickupSFX = GetNode<AudioStreamPlayer>("SFX");
		pickupSFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/Pop.wav");
	}


	public override void _Process(double delta)
	{
		if(_active) _time_active += (float)delta;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_active) return;

		for (int i = 0; i < _bodies.Count; i++)
		{
			RigidBody3D body = _bodies[i];
			if (body.IsInGroup("cleanable_vacuum"))
			{
				body.ApplyCentralForce(Godot.Vector3.Up * 0.001f); //to wkae the body up
				int trash_id = (int)_bodies[i].Call("_GetTrashID");
				int playerpercentage = (int)_player.Call("_GetTankPercentage", trash_id);
				if (playerpercentage < 40 && _time_active > 0.2)
				{
					body.CollisionLayer = 0;
					_player.Call("_IncTank", trash_id);
					var tween = GetTree().CreateTween();
					tween.TweenProperty(body, "global_position", _player.GlobalPosition + Vector3.Up * 0.5f, .13f).SetTrans(Tween.TransitionType.Quad);
					tween.Finished += () => body.QueueFree();
					_PlaySuckSFX();
					_bodies.Remove(body);
				}
			}

		}

	}

	private void _suck_entered(Node3D body)
	{
		if (body.IsInGroup("cleanable_vacuum"))
		{
			_bodies.Add((RigidBody3D)body);
		}
	}

	private void _suck_exited(Node3D body)
	{
		if (body.IsInGroup("cleanable_vacuum"))
		{
			_bodies.Remove((RigidBody3D)body);
		}
	}

	private void _PlaySuckSFX()
	{
		pickupSFX.PitchScale = (float)GD.RandRange(0.7, 1.0);
		pickupSFX.Play();
	}

	private void _Activate()
	{
		_active = true;
		Gravity = 30;
		_particles1.Emitting = true;
		_particles2.Emitting = true;
	}

	private void _Deactivate()
	{
		_active = false;
		_time_active = 0;
		Gravity = 0;
		_particles1.Emitting = false;
		_particles2.Emitting = false;
	}
}
