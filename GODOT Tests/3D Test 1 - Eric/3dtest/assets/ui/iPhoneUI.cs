using Godot;
using System;
using System.Net;

public partial class iPhoneUI : Control
{
	private Button _fireButton;
	private Button _waterButton;
	private Button _powerButton;
	private Button _animalButton;
	private Button _healthButton;
	private Button _dialButton;

	private RichTextLabel displayInfo;
	private RichTextLabel dialDisplay;

	private Control _ui;
	private AudioStreamPlayer _phoneSfx;
	private Tween _tween;

	private CharacterBody3D _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_fireButton = GetNode<Control>("Fire").GetNode<Button>("fireButton");
		_waterButton = GetNode<Control>("Water").GetNode<Button>("waterButton");
		_powerButton = GetNode<Control>("Power").GetNode<Button>("powerButton");
		_healthButton = GetNode<Control>("Health").GetNode<Button>("healthButton");
		_dialButton = GetNode<Control>("Dial").GetNode<Button>("dialTextDisplay/dialButton");

		_ui = GetNodeOrNull<Control>("../../UI");
		_phoneSfx = GetNodeOrNull<AudioStreamPlayer>("PhoneSFX");

		displayInfo = GetNode<RichTextLabel>("displayInfo");
		dialDisplay = GetNode<Control>("Dial").GetNode<RichTextLabel>("dialTextDisplay");

		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		dialDisplay.Text = (string)_ui.Call("GetPhoneText");
	}

	private void firePressed()
	{

		_player.Call("setHomeScreen", false);
		displayInfo.PivotOffset = new Vector2(70, 190);
		displayInfo.Text = "The [b][color=red]Fire Department[/color][/b] responds to:\n•  [b]Fires[/b]\n•  [b]Rescue operations[/b]\n\nSeparate any flammable objects close to the area to control the spread of fire.\n\n[b]Agency Access Code:[/b] ↑ → ↓ ←";
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(displayInfo, "scale", new Vector2(1, 1), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		
		if ((int)_ui?.Call("GetTutorialStep") == 6)
			_ui.Call("NextTutorialStep");
	}
	private void waterPressed()
	{
		_player.Call("setHomeScreen", false);
		displayInfo.PivotOffset = new Vector2(215, 190);
		displayInfo.Text = "The [b][color=light_blue]Water Utility Company[/color][/b] maintains [i]water systems[/i] and responds to:\n•  [b]Burst pipes[/b] \n\nBe wary of contaminated water. \n\n[b]Agency Access Code:[/b] ↓ → ↑ ↑";
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(displayInfo, "scale", new Vector2(1, 1), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}
	private void powerPressed()
	{
		_player.Call("setHomeScreen", false);
		displayInfo.PivotOffset = new Vector2(365, 190);
		displayInfo.Text = "The [b][color=yellow]Power Company[/color][/b] maintains the [i]electric grid[/i] and responds to:\n•  [b]downed lines[/b] \n•  [b]blackouts[/b]\n•  [b]damaged transformer boxes[/b]\n\nContact them for electrical hazards. Do not handle power lines yourself\n\n[b]Agency Access Code:[/b] → ← → ←";
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(displayInfo, "scale", new Vector2(1, 1), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);


	}
	private void animalPressed()
	{
		_player.Call("setHomeScreen", false);
		displayInfo.PivotOffset = new Vector2(135, 340);
		displayInfo.Text = "The [b][color=purple]Animal Control Center[/color][/b] responds to:\n•  [b]wild or injured animals[/b]\n\nDo not handle wild animals without proper equipment.\n\n[b]Agency Access Code:[/b] ← ↓ → ←";
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(displayInfo, "scale", new Vector2(1, 1), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}
	private void healthPressed()
	{
		_player.Call("setHomeScreen", false);
		displayInfo.PivotOffset = new Vector2(295, 340);
		displayInfo.Text = "The [b][color=green]General Health Team[/color][/b] responds to:\n•  [b]tipped porta potties[/b]\n•  [b]chemical spills[/b]\n\nStay a safe distance away from the hazard. \n\n[b]Agency Access Code:[/b] ↓ → ↓ →";

		//"Chemical spills can be [b]extremely dangerous[/b].\n\nThey may cause [i]fires[/i], [i]explosions[/i], or long-term [color=orange]environmental damage[/color].\n\nIf you see a chemical spill:\n• [b]Avoid the area immediately[/b]\n• Call the proper agency for cleanup\n• Do not touch or inhale fumes";
		
		_tween?.CustomStep(0.3);
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(displayInfo, "scale", new Vector2(1, 1), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		
		// "Fallen trees can block roads and damage property or power lines.\n\nWhen you see one:\n• [b]Keep a safe distance[/b] from hanging wires\n• Call the appropriate agency\n• Do not attempt to move it yourself"
		//"EMS provides [b]life-saving care[/b] in emergencies.\n\nThey respond to:\n• [i]Injuries[/i]\n• [i]Exposure to toxins[/i]\n• [i]Fainting or collapse[/i]\n\nCall EMS if someone is hurt or unconscious.\n\n[b]Agency Access Code:[/b] ↓ ↓ ↑ ↑"
	}
	private void dialPressed()
	{
		GD.Print("dial pressed");
		/*
		_tween?.Kill();
		_tween?.CustomStep(0.3);
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(dialDisplay, "size", new Vector2(410, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		_dialButton.Hide();
		*/
		_ui.Call("_dial", ' ');
		//ui.Call("")
		//_tween.TweenProperty(dialDisplay, "size", new Vector2(1, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

		// displayInfo.PivotOffset = new Vector2(365, 340);
		// _tween?.CustomStep(0.3);
		// _tween?.Kill();
		// _tween = GetTree().CreateTween();
		// _tween.TweenProperty(dialDisplay, "position", new Vector2(-205, -270), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		// _tween.TweenProperty(dialDisplay, "size", new Vector2(415, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		// _tween.TweenProperty(dialDisplay, "size", new Vector2(415, 300), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}


	public void homePressed()
	{
		GD.Print("homepressed");
		if (!(bool)_player.Call("_getPhone")) return;
		_ui.Call("ResetPhoneText");
		if (!_dialButton.Visible)
		{
			displayInfo.Text = "";
			_tween?.CustomStep(0.3);
			_tween?.Kill();
			_tween = GetTree().CreateTween();
			_tween.TweenProperty(displayInfo, "scale", new Vector2(0, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

			_tween.Finished += () => _player.Call("setHomeScreen", true);
		}

		if (_dialButton.Visible)
			{
				_dialButton.Hide();

				_tween?.Kill();
				_tween?.CustomStep(0.3);
				_tween = GetTree().CreateTween();
				_tween.TweenProperty(dialDisplay, "size", new Vector2(410, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
				//_ui.Call("_dial", ' ');

				if (displayInfo.Scale != new Godot.Vector2(1, 1))
				_tween.Finished += () => _player.Call("setHomeScreen", true);
			}
	}
	
}
