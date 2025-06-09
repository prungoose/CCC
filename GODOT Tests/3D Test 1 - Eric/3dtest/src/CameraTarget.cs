using Godot;
using System;

public partial class CameraTarget : Node3D
{
	[Export] Node3D _player;
	[Export] Node3D _mousetarget;
	[Export] float _distance = .25f;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		//move smoothly to a point _distance*100% of the way from player to mousetarget
		Vector3 from = _player.GlobalPosition;
		Vector3 to = _mousetarget.GlobalPosition;
		Vector3 pos = from.Lerp(to, _distance);
		this.GlobalPosition = pos;
	}
}
