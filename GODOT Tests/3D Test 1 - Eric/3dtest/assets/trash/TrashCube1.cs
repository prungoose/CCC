using Godot;
using System;

public partial class TrashCube1 : RigidBody3D
{
	int _trash_id = 0;
	private AnimatedSprite3D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		var random_trash = GD.RandRange(1, 8);
		_sprite.Frame = random_trash - 1;
		if (random_trash > 4) random_trash -= 4;
		_trash_id = random_trash;

		var r = GD.RandRange(0, 1);
		if (r > 0) _sprite.FlipH = true;

		_sprite.Pause();

	}

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		GetNode<SpringArm3D>("SpringArm3D").GlobalRotation = new Godot.Vector3(Mathf.DegToRad(90), 0, 0);
	}

	public int _GetTrashID()
	{
		return _trash_id;
	}

	public void _ChangeToType(int id)
	{
		if (id == 0) return;
		_trash_id = id;
		var r = GD.RandRange(0, 1);
		if (r > 0) id += 4;
		_sprite.Frame = id - 1;
	}
}
