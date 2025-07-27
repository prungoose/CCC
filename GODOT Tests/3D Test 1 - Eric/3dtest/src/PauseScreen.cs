using Godot;
using System;

public partial class PauseScreen : Control
{
	[Export] SceneTransition _transitionscene;
	public Control IGOptions;
	public ConfigFile CF = new ConfigFile();
	int lang = 0;

	private Label _title;
	private Button _resumeButton;
	private Button _IGOptionsButton;
	private Button _IGexitButton;
	private Button _pauseButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pauseButton = GetParent().GetNode<Button>("PauseButton");
		IGOptions = GetParent().GetNode<Control>("IGOptions");

		_title = GetNode<Label>("PauseTitle");
		_resumeButton = GetNode<Button>("ResumeButton");
		_IGOptionsButton = GetNode<Button>("InGameOptionsButton");
		_IGexitButton = GetNode<Button>("QuitLevelButton");

		_transitionscene = GetNode<SceneTransition>("/root/SceneTransition");
		StyleBoxTexture imageR = new();
		StyleBoxTexture imageO = new();
		StyleBoxTexture imageE = new();
		StyleBoxTexture imageHoverR = new();
		StyleBoxTexture imageHoverO = new();
		StyleBoxTexture imageHoverE = new();

		var JFont = GD.Load<FontFile>("res://assets/menu/Futehodo-MaruGothic.ttf");
		var EFont = GD.Load<FontFile>("res://assets/menu/Atop.ttf");

		if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
		{
			lang = 0;

			_title.Text = "Paused";
			_title.AddThemeFontOverride("font", EFont);

			imageHoverR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button_Hover.png");
			imageR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button.png");
			_resumeButton.AddThemeStyleboxOverride("normal", imageR);
			_resumeButton.AddThemeStyleboxOverride("hover", imageHoverR);
			_resumeButton.AddThemeStyleboxOverride("pressed", imageHoverR);

			imageHoverO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button.png");
			imageO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover.png");
			_IGOptionsButton.AddThemeStyleboxOverride("normal", imageO);
			_IGOptionsButton.AddThemeStyleboxOverride("hover", imageHoverO);
			_IGOptionsButton.AddThemeStyleboxOverride("pressed", imageHoverO);

			imageHoverE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT.png");
			imageE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT_Pressed.png");
			_IGexitButton.AddThemeStyleboxOverride("normal", imageE);
			_IGexitButton.AddThemeStyleboxOverride("hover", imageHoverE);
			_IGexitButton.AddThemeStyleboxOverride("pressed", imageHoverE);
		}
		else
		{
			lang = (int)CF.GetValue("playersettings", "lang");
			if (lang == 1)
			{
				_title.Text = "休止";
				_title.AddThemeFontOverride("font", JFont);

				imageHoverR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button_Hover_JP.png");
				imageR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button_JP.png");
				_resumeButton.AddThemeStyleboxOverride("normal", imageR);
				_resumeButton.AddThemeStyleboxOverride("hover", imageHoverR);
				_resumeButton.AddThemeStyleboxOverride("pressed", imageHoverR);

				imageHoverO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_JP.png");
				imageO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover_JP.png");
				_IGOptionsButton.AddThemeStyleboxOverride("normal", imageO);
				_IGOptionsButton.AddThemeStyleboxOverride("hover", imageHoverO);
				_IGOptionsButton.AddThemeStyleboxOverride("pressed", imageHoverO);

				imageHoverE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT_JP.png");
				imageE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT_Pressed_JP.png");
				_IGexitButton.AddThemeStyleboxOverride("normal", imageE);
				_IGexitButton.AddThemeStyleboxOverride("hover", imageHoverE);
				_IGexitButton.AddThemeStyleboxOverride("pressed", imageHoverE);

			}
			else
			{
				_title.Text = "Paused";
				_title.AddThemeFontOverride("font", EFont);

				imageHoverR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button_Hover.png");
				imageR.Texture = GD.Load<Texture2D>("res://assets/menu/Resume_Button.png");
				_resumeButton.AddThemeStyleboxOverride("normal", imageR);
				_resumeButton.AddThemeStyleboxOverride("hover", imageHoverR);
				_resumeButton.AddThemeStyleboxOverride("pressed", imageHoverR);

				imageHoverO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button.png");
				imageO.Texture = GD.Load<Texture2D>("res://assets/menu/Options_Button_Hover.png");
				_IGOptionsButton.AddThemeStyleboxOverride("normal", imageO);
				_IGOptionsButton.AddThemeStyleboxOverride("hover", imageHoverO);
				_IGOptionsButton.AddThemeStyleboxOverride("pressed", imageHoverO);

				imageHoverE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT.png");
				imageE.Texture = GD.Load<Texture2D>("res://assets/menu/EXIT_Pressed.png");
				_IGexitButton.AddThemeStyleboxOverride("normal", imageE);
				_IGexitButton.AddThemeStyleboxOverride("hover", imageHoverE);
				_IGexitButton.AddThemeStyleboxOverride("pressed", imageHoverE);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play()
	{
		_pauseButton.Icon = GD.Load<CompressedTexture2D>("res://assets/menu/pausebutton.png");
		_pauseButton.Scale = new Vector2(0.6f, 0.6f);
		_pauseButton.Position = new Vector2(-12f, -12f);
		_pauseButton.Size = new Vector2(200f, 200f);
		GetTree().Paused = false;
		this.Hide();
	}

	public void Paused()
	{
		_pauseButton.Icon = GD.Load<CompressedTexture2D>("res://assets/menu/playbutton.png");
		_pauseButton.Scale = new Vector2(0.2f, 0.2f);
		_pauseButton.Position = new Vector2(0f, 0f);
		GetTree().Paused = true;
		this.Show();
	}

	public void InGameOptions()
	{
		IGOptions.Show();
	}

	public void QuitLevel()
	{
		GetTree().Paused = false;
		_transitionscene.Call("ChangeScene", "res://TitleScreen.tscn");
	}
}
