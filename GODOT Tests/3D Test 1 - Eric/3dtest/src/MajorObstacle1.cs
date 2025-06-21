using Godot;
using System;

public partial class MajorObstacle1 : StaticBody3D
{
	private bool _dealt_with = false;
	private string _hazard_type = "power";
	bool phone = false;
	bool is_sucking = false;
	private Area3D _vacuum;
	[Export] private float _movespeed = 10;
	[Export] public Control _ui;
	[Export] public CharacterBody3D _player;
	public AnimatedSprite3D _anim;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_anim.Play("Warning Sign");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.DistanceTo(_player.GlobalPosition) < 5 && !phone)
		{
			phone = !phone;
			_ui.Call("_updatephone", phone);
			if (_vacuum != null)
			{
				_vacuum.QueueFree();
				_vacuum = null;
				_movespeed = 10f;
				is_sucking = false;
			}
			else if (phone)
			{
				phone = !phone;
				_ui.Call("_updatephone", phone);
			}
		}
	}

	private void _DealWith(string type)
	{

	}
}
