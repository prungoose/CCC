using Godot;
using System;

public partial class StartButton : Button
{
	private Control _pauseScreen;
	private Tween _tween;
	private Vector2 originalScale = Godot.Vector2.One;
	private Vector2 hoverScale = new Godot.Vector2(1.2f, 1.2f);
	private float _animationTime = 0.15f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pauseScreen = GetParent<Control>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/level/testscene.tscn");
		_pauseScreen.Hide();
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
