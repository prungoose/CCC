using Godot;
using System;

public partial class OptionsButton : Button
{
	private Tween _tween;
	private Vector2 originalScale = Godot.Vector2.One;
	private Vector2 hoverScale = new Godot.Vector2(1.2f, 1.2f);
	private float _animationTime = 0.15f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OptionsButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/menu/Options.tscn");
	}

	private void hovered()
	{
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "scale", new Vector2(this.Scale.X*1.2f, this.Scale.Y*1.2f), 0.3f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}

	private void Nothovered()
	{
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "scale", new Vector2(this.Scale.X/1.2f, this.Scale.Y/1.2f), 0.3f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
	}
}
