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

	int _GetBeaconID()
	{
		return _beacon_id;
	}

	void _DealtWithSomething()
	{
		_primed = true;
		_particles.Emitting = true;
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
