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
	private AnimationPlayer _anim;
	private Area3D _vacuum;
	bool phone = false;
	bool face_left = false;

	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		_anim = GetNode<AnimationPlayer>("WorldModel/Sprite3D/AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{

		_HandleCollisions((float)delta);

	}

	public override void _Process(double delta)
	{
		if (!phone)
		{
			_Movement((float)delta);
		}
		_head.LookAt(_headtarget.GlobalPosition, Godot.Vector3.Forward);
		_HandleAnimations();
		_HandleControls();

	}

	public override void _UnhandledInput(InputEvent @event)
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

	public override void _Input(InputEvent @event)
	{

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
		//play the right animation based on player input
		Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		if (phone)
		{
			_anim.Play("idle");
		}

		else if (inputdir.X != 0)
		{
			if (inputdir.X > 0)
			{
				_anim.Play("walk_right");
				face_left = false;
			}
			else
			{
				_anim.Play("walk_left");
				face_left = true;
			}
		}
		else if (inputdir.Y != 0)
		{
			if (face_left)
			{
				_anim.Play("walk_left");
			}
			else
			{
				_anim.Play("walk_right");
			}
		}

		else
		{
			_anim.Play("idle");
		}
	}

	private void _HandleControls()
	{
		if (Input.IsActionJustPressed("phone"))
		{
			phone = !phone;
			_ui.Call("_updatephone", phone);
			if (_vacuum != null)
			{
				_vacuum.QueueFree();
				_vacuum = null;
			}
		}

		if (phone)
		{
			_velocity = Godot.Vector3.Zero;
		}
		else
		{
			if (Input.IsActionJustPressed("m1"))
			{
				_vacuum = _vacuumzone.Instantiate<Area3D>();
				this.AddChild(_vacuum);
			}
			else if (Input.IsActionJustReleased("m1") && _vacuum != null)
			{
				_vacuum.QueueFree();
				_vacuum = null;
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



}
