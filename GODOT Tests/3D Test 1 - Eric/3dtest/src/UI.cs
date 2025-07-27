using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class UI : Control
{

	private struct Sidequest
	{
		public int spawn_location;
		public int caller_id;
		public string[] starting_dialogue_en;
		public string[] ending_dialogue_en;
		public string[] starting_dialogue_jp;
		public string[] ending_dialogue_jp;

		public Sidequest(int a, int b, string[] c, string[] d, string[] e, string[] f)
		{
			spawn_location = a;     //
			caller_id = b;
			starting_dialogue_en = c;
			ending_dialogue_en = d;
			starting_dialogue_jp = e;
			ending_dialogue_jp = f;
		}
	}

	[Export] public CharacterBody3D _player;
	private Tween _tween;
	private Tween _wiggletween;
	private Control _phone;
	private ProgressBar _tank;
	private RichTextLabel _phonedisplay;
	private Button _phonedisplayButton;
	private string _phonetext = "";
	private MarginContainer _tutorialStuff;


	private bool _tankStepCompleted = false;
	private bool _movementStepCompleted = false;

	// Hazards
	private Node3D _tutorialHazard;

	// Tanks
	private ProgressBar _tank1;
	private ProgressBar _tank2;
	private ProgressBar _tank3;
	private ProgressBar _tank4;
	private ProgressBar[] _tanks;

	private AudioStreamPlayer PhoneSFX;

	[Export] bool debug = false;
	private PanelContainer _debugpanel;
	private RichTextLabel _debugtext;

	private Control _pauseScreen;
	private Control _textbox;
	private Control _minimap;

	private ProgressBar _gameCompletionBar;

	private Timer _sidequestgivetimer;
	private int _active_sidequest = 100;
	private bool _firstsidequestgiven = false;
	private bool _jp_lang_enable = false;
	Sidequest[] _possible_sidequests = {
		new Sidequest(0, 2, ["Hey... haha... I heard you were in the area...", "Listen, I lost something important to me, just, don't tell anyone it's mine, OK?", "It should be around the parking lot to the north. If you find it, you'll be a big help, thanks."], ["sq1 f en"], ["sq1 s jp"], ["sq1 e jp"]),
		new Sidequest(1, 3, ["HELP!!!!! AHH!!!!!", "During the typhoon, I lost my cat, Mr. Snuggles, and I don't know where he went!!!", "Can you check around and see if you can find him??", "I think I last saw him around the [b]western park[/b] area..."], ["sq2 f en"], ["sq2 s jp"], ["sq2 e jp"])
	};




	[Export] private Label _baitText;

	public override void _Ready()
	{
		_phone = GetNode<Control>("Phone");
		_phonedisplay = GetNode<RichTextLabel>("Phone/Dial/dialTextDisplay");
		_phonedisplayButton = GetNode<Button>("Phone/Dial/dialTextDisplay/dialButton");

		_tank1 = GetNode<ProgressBar>("Minimap/Tanks/Tank1");
		_tank2 = GetNode<ProgressBar>("Minimap/Tanks/Tank2");
		_tank3 = GetNode<ProgressBar>("Minimap/Tanks/Tank3");
		_tank4 = GetNode<ProgressBar>("Minimap/Tanks/Tank4");
		_tanks = [_tank1, _tank2, _tank3, _tank4];

		_tutorialStuff = GetNode<MarginContainer>("Tutorial Stuff");

		_tutorialHazard = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/NavigationRegion3D/Level/HazSpawners/HazardSpawner");

		PhoneSFX = GetNode<AudioStreamPlayer>("Phone/PhoneSFX");

		_debugpanel = GetNode<PanelContainer>("DebugPanel");
		_debugtext = _debugpanel.GetNode<RichTextLabel>("MarginContainer/MarginContainer/RichTextLabel");
		if (!debug)
		{
			_debugpanel.Hide();
		}

		_pauseScreen = GetNode<Control>("PauseScreen");
		_textbox = GetNode<Control>("Textbox");
		_minimap = GetNode<Control>("Minimap");

		_gameCompletionBar = GetNode<ProgressBar>("Minimap/ProgressBar");

		_sidequestgivetimer = GetNode<Timer>("SidequestGiveTimer");
		_sidequestgivetimer.Timeout += SidequestGive;

		_baitText.Modulate = new Color(0.8f, 0.8f, 0.8f);

	}

	public override void _Process(double delta)
	{
		//_phonedisplay.Text = _phonetext;
		_tank1.Value = (int)_player.Call("_GetTankPercentage", 1);
		_tank2.Value = (int)_player.Call("_GetTankPercentage", 2);
		_tank3.Value = (int)_player.Call("_GetTankPercentage", 3);
		_tank4.Value = (int)_player.Call("_GetTankPercentage", 4);
		UpdateTankColors();

		// Go to next step in tutorial first time tank reaches 50%
		if (GetTutorialStep() == 1)
			for (int i = 1; i < 5; i++)
				if ((int)_player.Call("_GetTankPercentage", i) >= 20) NextTutorialStep();

		if (debug)
			_debugtext.Text = "FPS: " + Engine.GetFramesPerSecond() + "\nMemory: " + (OS.GetStaticMemoryUsage() / 1024) + " KB";

	}

	public void _updatephone(bool isPhoneOpen)
	{
		_tween?.CustomStep(1f);
		if (isPhoneOpen)
		{
			// Tutorial Progression
			if (GetTutorialStep() == 5)
			{
				NextTutorialStep();
			}

			_tween?.Kill();
			_tween = GetTree().CreateTween();

			//occur first
			_tween.TweenProperty(_minimap, "position", new Vector2(_minimap.Position.X, 1535), 0.2f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
			_tween.TweenProperty(_minimap, "scale", new Vector2(1, 1), 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In); //dummy tween for a delay
			_tween.TweenProperty(_minimap, "scale", new Vector2(1, 1), 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In); //dummy tween for setparallel
																																			 //occur after first tween is done
			_tween.SetParallel();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, 551), 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			_tween.TweenProperty(_phone, "rotation", Mathf.DegToRad(8), .4f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);

		}
		else
		{
			_tween?.Kill();
			_tween = GetTree().CreateTween();

			//occur first
			_tween.SetParallel();
			_tween.TweenProperty(_phone, "position", new Vector2(_phone.Position.X, 1551), 0.3f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
			_tween.TweenProperty(_phone, "rotation", 0, .3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			_tween.SetParallel(false);
			//occur second
			_tween.TweenProperty(_minimap, "position", new Vector2(_minimap.Position.X, 935), 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			_phonetext = "";
		}
	}



	public void _dial(char c)
	{
		GD.Print(c, " l:", _phonetext.Length);
		GD.Print("visibility: ", _phonedisplayButton.Visible);
		if (c != ' ' && _phonetext.Length < 4)
		{
			_phonetext += c;
			if (!_phonedisplayButton.Visible)
			{
				_player.Call("setHomeScreen", false);
				//_tween?.Kill();
				_tween = GetTree().CreateTween();
				//_tween.TweenProperty(_phonedisplay, "size", new Vector2(410, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
				_tween.TweenProperty(_phonedisplay, "size", new Vector2(410, 290), 0.2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
				_tween.Finished += () => _phonedisplayButton.Show();

			}
			_wiggle();
			PhoneSFX.Play();
		}
		else if (c == ' ')
		{
			GD.Print("dial detected");
			int beacon_to_get = 5;
			switch (_phonetext)
			{
				case "↑→↓←": if (GetTutorialStep() == 7) NextTutorialStep(); beacon_to_get = 0; break; //fire
				case "↓→↑↑": beacon_to_get = 1; break; //water
				case "→←→←": beacon_to_get = 2; break; //elec
				case "←↓→←": beacon_to_get = 3; break; //animal
				case "↓→↓→": beacon_to_get = 4; break; //health
			}

			if (beacon_to_get <= 4) { _player.Call("ReadyBeacon", beacon_to_get); _phone.Call("homePressed"); }
			;
			_phonetext = "";
			_wiggle();
			PhoneSFX.Play();



		}
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
				var beacon = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/NavigationRegion3D/Level/Beacon");
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

	public void UpdateTankColors()
	{
		foreach (ProgressBar t in _tanks)
		{
			//eventually add something so that tank background changes when throwable ready
		}
	}

	public void IncreaseGameCompletion(float completion)
	{
		_gameCompletionBar.Value += completion;
		if (_gameCompletionBar.Value >= 100)
		{
			_gameCompletionBar.Modulate = new Color(0.2f, 1f, 0.2f);
			_gameCompletionBar.Value = 100;
			// Trigger game completion logic here, e.g., show a victory screen or end level
		}
		else
		{
			_gameCompletionBar.Modulate = new Color(1f, 1f, 1f);
		}
	}

	public string GetPhoneText()
	{
		return _phonetext;
	}

	public void ResetPhoneText()
	{
		_phonetext = "";
	}

	public void SidequestGive()
	{
		if (_active_sidequest != 100) //sq id 100 = no sidequest active
		{
			_sidequestgivetimer.WaitTime = 20;
			_sidequestgivetimer.Start();
			return;
		}
		_sidequestgivetimer.WaitTime = 120;
		_sidequestgivetimer.Start();
		_active_sidequest = GD.RandRange(1, _possible_sidequests.Length - 1);
		Sidequest quest = _possible_sidequests[_active_sidequest];

		if (!_jp_lang_enable) _textbox.Call("PopUp", quest.starting_dialogue_en, quest.caller_id);
		else _textbox.Call("PopUp", quest.starting_dialogue_jp, quest.caller_id);

		if (!_firstsidequestgiven) QuestTutorial();
	}

	public void SidequestComplete()
	{
		Sidequest quest = _possible_sidequests[_active_sidequest];
		_active_sidequest = 100;
		if (!_jp_lang_enable) _textbox.Call("PopUp", quest.ending_dialogue_en, quest.caller_id);
		else _textbox.Call("PopUp", quest.ending_dialogue_jp, quest.caller_id);
		IncreaseGameCompletion(10);
	}

	public void _UpdateBaitCharges(int charges)
	{
		_baitText.Text = charges.ToString();
	}

	private void _UpdateBaitSelected(bool isSelected)
	{
		if (isSelected)
		{
			_baitText.Modulate = new Color(1f, 1f, 1f);

		}
		else
		{
			_baitText.Modulate = new Color(0.8f, 0.8f, 0.8f);

		}

	}

	private void QuestTutorial()
	{
		_textbox.Call("PopUp", (string[])["It looks like you just got your first [b]request[/b]. Help the citizen in need by finding where their lost item is!"], 1);
	}
}
