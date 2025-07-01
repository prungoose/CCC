using Godot;
using System;
using System.Threading.Tasks;

public partial class Beacon : RigidBody3D
{
	GpuParticles3D _particles;
	int _beacon_id = 0;
	private Timer _flaretimer;
	private bool timer_phase = false;

	public override void _Ready()
	{
		_particles = GetNode<GpuParticles3D>("GPUParticles3D");
		_flaretimer = new Timer();
		_flaretimer.WaitTime = 20f;
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
			GD.Print("tried dealing with ", body.Name, " using id ", _beacon_id);
			body.Call("DealWith", _beacon_id);
			Freeze = true;
			_particles.Emitting = true;
		}
		_flaretimer.Start();
	}

	void TimerEnd()
	{
		if (!timer_phase)
		{
			timer_phase = true;
			_particles.Emitting = false;
			_flaretimer.WaitTime = 3.5f;
			_flaretimer.Start();
		}
		else
		{
			QueueFree();
		}
	}
	

}
