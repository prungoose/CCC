using Godot;
using System;

public partial class OptionsButton : Button
{
	private Tween _tween;
	private Vector2 originalScale = Godot.Vector2.One;
	private Vector2 hoverScale = new Godot.Vector2(1.2f, 1.2f);
	private float _animationTime = 0.15f;
	public ConfigFile CF = new ConfigFile();
	[Export] private Control Options;
	private int langSelected;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			langSelected = 0;

			StyleBoxTexture image = new();
			StyleBoxTexture imageHover = new();
			image.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button.png");
			imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover.png");
			this.AddThemeStyleboxOverride("normal", image);
			this.AddThemeStyleboxOverride("hover", imageHover);
			this.AddThemeStyleboxOverride("pressed", imageHover);
		}
		else
		{
			langSelected = (int)CF.GetValue("playersettings", "lang");
			if (langSelected == 1)
			{
				StyleBoxTexture image = new();
				StyleBoxTexture imageHover = new();
				image.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_JP.png");
				imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover_JP.png");
				this.AddThemeStyleboxOverride("normal", image);
				this.AddThemeStyleboxOverride("hover", imageHover);
				this.AddThemeStyleboxOverride("pressed", imageHover);
			}
			else
			{
				StyleBoxTexture image = new();
				StyleBoxTexture imageHover = new();
				image.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button.png");
				imageHover.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover.png");
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

	private void _OptionsButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://assets/menu/Options.tscn");		
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
		_tween.TweenProperty(this, "scale", new Vector2(this.Scale.X/1.2f, this.Scale.Y/1.2f), 0.3f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
	}
}
