using Godot;
using System;
using System.Linq;

public partial class MouseTarget : Node3D
{

	[Export] public Camera3D _camera;
	[Export] public CharacterBody3D _player;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 1000f;
		var result = GetMouseGroundPos(from, to);

		if (result != null)
		{
			this.Position = result.Value;
		}


	}

	private Vector3? GetMouseGroundPos(Vector3 from, Vector3 to)
	{
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollideWithAreas = false,
			CollideWithBodies = true,
			CollisionMask = 1 << 1
		};

		query.Exclude = new Godot.Collections.Array<Rid> { _player.GetRid() };



		var result = space.IntersectRay(query);
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			return (Vector3)result["position"];
		}
		return null;
	}
}
