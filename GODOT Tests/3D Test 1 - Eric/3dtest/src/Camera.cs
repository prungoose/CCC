using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] public Node3D _target;
	[Export] public float _followspeed = 4f;
	[Export] public float _orbitspeed = 5f;
	private float _targetangle = 0;


	public override void _Ready()
	{
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

}
