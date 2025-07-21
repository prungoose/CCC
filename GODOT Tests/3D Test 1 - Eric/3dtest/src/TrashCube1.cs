using Godot;
using System;

public partial class TrashCube1 : RigidBody3D
{
	//1: Combustibles 2: Plastic 3: PET 4: Cans/Almn
	int _trash_id = 0;
	private float _time_active = 0;
	private Sprite3D _sprite;
	private CharacterBody3D _player;
	private Sprite3D _minimapsprite;
	private SpringArm3D _springarm;
	private Timer _timer;

	private float _player_dis;



	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_minimapsprite = GetNode<Sprite3D>("MapSprite3D");
		_springarm = GetNode<SpringArm3D>("SpringArm3D");
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += _TimerTimeout;
		_timer.WaitTime += GD.Randf() * 5;

		var r = GD.RandRange(0, 1);
		if (r > 0) _sprite.FlipH = true;

	}

	public override void _Process(double delta)
	{
		_time_active += (float)delta;

	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player_dis > 40)
		{
			Sleeping = true;
		}
		else
		{
			Sleeping = false;
			_springarm.GlobalRotation = new Godot.Vector3(Mathf.DegToRad(90), 0, 0);
			_minimapsprite.GlobalPosition = GlobalPosition;
		}

	}

	public int _GetTrashID()
	{
		return _trash_id;
	}

	public void _ChangeToType(int id)
	{
		if (id == 0) return;
		_trash_id = id;
		switch (id)
		{
			case 1: _minimapsprite.Modulate = new Color(.99f, .39f, .32f, 1f); id += 2; break; //red, id += 2 cause red/yellow columns are switched on the spritesheet
			case 2: _minimapsprite.Modulate = new Color(0f, .75f, .15f, 1f); break; //green
			case 3: _minimapsprite.Modulate = new Color(.99f, .73f, 0f, 1f); id -= 2; break; //yellow, id -= 2 cause of above reason
			case 4: _minimapsprite.Modulate = new Color(0f, .67f, .89f, 1f); break; //blue
		}
		var r = GD.RandRange(0, 1);
		if (r > 0) id += 4;
		_sprite.Frame = id - 1;
	}


	private void _TimerTimeout()
	{
		_player_dis = GlobalPosition.DistanceTo(_player.GlobalPosition);
				if (_player_dis > 40)
		{
			_sprite.Hide();
			_minimapsprite.Hide();
		}
		else
		{
			_sprite.Show();
			_minimapsprite.Show();
		}

		if ((GlobalPosition.Y < -40 | _time_active > 60) && _player_dis > 40)
		{
			QueueFree();
		}
	}


}
