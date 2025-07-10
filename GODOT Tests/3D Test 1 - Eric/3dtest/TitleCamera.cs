using Godot;
using System;

public partial class TitleCamera : Camera3D
{

	private Node3D _anchor;
	private Node3D _camtarget;
	private RigidBody3D _camtargetbody;
	private float _followspeed = 2.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anchor = GetTree().CurrentScene.GetNode<Node3D>("MouseStuff/Anchor");
		_camtarget = GetTree().CurrentScene.GetNode<Node3D>("MouseStuff/CamTarget");
		_camtargetbody = GetTree().CurrentScene.GetNode<RigidBody3D>("MouseStuff/CamTargetBody");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		var pos = GetMouseCollidePos();
		if (pos != null)
		{
			_camtarget.GlobalPosition = _anchor.Position.Lerp((Godot.Vector3)pos, .25f);
			_camtargetbody.LinearVelocity = (_camtarget.GlobalPosition - _camtargetbody.GlobalPosition) * _followspeed;
			LookAt(_camtargetbody.GlobalPosition);
		}
	}

	private Vector3? GetMouseCollidePos()
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = GetViewport().GetCamera3D().ProjectRayOrigin(mousePos);
		Vector3 to = from + GetViewport().GetCamera3D().ProjectRayNormal(mousePos) * 1000f;
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollideWithAreas = false,
			CollideWithBodies = true,
			CollisionMask = 1 << 0
		};
		var result = space.IntersectRay(query);
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			//GD.Print(result);
			return (Vector3)result["position"];
		}
		return null;
	}
}
