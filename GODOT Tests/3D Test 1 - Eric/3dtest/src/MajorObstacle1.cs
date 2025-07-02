using Godot;
using System;
using System.Numerics;

public partial class MajorObstacle1 : StaticBody3D
{
	private bool _dealt_with = false;
	[Export] private int _hazard_id;
	bool phone = false;
	bool popupExists = false;
	bool is_sucking = false;
	private Area3D _vacuum;
	[Export] private float _movespeed = 10;
	[Export] public Control _ui;
	[Export] public CharacterBody3D _player;
	public Label popUp;
	public AnimatedSprite3D _anim;
	private Node3D _tape;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_tape = GetNode<Node3D>("tapebody");

		var parent = GetParent().GetParent().GetParent().GetNode<Control>("UI");
		popUp = parent.GetNode<Label>("Popupmsg");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.DistanceTo(_player.GlobalPosition) < 5)
		{
			if ((int)_ui.Call("GetTutorialStep") == 4)
			{
				_ui.Call("NextTutorialStep");
				// StopAnimation();
			}
			// if (popupExists == false)
			// {
			// 	popupExists = true;
			// 	_ui.Call("Pop", "Hello");
			// }
		}
		// else
		// {
		// 	if (popupExists)
		// 	{
		// 		popupExists = false;
		// 		_ui.Call("noPop");
		// 	}
		// }
	}

	private void DealWith(int id)
	{
		if (id == _hazard_id && _dealt_with == false)
		{
			_dealt_with = true;
			GD.Print(Name, " dealt with");
			var tween = GetTree().CreateTween();
			_tape.Show();
			tween.TweenProperty(_tape, "position", _tape.Position + Godot.Vector3.Down * 50, 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		}


	}

	public void StartAnimation()
	{
		_anim.Visible = true;
		_anim.Play("Warning_Sign");

	}

	public void StopAnimation()
	{
		_anim.Stop();
		_anim.Visible = false;
	}

	public bool GetDealtWithStatus()
	{
		return _dealt_with;
	}

}
