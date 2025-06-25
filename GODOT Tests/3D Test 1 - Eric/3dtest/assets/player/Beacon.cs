using Godot;
using System;
using System.Threading.Tasks;

public partial class Beacon : RigidBody3D
{
	GpuParticles3D _particles;
	int _beacon_id;
	private Timer _flaretimer;

	public override void _Ready()
	{
		_particles = GetNode<GpuParticles3D>("GPUParticles3D");
		_flaretimer = new Timer();
		_flaretimer.WaitTime = 8f;
		_flaretimer.OneShot = true;
		_flaretimer.Timeout += TimerEnd;
		AddChild(_flaretimer);
	}

	public override void _Process(double delta)
	{
		_particles.GlobalPosition = GlobalPosition;
	}

	void SetBeaconID(int id)
	{
		_beacon_id = id;
	}

	void _body_entered(Node body)
	{
		if (body.IsInGroup("major_obstacle"))
		{
			body.Call("DealWith", _beacon_id);
			Freeze = true;
			_particles.Emitting = true;
		}
		_flaretimer.Start();
	}

	void TimerEnd() {
		QueueFree();
	}
}
