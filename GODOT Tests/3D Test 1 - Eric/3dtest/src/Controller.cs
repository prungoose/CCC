using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;

public partial class Controller : CharacterBody3D {

	private Godot.Vector3 _velocity = Godot.Vector3.Zero;

	[Export] private float 		_movespeed 	= 10;
	[Export] private float 		_accel 		= 7.5f;
	[Export] private float 		_fric 		= 10;
	[Export] private float 		_pushforce 	= 5f;
	[Export] public Node3D 		_headtarget;
	[Export] public Node3D 		_campivot;
	[Export] public PackedScene _vacuumzone;
	[Export] public PackedScene _thrown_trash;
	[Export] public PackedScene _trajtarget_scene;
	[Export] public Control 	_ui;
	public int 					_tankpercentage = 0;
	private Node3D 				_head;
	private AnimatedSprite3D 	_anim;
	private Area3D 				_vacuum;
	private CsgPolygon3D 		_trajpathmesh;
	private Path3D 				_trajpath;
	private Node3D 				_trajnode;
	private MeshInstance3D 		_trajtarget;
	bool phone 		= false;
	bool is_sucking = false;
	bool is_blowing = false;
	bool is_moving 	= true;
	float 		_throw_strength = 0;
	private int _status = 0;
	private AudioStreamPlayer vacSFX;

	public override void _Ready()
	{
		AddToGroup("player");
		_head = GetNode<Node3D>("Head");
		_anim = GetNode<AnimatedSprite3D>("WorldModel/AnimatedSprite3D");
		_trajpathmesh = GetNode<CsgPolygon3D>("Trajectory/CSGPolygon3D");
		_trajpathmesh.Polygon = _makecirclepolygon();
		_trajpath = GetNode<Path3D>("Trajectory/Path3D");
		_trajnode = GetNode<Node3D>("Trajectory");
		vacSFX = GetNode<AudioStreamPlayer>("SFX");
	}

	public override void _PhysicsProcess(double delta) {
		if (!phone && _status == 0)
			_HandleMovement((float)delta);
		_HandleCollisions((float)delta);

	}

	public override void _Process(double delta) {
		_head.LookAt(_headtarget.GlobalPosition, Godot.Vector3.Up);
		_HandleAnimations();
		_HandleControls((float)delta);
		//GD.Print("fps: ", Engine.GetFramesPerSecond());
	}

	public override void _UnhandledInput(InputEvent @event) {
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && phone) {
			switch (keyEvent.PhysicalKeycode) {
				case Key.W:		_ui.Call("_dial", '↑'); break;
				case Key.S:		_ui.Call("_dial", '↓'); break;
				case Key.A:		_ui.Call("_dial", '←'); break;
				case Key.D:		_ui.Call("_dial", '→'); break;
				case Key.Space:	_ui.Call("_dial", ' '); break;
			}
		}
	}

	private void _HandleMovement(float delta) {
		//get input direction, align it to camera, move that direction

		if (phone) {
			Velocity = Godot.Vector3.Zero;
		} else {
			if (is_sucking | is_blowing)
				_movespeed = 2f;
			else _movespeed = 10f;

			Godot.Vector2 inputdir	= Input.GetVector("move_left", "move_right", "move_up", "move_down");
			Godot.Vector3 dir 		= new Godot.Vector3(inputdir.X, 0, inputdir.Y);
			dir = new Basis(Godot.Vector3.Up, _campivot.Rotation.Y + Mathf.DegToRad(-45)) * dir;
			if (!dir.IsZeroApprox() && !vacSFX.Playing && !is_sucking && !is_blowing)
			{
				vacSFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/wanpeesWalk.wav");
				vacSFX.VolumeDb = -15f;
				vacSFX.PitchScale = 1f;
				vacSFX.Play();
			}
			

			if (dir != Godot.Vector3.Zero)
			{
				_velocity = _velocity.Lerp(dir * _movespeed, (float)delta * _accel);
			}
			else _velocity = _velocity.Lerp(Godot.Vector3.Zero, (float)delta * _fric);
			_velocity.Y -= 100 * (float)delta;
			Velocity = _velocity;
		}
		MoveAndSlide();
	}

	private void _HandleCollisions(float delta) {
		//push away stuff collided with
		for (int i = 0; i < GetSlideCollisionCount(); i++) {
			var col = GetSlideCollision(i);
			if (col.GetCollider() is RigidBody3D rigidbody) {
				Godot.Vector3 pushdir = col.GetNormal() * -1;
				rigidbody.ApplyCentralImpulse(pushdir * _pushforce * delta);
			}
		}
	}

	private void _HandleAnimations() {
		var frame = _anim.Frame;
		var prog = _anim.FrameProgress;
		
		// play the right animation based on player input
		Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_down", "move_up").Normalized();

		String[] dirs = { "sw", "s", "se", "e", "ne", "n", "nw", "w", "sw", "s", "se" };
		if (!phone && (is_sucking | is_blowing))
			_anim.Play(dirs[(int)((Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle())) / 45 + 6.5)] + "_suck");
		else if (!inputdir.IsZeroApprox() && !phone)
			_anim.Play(dirs[(int)Mathf.RadToDeg(inputdir.Angle()) / 45 + 3] + "_run");
		else
			_anim.Play(dirs[(int)((Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle())) / 45 + 6.5)] + "_idle");
		_anim.SetFrameAndProgress(frame, prog);
	}

	private void _HandleControls(float delta) {
		if (_status != 0) return;

		// pull up phone
		if (Input.IsActionJustPressed("phone")) {
			phone = !phone;
			_ui.Call("_updatephone", phone);
			// get rid of vacuum if it is out
			if (_vacuum != null) {
				_vacuum.QueueFree();
				_vacuum = null;
				is_sucking = false;
			}
		}

		if (_vacuum != null)
		{

			// if vacuuming, rotate the vacuum zone in the direction of the mouse
			_vacuum.Rotation = new Godot.Vector3(0, _head.Rotation.Y, 0);
			if (!vacSFX.Playing)
			{
				vacSFX.Stream = GD.Load<AudioStreamMP3>("res://assets/Audios/KirbyInhale.mp3");
				vacSFX.VolumeDb = -25f;
				vacSFX.PitchScale = 0.8f;
				vacSFX.Play();
			}

			//stop vacuuming
			if (Input.IsActionJustReleased("m1"))
			{
				_vacuum.QueueFree();
				_vacuum = null;
				is_sucking = false;
				vacSFX.Stop();
				vacSFX.Stream = GD.Load<AudioStreamMP3>("res://assets/Audios/KirbyStop.mp3");
				vacSFX.VolumeDb = -25f;
				vacSFX.PitchScale = 0.8f;
				vacSFX.Play();
			}
		}
		else if (Input.IsActionJustPressed("m1") && !is_blowing)
		{ // start vacuuming
			_vacuum = _vacuumzone.Instantiate<Area3D>();
			AddChild(_vacuum);
			is_sucking = true;
			vacSFX.Stream = GD.Load<AudioStreamMP3>("res://assets/Audios/KirbyStart.mp3");
			vacSFX.VolumeDb = -25f;
			vacSFX.PitchScale = 0.8f;
			vacSFX.Play();
		}

		// start a throw
		if (Input.IsActionJustPressed("m2") && !is_sucking && _tankpercentage >= 25) {
			is_blowing = true;
			_trajnode.Show();
			if (_trajtarget != null)
				_trajtarget.Show();
		}

		// hold a throw
		if (Input.IsActionPressed("m2") && is_blowing && !is_sucking)
		{
			// one second to max throw strength
			_throw_strength += 150 * delta;
			if (_throw_strength >= 100)
				_throw_strength = 100;
			_drawthrowtrajectory();
			vacSFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/FWOOMP.wav");
			vacSFX.Play();
		}

		//release a throw
		if (Input.IsActionJustReleased("m2") && is_blowing && !is_sucking) {
			_trajnode.Hide();
			var yeet = _thrown_trash.Instantiate<RigidBody3D>();
			GetTree().CurrentScene.AddChild(yeet);
			yeet.GlobalPosition = GlobalPosition + new Godot.Vector3(0, 1, 0);
			var yeetvelocity = new Godot.Vector3(0, 5f, -5f) + (new Godot.Vector3(0, 1, -1) * _throw_strength * _throw_strength / 2500);
			var yeetrotation = new Godot.Vector3(-10, 0, 0);
			yeet.LinearVelocity = yeetvelocity.Rotated(Godot.Vector3.Up, _head.Rotation.Y);
			yeet.AngularVelocity = yeetrotation.Rotated(Godot.Vector3.Up, _head.Rotation.Y);
			is_blowing = false;
			_throw_strength = 0;
			_tankpercentage -= 25;
			_trajpath.Curve.ClearPoints();
			if (_trajtarget != null)
				_trajtarget.Hide();
		}
	}

	private void _addpercent(int value)	{ _tankpercentage += value; }

	private int _gettankpercent()		{ return _tankpercentage; }

	// 0 = normal, 1 = in minigame, 
	private int _getstatus()			{ return _status; }

	private void _changestatus(int s)	{ _status = s; }

	private void _drawthrowtrajectory() {
	
		var start_pos 	= GlobalPosition + new Godot.Vector3(0, 1, 0);
		var start_vel	= (new Godot.Vector3(0, 5f, -5f) + (new Godot.Vector3(0, 1, -1) * _throw_strength * _throw_strength / 2500))
			.Rotated(Godot.Vector3.Up, _head.Rotation.Y) * 0.92f;
		
		Array<Godot.Vector3> result = (Array<Godot.Vector3>)_gettrajectorypoints(start_pos, start_vel)["points"];
		Curve3D curve = new Curve3D();
		foreach (Godot.Vector3 i in result) { curve.AddPoint(i); }
		_trajpath.Curve = curve;
		
		if (_trajtarget == null) {
			_trajtarget = _trajtarget_scene.Instantiate<MeshInstance3D>();
			GetTree().CurrentScene.AddChild(_trajtarget);
		}
		_trajtarget.GlobalPosition = curve.GetPointPosition(curve.PointCount - 1);
		_trajtarget.Show();
	}

	private Dictionary _gettrajectorypoints(Godot.Vector3 start_pos, Godot.Vector3 start_vel) {
		var t_step = 0.05f;
		var g = -(float)ProjectSettings.GetSetting("physics/3d/default_gravity", 9.8);
		var d = -(float)ProjectSettings.GetSetting("physics/3d/default_linear_damp", 0.1);
		Array<Godot.Vector3> pts = new Array<Godot.Vector3> { start_pos };
		var total_len	= 0.0f;
		var cur_pos		= start_pos;
		var cur_vel		= start_vel;
		Dictionary que;
		for (int i = 0; i < 60; i++) {
			var next_pos = cur_pos + cur_vel * t_step;
			cur_vel.Y += g * t_step;
			cur_vel *= Mathf.Clamp(1.0f - d * t_step, 0.0f, 1.0f);

			//prediction hit something
			que = _raycastquery(cur_pos, next_pos);
			if (que.Count != 0) {
				var point = (Godot.Vector3)que["position"];
				pts.Add(point);
				total_len += (point - cur_pos).Length();

				break;
			}

			total_len += (next_pos - cur_pos).Length();
			pts.Add(next_pos);
			cur_pos = next_pos;
		}
		
		Dictionary result = new Dictionary {
			{"points", pts },
			{"length", total_len}
		};
		
		return result;
	}

	private Dictionary _raycastquery(Godot.Vector3 a, Godot.Vector3 b) {
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D {
			From = a,
			To = b,
			CollideWithAreas = false,
			CollideWithBodies = true,
			CollisionMask = 1 << 0
		};
		query.Exclude = new Godot.Collections.Array<Rid> { this.GetRid() };
		query.HitFromInside = false;
		var result = space.IntersectRay(query);
		return result;
	}

	private Godot.Vector2[] _makecirclepolygon() {
		var res = 4;
		var radius = 0.05f;
		var circ = new Godot.Vector2[res];
		for (int i = 0; i < res; i++) {
			var x = radius * Mathf.Sin(Mathf.Pi * 2 * i / res);
			var y = radius * Mathf.Cos(Mathf.Pi * 2 * i / res);
			var coords = new Godot.Vector2(x, y);
			circ[i] = coords;
		}
		return circ;
	}
}
