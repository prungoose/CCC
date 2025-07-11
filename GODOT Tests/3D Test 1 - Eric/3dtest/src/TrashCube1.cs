using Godot;
using System;

public partial class TrashCube1 : RigidBody3D
{
	//1: Combustibles 2: Plastic 3: PET 4: Cans/Almn
	int _trash_id = 0;
	private float _time_active = 0;
	private AnimatedSprite3D _sprite;
	private CharacterBody3D _player;
	private Sprite3D _minimapsprite;
	private SpringArm3D _springarm;

	

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		_minimapsprite = GetNode<Sprite3D>("Sprite3D");
		_springarm = GetNode<SpringArm3D>("SpringArm3D");

		var r = GD.RandRange(0, 1);
		if (r > 0) _sprite.FlipH = true;
		_sprite.Pause();
	}

	public override void _Process(double delta)
	{
		
		_time_active += (float)delta;
		if ((GlobalPosition.Y < -40 | _time_active > 100) && GlobalPosition.DistanceTo(_player.GlobalPosition) > 40)
		{
			QueueFree();
		}

		if (_minimapsprite.Modulate == new Color(1, 1, 1, 1))
		{
			switch (_trash_id)
			{
				case 1: _minimapsprite.Modulate = new Color(.99f, .39f, .32f, 1f); break; //red
				case 2: _minimapsprite.Modulate = new Color(0f, .75f, .15f, 1f); break; //green
				case 3: _minimapsprite.Modulate = new Color(.99f, .73f, 0f, 1f); break; //yellow
				case 4: _minimapsprite.Modulate = new Color(0f, .67f, .89f, 1f); break; //blue
			}	
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		_springarm.GlobalRotation = new Godot.Vector3(Mathf.DegToRad(90), 0, 0);
		_minimapsprite.GlobalPosition = GlobalPosition;
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
		_sprite.Frame = id;

		switch (id)
		{
			case 1: _minimapsprite.Modulate = new Color(.99f, .39f, .32f, 1f); break; //red
			case 2: _minimapsprite.Modulate = new Color(0f, .75f, .15f, 1f); break; //green
			case 3: _minimapsprite.Modulate = new Color(.99f, .73f, 0f, 1f); break; //yellow
			case 4: _minimapsprite.Modulate = new Color(0f, .67f, .89f, 1f); break; //blue
		}
	}
}
