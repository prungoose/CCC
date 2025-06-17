using Godot;
using System;
using System.Numerics;

public partial class Trash : RigidBody3D
{
	private Node3D _pivot;
	private bool hovered = false;
	private bool dragging = false;
	private bool thrown = false;
	private Godot.Vector3? originalpos;
	private float timesincethrown = 0;
	private float speedofTrash = 35f;
	private float acc = 0;

	public override void _Ready()
	{
		_pivot = GetNode<Node3D>("SpotlightPivot");
	}

	public override void _Process(double delta)
	{
		if (originalpos == null)
		{
			originalpos = GlobalPosition;
		}

		if (thrown)
		{
			timesincethrown += (float)delta;
			if (timesincethrown > 3)
			{
				GlobalPosition = (Godot.Vector3)originalpos;
				LinearVelocity = Godot.Vector3.Zero;
				AngularVelocity = Godot.Vector3.Zero;
				thrown = false;
				timesincethrown = 0;
			}
		}
		_pivot.GlobalPosition = GlobalPosition;

		acc += (float)delta;
		if (acc > 0.5)
		{
			acc = 0;
			//GD.Print(this.Name, ":\thovered: ", hovered, "\t\tdragging: ", dragging, "\t\tthrown: ", thrown, "\t\ttst: ", timesincethrown, "\tgp: ", GlobalPosition);
		}
		
	}

	public override void _Input(InputEvent @event)
	{
		//base._Input(@event);
		if (@event is InputEventMouseButton mouseevent)
		{
			if (mouseevent.ButtonIndex == MouseButton.Left && hovered && @event.IsPressed())
			{
				dragging = true;
			}
			else if (dragging && mouseevent.ButtonIndex == MouseButton.Left && @event.IsReleased())
			{
				dragging = false;
				thrown = true;
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
		Godot.Vector3 from = GetViewport().GetCamera3D().ProjectRayOrigin(mousePos);
		Godot.Vector3 to = from + GetViewport().GetCamera3D().ProjectRayNormal(mousePos) * 1000f;
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D
		{
			From = from,
			To = to,
			CollideWithAreas = true,
			CollideWithBodies = false,
			CollisionMask = 1 << 1
		};

		query.Exclude = new Godot.Collections.Array<Rid> { GetRid() };
		var result = space.IntersectRay(query);
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			return (Godot.Vector3)result["position"];
		}
		return null;
	}
}
