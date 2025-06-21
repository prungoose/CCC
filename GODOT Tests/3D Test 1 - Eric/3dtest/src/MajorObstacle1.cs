using Godot;
using System;

public partial class MajorObstacle1 : StaticBody3D
{
	private bool _dealt_with = false;
	private string _hazard_type = "power";
	bool phone = false;
	bool popupExists = false;
	bool is_sucking = false;
	private Area3D _vacuum;
	[Export] private float _movespeed = 10;
	[Export] public Control _ui;
	[Export] public CharacterBody3D _player;
	public Label popUp;
	public AnimatedSprite3D _anim;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_anim.Play("Warning Sign");

		var parent = GetParent().GetParent().GetParent().GetNode<Control>("UI");
		popUp = parent.GetNode<Label>("Popupmsg");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.DistanceTo(_player.GlobalPosition) < 5)
		{
			if (popupExists == false)
			{
				popupExists = true;
				popUp.Show();
				_ui.Call("Pop", "hello");
			}
		}
		else 
		{
			if (popupExists)
			{
				popupExists = false;
				_ui.Call("noPop");
			}
		}
	}

	private void _DealWith(string type)
	{

	}


}
