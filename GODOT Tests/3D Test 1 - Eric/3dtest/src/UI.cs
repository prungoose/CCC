using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class UI : Control
{

	[Export] public CharacterBody3D _player;
	private Tween _tween;
	private Tween _wiggletween;
	private Control _phone;
	private ProgressBar _tank;
	private Label _phonedisplay;
	private string _phonetext;
	private MarginContainer _tutorialStuff;

	private bool _tankStepCompleted = false;
	private bool _movementStepCompleted = false;

	private Control _infoSection;
	private RichTextLabel _infoHeading;
	private RichTextLabel _infoText;

	// Hazards
	private StaticBody3D _powerLineHazard;

	// Tanks
	private ProgressBar _tank1;
	private ProgressBar _tank2;
	private ProgressBar _tank3;
	private ProgressBar _tank4;

	private AudioStreamPlayer PhoneSFX;

	[Export] bool debug = false;
	private PanelContainer _debugpanel;
	private RichTextLabel _debugtext;

	private Control _pauseScreen;

	public override void _Ready()
	{
		_phone = GetNode<Control>("Phone");
		_phonedisplay = GetNode<Label>("Phone/PhoneSprite/Dial Screen/Label");
		_tank1 = GetNode<ProgressBar>("Tanks/Tank1");
		_tank2 = GetNode<ProgressBar>("Tanks/Tank2");
		_tank3 = GetNode<ProgressBar>("Tanks/Tank3");
		_tank4 = GetNode<ProgressBar>("Tanks/Tank4");
		_tutorialStuff = GetNode<MarginContainer>("Tutorial Stuff");

		_infoSection = GetNode<Control>("Info Section");
		_infoHeading = GetNode<RichTextLabel>("Info Section/Heading");
		_infoText = GetNode<RichTextLabel>("Info Section/Text");
		_infoSection.Visible = false;

		_powerLineHazard = GetParent().GetNode<StaticBody3D>("SubViewport/Level/Major Obstacle");

		PhoneSFX = _phone.GetNode<AudioStreamPlayer>("PhoneSFX");

		_debugpanel = GetNode<PanelContainer>("DebugPanel");
		_debugtext = _debugpanel.GetNode<RichTextLabel>("MarginContainer/MarginContainer/RichTextLabel");
		if (!debug)
		{
			_debugpanel.Hide();
		}

		_pauseScreen = GetNode<Control>("PauseScreen");
	}

	public override void _Process(double delta)
	{
		_phonedisplay.Text = _phonetext;

		_tank1.Value = (int)_player.Call("_GetTankPercentage", 1);
		_tank2.Value = (int)_player.Call("_GetTankPercentage", 2);
		_tank3.Value = (int)_player.Call("_GetTankPercentage", 3);
		_tank4.Value = (int)_player.Call("_GetTankPercentage", 4);

		// Go to next step in tutorial first time tank reaches 50%
		if ((float)_player.Call("_GetTankPercentage", 1) >= 20 && !_tankStepCompleted)
		{
			_tankStepCompleted = true;
			NextTutorialStep();
		}

		if (debug)
		{
			_debugtext.Text = "FPS: " + Engine.GetFramesPerSecond() + "\nMemory: " + (OS.GetStaticMemoryUsage() / 1024) + " KB";
		}
	}

	public void _updatephone(bool isPhoneOpen)
	{
		_tween?.CustomStep(0.3);
		if (isPhoneOpen)
		{
			// Tutorial Progression
			if (GetTutorialStep() == 5)
			{
				NextTutorialStep();
			}

			_tween?.Kill();
			_tween = GetTree().CreateTween();
			_tween.SetParallel();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, _phone.Position.Y - 1000), 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			_tween.TweenProperty(_phone, "rotation", Mathf.DegToRad(8), .4f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		}
		else
		{
			_tween?.Kill();
			_tween = GetTree().CreateTween();
			_tween.SetParallel();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, _phone.Position.Y + 1000), 0.3f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
			_tween.TweenProperty(_phone, "rotation", 0, .4f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
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
			GD.Print("Dialed: " + _phonetext);

			//Power company: ID 1
			if (_phonetext == "↑→↓←")
			{
				if (GetTutorialStep() == 9)
				{
					NextTutorialStep();
				}
				_player.Call("ReadyBeacon", 1);
			}

			//More agencies go here with their own code

			_phonetext = "";
		}
		_wiggle();
		PhoneSFX.Play();
	}

	public void _wiggle()
	{
		_wiggletween?.Kill();
		_wiggletween?.CustomStep(0.2);
		_wiggletween = GetTree().CreateTween();
		var init = Mathf.RadToDeg(Mathf.DegToRad(8));
		_wiggletween.TweenProperty(_phone, "rotation_degrees", init - 5.0f, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		_wiggletween.TweenProperty(_phone, "rotation_degrees", init + 5.0f, 0.1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		_wiggletween.TweenProperty(_phone, "rotation_degrees", init, 0.05f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
	}

	public void OnBeaconReached(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			if (!_movementStepCompleted)
			{
				_movementStepCompleted = true;
				var beacon = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/Level/Beacon");
				beacon.QueueFree();
				NextTutorialStep();
			}
		}

	}
	public void NextTutorialStep()
	{
		_tutorialStuff.Call("NextStep");
	}

	public int GetTutorialStep()
	{
		return (int)_tutorialStuff.Call("GetTutorialStep");
	}

	public void ShowInfoSection(string heading, string text)
	{
		_infoHeading.Text = heading;
		_infoText.Text = text;
		_infoSection.Visible = true;
	}

	private void HideInfoSection()
	{
		_infoSection.Visible = false;
		_infoHeading.Text = "";
		_infoText.Text = "";
	}

	private void _UpdateThrown(int id)
	{
		ProgressBar[] tanks = [_tank1, _tank2, _tank3, _tank4];
		id -= 1;

		for (int i = 0; i < 4; i++)
		{
			ProgressBar target = tanks[i];

			float XTarget = 80f;
			float YTarget = 200f;
			float duration = .25f;
			if (i == id)
			{
				XTarget = 100f;
				YTarget = 240f;
			}
			Vector2 initial = target.CustomMinimumSize;
			Vector2 final = new Vector2(XTarget, YTarget);
			if ((initial - final).IsZeroApprox()) continue;

			var tween1 = GetTree().CreateTween();
			var tween2 = GetTree().CreateTween();
			tween1.TweenMethod(
				Callable.From<float>(x => target.CustomMinimumSize = new Vector2(x, target.CustomMinimumSize.Y)),
				initial.X, final.X, duration
			).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);
			tween2.TweenMethod(
				Callable.From<float>(y => target.CustomMinimumSize = new Vector2(target.CustomMinimumSize.X, y)),
				initial.Y, final.Y, duration
			).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);

		}
	}
	
	public void Paused()
	{
		GetTree().Paused = true;
		_pauseScreen.Show();
	}
}
