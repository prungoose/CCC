using Godot;
using System;

public partial class RequestItem : RigidBody3D
{

	private Sprite3D _sprite;
	private Sprite3D _mapsprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite3D>("Sprite3D");
		_mapsprite = GetNode<Sprite3D>("MapSprite3D");
	}

	public override void _Process(double delta)
	{
		_mapsprite.GlobalPosition = GlobalPosition;
	}

	public void _ChangeSprite(int id)
	{
		_sprite.Frame = id;
	}
}
