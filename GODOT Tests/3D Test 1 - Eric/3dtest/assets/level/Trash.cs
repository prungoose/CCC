using Godot;
using System;
using System.Numerics;

public partial class Trash : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.

	bool Selected = false; // when clicked = true

	

	float speedofTrash = 5f; // standard spd of trash
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Selected && Input.IsActionJustPressed("m1"))
		{
			Godot.Vector2 mousePos = GetViewport().GetMousePosition(); // takes the position of the mouse on the screen, not in a 3d space
			Godot.Vector2 positionDiff = new Godot.Vector2(mousePos.X-GlobalPosition.X, mousePos.Y-GlobalPosition.Y);

			LinearVelocity = new Godot.Vector3(positionDiff.X * speedofTrash, positionDiff.Y * speedofTrash, 0f); // grab the mouseposition and set a fixed Z value

		}
		if (Selected && Input.IsActionJustReleased("m1"))
		{
			LinearDamp = 0.1f;// grab the last mouse position when m1 was held down and keep the obj moving in the direction the mouse was headed towards
		}

	}

	public void SelectTrash()
	{
		GD.Print("select trash");
		Selected = true;
	}

	public void DeselectTrash()
	{
		GD.Print("exit trash");
		Selected = false;
	}
}
