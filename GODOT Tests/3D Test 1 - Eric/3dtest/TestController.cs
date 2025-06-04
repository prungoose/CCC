using Godot;
using System;
using System.Numerics;
using System.Runtime;

public partial class TestController : CharacterBody3D
{
	public const float Speed = 15.0f;
	public const float JumpVelocity = 4.5f;
	private const float LookSensitivity = 0.5f;

	private float _cameraAngle = 0f;

	private Node3D _head;
	private Node3D _body;
	private Camera3D _camera;

	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		_camera = GetNode<Camera3D>("Head/Camera3D");
		_body = GetNode<Node3D>("WorldModel");
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//mouse inputs
		if (@event is InputEventMouseButton eventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}

		//key inputs
		else if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}


	}

	public override void _Input(InputEvent @event)
	{
		//mouselook
		if (@event is not InputEventMouseMotion motion) return;
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			_body.RotateY(Mathf.DegToRad(-motion.Relative.X * LookSensitivity));
			_head.RotateY(Mathf.DegToRad(-motion.Relative.X * LookSensitivity));
			_camera.RotateX(Mathf.DegToRad(-motion.Relative.Y * LookSensitivity));
			_camera.Rotation = new Godot.Vector3(Mathf.Clamp(_camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)), _camera.Rotation.Y, _camera.Rotation.Z);
		}
	}

	private void Movement()
	{
		var input_dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Godot.Vector3 direction = new();
		//Basis aim = _camera.GlobalTransform.Basis;

		var aim = _camera.GlobalTransform.Basis;


		if (Input.IsActionPressed("move_up")) direction -= aim.Z;
		if (Input.IsActionPressed("move_left")) direction -= aim.X;
		if (Input.IsActionPressed("move_down")) direction += aim.Z;
		if (Input.IsActionPressed("move_right")) direction += aim.X;

		Velocity = direction.Normalized() * Speed;
		MoveAndSlide();
	}


}
