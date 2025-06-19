using Godot;
using System;
using System.Net.Http;
using System.Numerics;

public partial class Controller : CharacterBody3D
{

	private Godot.Vector3 _velocity = Godot.Vector3.Zero;

	[Export] private float _movespeed = 10;
	[Export] private float _accel = 7.5f;
	[Export] private float _fric = 10;
	[Export] private float _pushforce = 5f;
	[Export] public Node3D _headtarget;
	[Export] public Node3D _campivot;
	[Export] public PackedScene _vacuumzone;
	[Export] public Control _ui;
	public int _tankpercentage = 0;
	private Node3D _head;
	private AnimatedSprite3D _anim;
	private Area3D _vacuum;
	bool phone = false;
	bool face_left = false;
	bool is_sucking = false;
	bool is_moving = true;
	private int _status = 0;

	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		_anim = GetNode<AnimatedSprite3D>("WorldModel/AnimatedSprite3D");
		AddToGroup("player");
	}

	public override void _PhysicsProcess(double delta)
	{

		_HandleCollisions((float)delta);

	}

	public override void _Process(double delta)
	{

		if (!phone && _status == 0)
		{
			_Movement((float)delta);
		}
		_head.LookAt(_headtarget.GlobalPosition, Godot.Vector3.Up);
		_HandleAnimations();
		_HandleControls();

		//GD.Print("Current vp: ", GetViewport());
		
		

	}

	public override void _UnhandledInput(InputEvent @event)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && phone)
		{
			switch (keyEvent.PhysicalKeycode)
			{
				case Key.W:
					_ui.Call("_dial", '↑');
					break;
				case Key.S:
					_ui.Call("_dial", '↓');
					break;
				case Key.A:
					_ui.Call("_dial", '←');
					break;
				case Key.D:
					_ui.Call("_dial", '→');
					break;
				case Key.Space:
					_ui.Call("_dial", ' ');
					break;
			}
		}
	}

	private void _Movement(float delta)
	{
		//get input direction, align it to camera, move that direction
		Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Godot.Vector3 dir = new Godot.Vector3(inputdir.X, 0, inputdir.Y);
		dir = new Basis(Godot.Vector3.Up, _campivot.Rotation.Y + Mathf.DegToRad(-45)) * dir;

		if (dir != Godot.Vector3.Zero)
		{
			_velocity = _velocity.Lerp(dir * _movespeed, (float)delta * _accel);
		}
		else
		{
			_velocity = _velocity.Lerp(Godot.Vector3.Zero, (float)delta * _fric);
		}
		_velocity.Y -= 100 * (float)delta;
		Velocity = _velocity;
		MoveAndSlide();

	}

	private void _HandleCollisions(float delta)
	{
		//push away stuff collided with
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var col = GetSlideCollision(i);
			if (col.GetCollider() is RigidBody3D rigidbody)
			{
				Godot.Vector3 pushdir = col.GetNormal() * -1;
				rigidbody.ApplyCentralImpulse(pushdir * _pushforce * delta);
			}
		}
	}

	private void _HandleAnimations()
	{
		var frame = _anim.Frame;
		var prog = _anim.FrameProgress;
		//play the right animation based on player input
		Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_down", "move_up").Normalized();

		if (is_sucking && !phone)
		{
			switch (Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle()) + 180)
			{
				case < 22.5f:
					_anim.Play("se_suck");
					break;

				case < 67.5f:
					_anim.Play("e_suck");
					break;

				case < 112.5f:
					_anim.Play("ne_suck");
					break;

				case < 157.5f:
					_anim.Play("n_suck");
					break;

				case < 202.5f:
					_anim.Play("nw_suck");
					break;

				case < 247.5f:
					_anim.Play("w_suck");
					break;

				case < 292.5f:
					_anim.Play("sw_suck");
					break;

				case < 337.5f:
					_anim.Play("s_suck");
					break;

				case < 360f:
					_anim.Play("se_suck");
					break;
			}
		}

		else if (!inputdir.IsZeroApprox() && !phone)
		{
			switch (Mathf.RadToDeg(inputdir.Angle()) + 180)
			{
				case 45:
					_anim.Play("sw_run");
					break;

				case 90:
					_anim.Play("s_run");
					break;

				case 135:
					_anim.Play("se_run");
					break;

				case 180:
					_anim.Play("e_run");
					break;

				case 225:
					_anim.Play("ne_run");
					break;

				case 270:
					_anim.Play("n_run");
					break;

				case 315:
					_anim.Play("nw_run");
					break;

				case 360:
					_anim.Play("w_run");
					break;
			}
		}

		else
		{
			switch (Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle()) + 180)
			{
				case < 22.5f:
					_anim.Play("se_idle");
					break;

				case < 67.5f:
					_anim.Play("e_idle");
					break;

				case < 112.5f:
					_anim.Play("ne_idle");
					break;

				case < 157.5f:
					_anim.Play("n_idle");
					break;

				case < 202.5f:
					_anim.Play("nw_idle");
					break;

				case < 247.5f:
					_anim.Play("w_idle");
					break;

				case < 292.5f:
					_anim.Play("sw_idle");
					break;

				case < 337.5f:
					_anim.Play("s_idle");
					break;

				case < 360f:
					_anim.Play("se_idle");
					break;
			}
		}
		_anim.SetFrameAndProgress(frame, prog);

	}

	private void _HandleControls()
	{
		if (_status != 0) return;

		if (Input.IsActionJustPressed("phone"))
		{
			phone = !phone;
			_ui.Call("_updatephone", phone);
			if (_vacuum != null)
			{
				_vacuum.QueueFree();
				_vacuum = null;
				_movespeed = 10f;
				is_sucking = false;
			}
		}

		if (phone)
		{
			_velocity = Godot.Vector3.Zero;
		}
		else
		{
			if (Input.IsActionJustPressed("m1") && _vacuum == null)
			{
				_vacuum = _vacuumzone.Instantiate<Area3D>();
				AddChild(_vacuum);
				_movespeed = 2f;
				is_sucking = true;
			}
			else if (Input.IsActionJustReleased("m1") && _vacuum != null)
			{
				_vacuum.QueueFree();
				_vacuum = null;
				_movespeed = 10f;
				is_sucking = false;
			}

			if (_vacuum != null)
			{
				_vacuum.Rotation = new Godot.Vector3(0, _head.Rotation.Y, 0);

			}
		}

	}

	private void _addpercent(int value)
	{
		_tankpercentage += value;
	}

	private int _gettankpercent()
	{
		return _tankpercentage;
	}

	private int _getstatus()
	{
		//0 = normal, 1 = in minigame, 
		return _status;
	}

	private void _changestatus(int s)
	{
		_status = s;
	}

}
