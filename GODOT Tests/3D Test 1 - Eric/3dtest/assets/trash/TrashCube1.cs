using Godot;
using System;

public partial class TrashCube1 : RigidBody3D
{
	int trash_id = 0;
	private AnimatedSprite3D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		trash_id = GD.RandRange(1, 8);
		var r = GD.RandRange(0, 1);
		_sprite.Frame = trash_id - 1;
		if (r > 0) _sprite.FlipH = true;
		_sprite.Pause();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
