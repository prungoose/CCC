using Godot;
using System;

public partial class MajorObstacle1 : StaticBody3D
{
	private bool _dealt_with = false;
	private string _hazard_type = "power";

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _DealWith(string type)
	{
		
	}
}
