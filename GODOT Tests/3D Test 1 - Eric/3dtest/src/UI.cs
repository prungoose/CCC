using Godot;
using System;
using System.Collections.Generic;

public partial class UI : Control
{

	[Export] public CharacterBody3D _player;
	private Tween _tween;
	private Tween _wiggletween;
	private Control _phone;
	private ProgressBar _tank;
	private Label _phonedisplay;
	private string _phonetext;

	public override void _Ready()
	{
		_phone = GetNode<Control>("Phone");
		_phonedisplay = GetNode<Label>("Phone/PhoneSprite/Label");
		_tank = GetNode<ProgressBar>("ProgressBar");

	}

	public override void _Process(double delta)
	{
		_phonedisplay.Text = _phonetext;
		_tank.Value = (float)_player.Call("_gettankpercent");
	}


	public void _updatephone(bool isPhoneOpen)
	{
		_tween?.CustomStep(0.3);
		if (isPhoneOpen)
		{
			_tween?.Kill();
			_tween = GetTree().CreateTween();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, _phone.Position.Y - 1000), 0.3f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		}
		else
		{
			_tween?.Kill();
			_tween = GetTree().CreateTween();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, _phone.Position.Y + 1000), 0.3f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
			_phonetext = "";
		}
	}

	public void _dial(char c)
	{
		if (c != ' ')
		{
			_phonetext += c;
		}
		else
		{
			_phonetext = "";
		}
		_wiggle();
	}

	public void _wiggle()
	{
		_wiggletween?.Kill();
		_wiggletween = GetTree().CreateTween();
    	_wiggletween.TweenProperty(_phone, "rotation_degrees", -5.0f, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		_wiggletween.TweenProperty(_phone, "rotation_degrees", 5.0f, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
    	_wiggletween.TweenProperty(_phone, "rotation_degrees", 0f, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
	}
}
