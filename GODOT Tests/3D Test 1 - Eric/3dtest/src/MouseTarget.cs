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
		var result = GetMouseGroundPos();
		if (result != null)
		{
			this.Position = result.Value;
		}


	}

	private Vector3? GetMouseGroundPos()
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
			CollisionMask = 1 << 1
		};

		query.Exclude = new Godot.Collections.Array<Rid> { _player.GetRid() };
		var result = space.IntersectRay(query);
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			//GD.Print(result);
			return (Vector3)result["position"];
		}
		return null;
	}
}
