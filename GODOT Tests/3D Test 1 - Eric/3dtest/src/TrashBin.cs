using Godot;
using System;

public partial class TrashBin : Area3D
{


	public override void _Ready()
	{


	}


	public override void _Process(double delta)
	{

	}

	void _body_entered(Node body)
	{
		if (body.IsInGroup("thrown"))
		{
			body.QueueFree();
		}
	}




	
}
