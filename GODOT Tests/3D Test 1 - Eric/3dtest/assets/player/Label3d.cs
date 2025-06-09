using Godot;
using System;

public partial class Label3d : Label3D
{
	
	private Node3D _player;

	public override void _Ready()
	{
		_player = GetParent<CharacterBody3D>();
	}

	public override void _Process(double delta)
	{
		this.Text = (string)_player.Call("_gettankpercent");
	}
}
