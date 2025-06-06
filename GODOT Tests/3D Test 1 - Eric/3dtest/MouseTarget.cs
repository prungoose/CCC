using Godot;
using System;

public partial class MouseTarget : Node3D
{

	[Export] public Camera3D _camera;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 1000f;
		Vector3? pos = GetGroundIntersection(from, to);

		if (pos != null)
		{
			this.Position = pos.Value;
		}


	}
	
	private Vector3? GetGroundIntersection(Vector3 rayOrigin, Vector3 rayEnd)
	{
    	Plane groundPlane = new Plane(Vector3.Up, 0);
    	Vector3 rayDir = (rayEnd - rayOrigin).Normalized();

		return groundPlane.IntersectsRay(rayOrigin, rayDir);
	}
}
