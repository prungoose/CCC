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
		if (_target == null) GD.Print("target null");
	}

	public override void _Process(double delta)
	{
		GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin.Lerp(_target.GlobalTransform.Origin, (float)delta * _followspeed));
		_SmoothOrbit(delta);
	
	}

	void _SmoothOrbit(double delta)
	{
		float input = 0;
		if (Input.IsActionPressed("orbit_left"))
			input += 1;
		if (Input.IsActionPressed("orbit_right"))
			input -= 1;

		if (input != 0)
		{
			_targetangle += .015f * input;
		}

		float _angle = Mathf.LerpAngle(this.Rotation.Y, _targetangle, _orbitspeed * (float)delta);
		this.Rotation = new Godot.Vector3(0, _angle, 0);
		
	}

}
