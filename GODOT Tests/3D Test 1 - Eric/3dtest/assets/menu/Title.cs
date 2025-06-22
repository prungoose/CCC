using Godot;
using System;

public partial class Title : Panel
{
	private Control _pauseScreen;
	private Tween _tweenScale;
	private Tween _tweenRotation;
	private Tween _tweenPosition;
	private Vector2 originalScale = Godot.Vector2.One;
	private Vector2 hoverScale = new Godot.Vector2(1.2f, 1.2f);
	private float _animationTime = 0.15f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tweenScale = GetTree().CreateTween();
		_tweenRotation = GetTree().CreateTween();
		_tweenPosition = GetTree().CreateTween();
		
		_tweenScale.TweenProperty(this, "scale", new Vector2(1.1f,1.1f), 0f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		_tweenScale.TweenProperty(this, "scale", new Vector2(1f,1f), 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		
		this.Rotation = 0f;
		_tweenRotation.TweenProperty(this, "rotation", 0.04f, 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		//_tweenScale.TweenProperty(this, "scale", new Vector2(this.Scale.X*1f, this.Scale.Y*1f), 0.3f).SetTrans(Tween.TransitionType.Bounce).SetEase(Tween.EaseType.In);
		
		_tweenPosition.TweenProperty(this, "position", new Vector2(this.Position.X, this.Position.Y + 20), 0.3f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}
}
