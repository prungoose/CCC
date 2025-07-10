using Godot;
using System;
using System.Threading.Tasks;

public partial class Beacon : RigidBody3D
{
	GpuParticles3D _particles;
	int _beacon_id = 0;
	private Timer _flaretimer;
	private bool _timer_phase = false;
	private bool _primed = false;

	public override void _Ready()
	{
		_particles = GetNode<GpuParticles3D>("GPUParticles3D");
		_flaretimer = new Timer();
		_flaretimer.Autostart = true;
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
		if (body.IsInGroup("major_obstacle") && !(bool)body.Call("GetDealtWithStatus"))
		{
			body.Call("DealWith", _beacon_id);
			if ((bool)body.Call("GetDealtWithStatus"))
			{
				_primed = true;
				_particles.Emitting = true;

			}
		}
		else if (body.IsInGroup("ground") && _primed)
		{
			Freeze = true;
		}
	}

	void TimerEnd()
	{
		if (!_primed | _timer_phase)
		{
			QueueFree();
		}
		else
		{
			_timer_phase = true;
			_particles.Emitting = false;
			_flaretimer.WaitTime = 3.5f;
			_flaretimer.Start();
		}

	}
	

}
