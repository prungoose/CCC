using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;

public partial class Controller : CharacterBody3D
{

	private Godot.Vector3 _velocity = Godot.Vector3.Zero;

	[Export] private float _movespeed = 10;
	[Export] private float _accel = 7.5f;
	[Export] private float _fric = 10;
	[Export] private float _pushforce = 5f;
	[Export] public Node3D _headtarget;
	[Export] public Node3D _campivot;
	[Export] public PackedScene _thrown_trash;
	[Export] public PackedScene _trajtarget_scene;
	[Export] public Control _ui;
	[Export] public PackedScene _thrown_beacon;
	public int _tankpercentage = 0;
	private Node3D _head;
	private AnimatedSprite3D _anim;
	private Area3D _vacuum;
	private CsgPolygon3D _trajpathmesh;
	private Path3D _trajpath;
	private Node3D _trajnode;
	private MeshInstance3D _trajtarget;
	private Node3D _lightpivot;
	private ShapeCast3D _stepray;
	private Node3D _stepraypivot;
	bool phone = false;
	bool is_sucking = false;
	bool is_blowing = false;
	bool is_moving = true;
	bool beacon_ready = false;
	float _throw_strength = 0;
	private int _status = 0;
	private AudioStreamPlayer VacSFX;
	private AudioStreamPlayer VacLoopSFX;
	private AudioStreamPlayer WalkSFX;
	private AudioStreamPlayer FWOOMPSFX;
	[Export] private AudioStreamPlayer windUpSFX;
	private int _beacon_id = 0;

	private int _thrown_id = 0;
	private int _tank1 = 0;
	private int _tank2 = 0;
	private int _tank3 = 0;
	private int _tank4 = 0;

	public override void _Ready()
	{
		AddToGroup("player");
		_head = GetNode<Node3D>("Head");
		_anim = GetNode<AnimatedSprite3D>("WorldModel/AnimatedSprite3D");
		_vacuum = GetNode<Area3D>("Vacuum");
		_trajpathmesh = GetNode<CsgPolygon3D>("Trajectory/CSGPolygon3D");
		_trajpathmesh.Polygon = _makecirclepolygon();
		_trajpath = GetNode<Path3D>("Trajectory/Path3D");
		_trajnode = GetNode<Node3D>("Trajectory");
		_lightpivot = GetNode<Node3D>("LightPivot");
		_stepray = GetNode<ShapeCast3D>("StepRayPivot/StepRay");
		_stepraypivot = GetNode<Node3D>("StepRayPivot");
		VacSFX = GetNode<AudioStreamPlayer>("Sounds/VacSFX");
		VacLoopSFX = GetNode<AudioStreamPlayer>("Sounds/VacLoopSFX");
		WalkSFX = GetNode<AudioStreamPlayer>("Sounds/WalkSFX");
		FWOOMPSFX = GetNode<AudioStreamPlayer>("Sounds/FWOOMPSFX");
	}

	public override void _PhysicsProcess(double delta)
	{

		_head.LookAt(_headtarget.GlobalPosition, Godot.Vector3.Up);
		Godot.Vector3 y_rotate = new Godot.Vector3(0, _head.Rotation.Y, 0);
		_vacuum.Rotation = y_rotate;
		_lightpivot.Rotation = y_rotate;
		if (!phone && _status == 0)
			_HandleMovement((float)delta);
		_HandleCollisions((float)delta);
		_HandleControls((float)delta);

	}

	public override void _Process(double delta)
	{

		_HandleAnimations();
		//GD.Print("fps: ", Engine.GetFramesPerSecond());
	}

	public override void _UnhandledInput(InputEvent @event)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (phone)
			{
				switch (keyEvent.PhysicalKeycode)
				{
					case Key.W: _ui.Call("_dial", '↑'); break;
					case Key.S: _ui.Call("_dial", '↓'); break;
					case Key.A: _ui.Call("_dial", '←'); break;
					case Key.D: _ui.Call("_dial", '→'); break;
					case Key.Space: _ui.Call("_dial", ' '); break;
				}
			}
			else
			{
				switch (keyEvent.PhysicalKeycode)
				{
					case Key.Key1: SwitchThrown(1); break;
					case Key.Key2: SwitchThrown(2); break;
					case Key.Key3: SwitchThrown(3); break;
					case Key.Key4: SwitchThrown(4); break;
				}
			}

		}

	}

	private void _HandleMovement(float delta)
	{
		//get input direction, align it to camera, move that direction

		if (phone)
		{
			Velocity = Godot.Vector3.Zero;
		}
		else
		{
			if (is_sucking | is_blowing)
				_movespeed = 2f;
			else _movespeed = 10f;

			Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			Godot.Vector3 dir = new Godot.Vector3(inputdir.X, 0, inputdir.Y);
			dir = new Basis(Godot.Vector3.Up, _campivot.Rotation.Y + Mathf.DegToRad(-45)) * dir;
			if (!dir.IsZeroApprox() && !WalkSFX.Playing && !is_sucking && !is_blowing)
			{
				WalkSFX.VolumeDb = -15f;
				WalkSFX.PitchScale = 1f;
				WalkSFX.Play();
			}


			if (dir != Godot.Vector3.Zero)
			{
				_velocity = _velocity.Lerp(dir * _movespeed, (float)delta * _accel);
				_stepraypivot.Rotation = new Godot.Vector3(0, Godot.Vector3.Forward.SignedAngleTo(dir, Godot.Vector3.Up), 0);
				if (_stepray.IsColliding())
				{

					var height = (_stepray.GetCollisionPoint(0)- GlobalPosition).Y;
					GlobalTranslate(new Godot.Vector3(0, height, 0));
				}
			}
			else _velocity = _velocity.Lerp(Godot.Vector3.Zero, (float)delta * _fric);
			_velocity.Y -= 100 * (float)delta;
			Velocity = _velocity;
		}


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
		int frame;
		float prog;

		// play the right animation based on player input
		Godot.Vector2 inputdir = Input.GetVector("move_left", "move_right", "move_down", "move_up").Normalized();
		if (!inputdir.IsZeroApprox() | is_sucking | is_blowing)
		{
			frame = _anim.Frame;
			prog = _anim.FrameProgress;
		}
		else
		{
			frame = 0;
			prog = 0;
		}

		String[] dirs = { "sw", "s", "se", "e", "ne", "n", "nw", "w", "sw", "s", "se" };

		if (!phone && (is_sucking | is_blowing))
		{
			_anim.Play(dirs[(int)((Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle())) / 45 + 6.5)] + "_suck");
		}
		else if (!inputdir.IsZeroApprox() && !phone)
			_anim.Play(dirs[(int)Mathf.RadToDeg(inputdir.Angle()) / 45 + 3] + "_run");
		else
			_anim.Play(dirs[(int)((Mathf.RadToDeg(Godot.Vector2.FromAngle(_head.Rotation.Y - _campivot.Rotation.Y).Angle())) / 45 + 6.5)] + "_idle");
			
		_anim.SetFrameAndProgress(frame, prog);
	}

	private void _HandleControls(float delta)
	{
		if (_status != 0) return;

		// pull up phone
		if (Input.IsActionJustPressed("phone") && !is_blowing && !is_sucking)
		{
			_campivot.Call("_ToggleZoom");
			phone = !phone;
			_ui.Call("_updatephone", phone);
			// get rid of vacuum if it is out
			if (is_sucking)
			{
				_vacuum.Call("_Deactivate");
				is_sucking = false;
			}
		}

		if (is_sucking)
		{
			// if vacuuming, rotate the vacuum zone in the direction of the mouse

			if (!VacLoopSFX.Playing)
			{
				VacLoopSFX.Play();
			}

			//stop vacuuming
			if (Input.IsActionJustReleased("m1"))
			{
				_vacuum.Call("_Deactivate");
				is_sucking = false;
				VacLoopSFX.Stop();
				VacSFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/VacuumStop.wav");
				VacSFX.Play();
			}
		}
		else if (Input.IsActionJustPressed("m1") && !is_blowing && !phone)
		{ // start vacuuming
			_vacuum.Call("_Activate");
			is_sucking = true;
			VacSFX.Stream = GD.Load<AudioStreamWav>("res://assets/Audios/VacuumStart.wav");
			VacSFX.Play();
		}

		// start a throw
		if (Input.IsActionJustPressed("m2") && !is_sucking && (_GetTankPercentage(_thrown_id) >= 20 | beacon_ready) && !phone)
		{
			windUpSFX.Play();
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

		}

		//release a throw
		if (Input.IsActionJustReleased("m2") && is_blowing && !is_sucking)
		{
			windUpSFX.Stop();
			_trajnode.Hide();

			RigidBody3D yeet;
			if (beacon_ready)
			{
				yeet = _thrown_beacon.Instantiate<RigidBody3D>();
				yeet.Call("SetBeaconID", _beacon_id);
				beacon_ready = false;
			}
			else
			{
				yeet = _thrown_trash.Instantiate<RigidBody3D>();
				_Remove20FromTankPercentage(_thrown_id);
			}

			GetTree().CurrentScene.AddChild(yeet);
			yeet.GlobalPosition = GlobalPosition + new Godot.Vector3(0, 1, 0);
			var yeetvelocity = new Godot.Vector3(0, 3.5f, -3.5f) + (new Godot.Vector3(0, 1, -1) * _throw_strength * _throw_strength / 2500);
			var yeetrotation = new Godot.Vector3(-5, 0, 0);
			yeet.LinearVelocity = yeetvelocity.Rotated(Godot.Vector3.Up, _head.Rotation.Y);
			yeet.AngularVelocity = yeetrotation.Rotated(Godot.Vector3.Up, _head.Rotation.Y);
			is_blowing = false;
			_throw_strength = 0;
			_trajpath.Curve.ClearPoints();
			if (_trajtarget != null)
				_trajtarget.Hide();
			FWOOMPSFX.Play();
		}
	}

	private void _IncTank(int tank_no)
	{
		switch (tank_no)
		{
			case 1: _tank1++; break;
			case 2: _tank2++; break;
			case 3: _tank3++; break;
			case 4: _tank4++; break;
		}
	}

	private int _GetTankPercentage(int tank_no)
	{
		switch (tank_no)
		{
			case 1: return _tank1;
			case 2: return _tank2;
			case 3: return _tank3;
			case 4: return _tank4;
		}
		return 0;
	}

	private void _Remove20FromTankPercentage(int tank_no)
	{
		switch (tank_no)
		{
			case 1: _tank1 -= 20; break;
			case 2: _tank2 -= 20; break;
			case 3: _tank3 -= 20; break;
			case 4: _tank4 -= 20; break;
		}
	}

	private void _drawthrowtrajectory()
	{

		var start_pos = GlobalPosition + new Godot.Vector3(0, 1, 0);
		var start_vel = (new Godot.Vector3(0, 3.5f, -3.5f) + (new Godot.Vector3(0, 1, -1) * _throw_strength * _throw_strength / 2500))
			.Rotated(Godot.Vector3.Up, _head.Rotation.Y) * 0.92f;

		Array<Godot.Vector3> result = (Array<Godot.Vector3>)_gettrajectorypoints(start_pos, start_vel)["points"];
		Curve3D curve = new Curve3D();
		foreach (Godot.Vector3 i in result) { curve.AddPoint(i); }
		_trajpath.Curve = curve;


		if (_trajtarget != null)
		{
			_trajtarget.QueueFree();
			_trajtarget = null;
		}
		_trajtarget = _trajtarget_scene.Instantiate<MeshInstance3D>();
		GetTree().CurrentScene.AddChild(_trajtarget);
		_trajtarget.GlobalPosition = curve.GetPointPosition(curve.PointCount - 1);
		_trajtarget.Show();
	}

	private Dictionary _gettrajectorypoints(Godot.Vector3 start_pos, Godot.Vector3 start_vel)
	{
		var t_step = 0.05f;
		var g = -(float)ProjectSettings.GetSetting("physics/3d/default_gravity", 9.8);
		var d = -(float)ProjectSettings.GetSetting("physics/3d/default_linear_damp", 0.1);
		Array<Godot.Vector3> pts = new Array<Godot.Vector3> { start_pos };
		var total_len = 0.0f;
		var cur_pos = start_pos;
		var cur_vel = start_vel;
		Dictionary que;
		for (int i = 0; i < 60; i++)
		{
			var next_pos = cur_pos + cur_vel * t_step;
			cur_vel.Y += g * t_step;
			cur_vel *= Mathf.Clamp(1.0f - d * t_step, 0.0f, 1.0f);

			//prediction hit something
			que = _raycastquery(cur_pos, next_pos);
			if (que.Count != 0)
			{
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

	private Dictionary _raycastquery(Godot.Vector3 a, Godot.Vector3 b)
	{
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
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

	private Godot.Vector2[] _makecirclepolygon()
	{
		var res = 4;
		var radius = 0.05f;
		var circ = new Godot.Vector2[res];
		for (int i = 0; i < res; i++)
		{
			var x = radius * Mathf.Sin(Mathf.Pi * 2 * i / res);
			var y = radius * Mathf.Cos(Mathf.Pi * 2 * i / res);
			var coords = new Godot.Vector2(x, y);
			circ[i] = coords;
		}
		return circ;
	}

	private void ReadyBeacon(int id)
	{
		beacon_ready = true;
		_beacon_id = id;
	}

	private void SwitchThrown(int id)
	{
		if (id == _thrown_id) _thrown_id = 0;
		else if (!(is_blowing && _GetTankPercentage(id) < 20)) _thrown_id = id;
		_ui.Call("_UpdateThrown", _thrown_id);
	}

	private int GetCurrentThrowing()
	{
		return _thrown_id;
	}

}
