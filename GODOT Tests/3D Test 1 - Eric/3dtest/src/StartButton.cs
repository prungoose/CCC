using Godot;
using System;

public partial class StartButton : Button
{
	private Control _pauseScreen;
	private Tween _tween;
	private Vector2 originalScale = Godot.Vector2.One;
	private Vector2 hoverScale = new Godot.Vector2(1.2f, 1.2f);
	private float _animationTime = 0.15f;

	[Export] SceneTransition _transitionscene;

	public ConfigFile CF = new ConfigFile();
	int lang = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pauseScreen = GetParent<Control>();
		_transitionscene = GetNode<SceneTransition>("/root/SceneTransition");

		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			lang = 0;

			StyleBoxTexture image = new();
			StyleBoxTexture imageHover = new();
			image.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button.png");
			imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button_Hover.png");
			this.AddThemeStyleboxOverride("normal", image);
			this.AddThemeStyleboxOverride("hover", imageHover);
			this.AddThemeStyleboxOverride("pressed", imageHover);
		}
		else
		{
			lang = (int)CF.GetValue("playersettings", "lang");
			if (lang == 1)
			{
				StyleBoxTexture image = new();
				StyleBoxTexture imageHover = new();
				image.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button_JP.png");
				imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button_Hover_JP.png");
				this.AddThemeStyleboxOverride("normal", image);
				this.AddThemeStyleboxOverride("hover", imageHover);
				this.AddThemeStyleboxOverride("pressed", imageHover);
			}
			else
			{
				StyleBoxTexture image = new();
				StyleBoxTexture imageHover = new();
				image.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button.png");
				imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Start_Button_Hover.png");
				this.AddThemeStyleboxOverride("normal", image);
				this.AddThemeStyleboxOverride("hover", imageHover);
				this.AddThemeStyleboxOverride("pressed", imageHover);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _StartButtonPressed()
	{
		_transitionscene.Call("ChangeScene", "res://assets/ui/cutscene.tscn");
		_pauseScreen.Hide();
	}

	private void hovered()
	{
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "scale", new Vector2(this.Scale.X * 1.2f, this.Scale.Y * 1.2f), 0.3f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}

	private void Nothovered()
	{
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "scale", new Vector2(this.Scale.X / 1.2f, this.Scale.Y / 1.2f), 0.3f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
	}
}
