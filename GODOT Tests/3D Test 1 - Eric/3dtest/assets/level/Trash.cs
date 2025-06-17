using Godot;
using System;
using System.Numerics;

public partial class Trash : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.

	[Export] Camera3D _camera;
	bool hovered = false; // when clicked = true
	bool dragging = false;
	bool thrown = false;
	Godot.Vector3 originalpos;
	float timesincethrown = 0;
	float speedofTrash = 100f;

	public override void _Ready()
	{
		originalpos = GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if (thrown)
		{
			timesincethrown += (float)delta;
		}
		if (timesincethrown > 3)
		{
			GlobalPosition = originalpos;
			LinearVelocity = Godot.Vector3.Zero;
			AngularVelocity = Godot.Vector3.Zero;
			thrown = false;
			timesincethrown = 0;
		}

	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseButton mouseevent)
		{
			if (mouseevent.ButtonIndex == MouseButton.Left && hovered && @event.IsPressed())
			{
				dragging = true;
			}
			else if (mouseevent.ButtonIndex == MouseButton.Left && @event.IsReleased())
			{
				dragging = false;
				thrown = true;
				//apply some upwards velocity here maybe?
			}
		}

	}

	public override void _PhysicsProcess(double delta)
	{
		//base._PhysicsProcess(delta);
		if (dragging && !thrown)
		{
			var result = GetArea3DMousePos();
			if (result != null)
			{
				LinearVelocity = ((Godot.Vector3)result - GlobalPosition) * speedofTrash;
			}
			else
			{
				dragging = false;
				thrown = true;
			}
		}
		//Godot.Vector3 temp = LinearVelocity;
		//temp.Y -= (float)GetGravity().Y;
	}



	public void SelectTrash()
	{
		hovered = true;
	}

	public void DeselectTrash()
	{
		hovered = false;
	}
	
	private Godot.Vector3? GetArea3DMousePos()
	{
		Godot.Vector2 mousePos = GetViewport().GetMousePosition();
		Godot.Vector3 from = _camera.ProjectRayOrigin(mousePos);
		Godot.Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 1000f;
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollideWithAreas = true,
			CollideWithBodies = false,
			CollisionMask = 1 << 1
		};

		query.Exclude = new Godot.Collections.Array<Rid> { this.GetRid() };
		var result = space.IntersectRay(query);
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			//GD.Print(result);
			return (Godot.Vector3)result["position"];
		}
		return null;
	}
}
