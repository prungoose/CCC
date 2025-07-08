using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] public Node3D _target;
	[Export] public float _followspeed = 4f;
	[Export] public float _orbitspeed = 5f;
	private Camera3D _cam;
	private float _targetangle = 0;
	private bool _zoom = false;


	public override void _Ready()
	{
		_cam = GetNode<Camera3D>("Camera3D");
	}

	public override void _Process(double delta)
	{
		GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin.Lerp(_target.GlobalTransform.Origin, _followspeed * (float)delta));
		_SmoothOrbit(delta);

	}

	void _SmoothOrbit(double delta)
	{
		//smoothly orbit in 45 degree increments
		if (Input.IsActionJustPressed("orbit_left"))
			_targetangle += Mathf.DegToRad(45);
		if (Input.IsActionJustPressed("orbit_right"))
			_targetangle -= Mathf.DegToRad(45);

		float _angle = Mathf.LerpAngle(this.Rotation.Y, _targetangle, _orbitspeed * (float)delta);
		this.Rotation = new Godot.Vector3(0, _angle, 0);

	}

	void _ToggleZoom()
	{
		_zoom = !_zoom;
		var tween = GetTree().CreateTween();
		float fov_target = 16f;
		float h_offset_target = 4f;
		if (!_zoom)
		{
			fov_target = 22.5f;
			h_offset_target = 0f;
		}
		tween.SetParallel();
		tween.TweenProperty(_cam, "fov", fov_target, .6f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(_cam, "h_offset", h_offset_target, 1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
	}

}
