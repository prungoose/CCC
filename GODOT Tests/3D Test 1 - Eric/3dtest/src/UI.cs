using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
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
		new Sidequest(0, 2, ["Hey... haha... I heard you were in the area...", "Listen, I lost something important to me, just, don't tell anyone it's mine, OK?", "It should be around the empty lots. If you find it, you'll be a big help, thanks."], ["Thanks for finding it. Remember to keep it a secret..."], ["ねえ...ハハハ...あなたがその辺りにいたと聞いたよ...", "いいか、俺は大切なものを失ったんだ、ただ、それが俺のものだって誰にも言わないでくれ、いいか？", "空き地のあたりにあるはずです。見つけたらとても助かります。ありがとうございます。"], ["見つけてくれてありがとう。秘密にしておいてくださいね..."]),
		new Sidequest(1, 3, ["HELP!!!!! AHH!!!!!", "During the typhoon, I lost my cat, [b]Mr. Snuggles[/b], and I don't know where he went!!!", "Can you check around and see if you can find him??", "I think I last saw him around the [b]smaller park[/b] area..."], ["Mr. Snuggles, there you are! Thank you so much for finding him for me!"], ["助けて！！！！！ああ！！！！！", "台風の際、私の猫の[b]チョコさん[/b]がいなくなり、どこへ行ったのか分かりません!!!", "周りを調べて彼を見つけられるかどうか確認してもらえますか？", "最後に彼を見たのは、[b]小さめの公園[/b]のあたりだったと思うのですが..."], ["チョコさん、そこにいました！見つけてくれて本当にありがとう！"]),
		new Sidequest(2, 4, ["Excuse me, I can’t find my important [b]family photo[/b].", "I was carrying it with me on the way home, and the wind blew it away. Please help me find it!", "It might be [b]near a police car[/b]."], ["Phew, the photo is safe, thanks to you!"], ["すみません、大切な[b]家族の写真[/b]が見つかりません。", "帰り道、持ち歩いていたのですが、風で飛ばされてしまいました。探すのを手伝ってください！", "[b]パトカーの近くにあるかもしれません[/b]。"], ["よかった、あなたのおかげで写真は無事です！"]),
		new Sidequest(3, 5, ["I lost my [b]buddy[/b] in the midst of all the chaos. If you find my [b]buddy[/b], let me know!"], ["Sick, you found my friend! Thanks!"], ["この混乱の中で、[b]相棒[/b]を失くしてしまいました。もし[b]相棒[/b]を見つけたら、教えてください！"], ["すごいね、友達が見つかった！ありがとう！"])
	};
	Array<int> _done_quests = [];
	private Node3D _sidequestspawner;

	private AudioStreamPlayer _yessfx;
	private AudioStreamPlayer _nosfx;

	private bool _gamedone = false;

	private RichTextLabel _objlabel;

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
		_sidequestspawner = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/NavigationRegion3D/Level/ItemSpawners");

		_baitText.Modulate = new Color(0.8f, 0.8f, 0.8f);

		_objlabel = GetNode<RichTextLabel>("Minimap/PanelContainer/CenterContainer/ObjLabel");

		_yessfx = GetNode<AudioStreamPlayer>("Phone/YesSFX");
		_nosfx = GetNode<AudioStreamPlayer>("Phone/NoSFX");

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

			if (beacon_to_get <= 4)
			{
				_player.Call("ReadyBeacon", beacon_to_get);
				_phone.Call("homePressed");
				_yessfx.Play();
			}
			else _nosfx.Play();
			_phonetext = "";
			_wiggle();
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

	public void UpdateTankColors()
	{
		foreach (ProgressBar t in _tanks)
		{
			//eventually add something so that tank background changes when throwable ready ?
		}
	}

	public double GetGameCompletion()
	{
		return _gameCompletionBar.Value;
	}

	public void IncreaseGameCompletion(float completion)
	{
		if (_gamedone) return;
		_gameCompletionBar.Value += completion;
		if (_gameCompletionBar.Value >= 100)
		{
			_gamedone = true;
			_gameCompletionBar.Modulate = new Color(0.2f, 1f, 0.2f);
			_gameCompletionBar.Value = 100;
			// Trigger game completion logic here, e.g., show a victory screen or end level
			if (!_jp_lang_enable)
			{
				_textbox.Call("PopUp", (string[])["Wow! Looks like you cleaned up the whole area! Good work!", "Emergency services can take it from here, but you can continue cleaning if you like."], 1);
				_textbox.Call("PopUp", (string[])["Yeah, you're awesome!"], 2);
				_textbox.Call("PopUp", (string[])["I can finally go home and reunite with Mr. Snuggles!"], 3);
				_textbox.Call("PopUp", (string[])["Woah... so you cleaned up that whole place? Sick..."], 5);
				_textbox.Call("PopUp", (string[])["Interesting, it's spotless. I never would have thought that this would happen so fast."], 4);
				_textbox.Call("PopUp", (string[])["Thank you so much for playing our game!"], 1);
				_objlabel.Text = "Thank you for playing our game!";
			}
			else
			{
				_textbox.Call("PopUp", (string[])["すごい！辺り一面をきれいに掃除したみたいですね！よくやった！", "ここからは緊急サービスが対応しますが、ご希望であれば清掃を続けることもできます。"], 1);
				_textbox.Call("PopUp", (string[])["うん、君は素晴らしいよ！"], 2);
				_textbox.Call("PopUp", (string[])["やっと家に帰ってチョコさんと再会できる！"], 3);
				_textbox.Call("PopUp", (string[])["うわあ。。。それで、あそこ全部掃除したの？すごい。。。"], 5);
				_textbox.Call("PopUp", (string[])["面白いですね、きれいですね。こんなに早くそうなるとは思いませんでした。"], 4);
				_textbox.Call("PopUp", (string[])["私たちのゲームをプレイしていただき、誠にありがとうございます！"], 1);
				_objlabel.Text = "私たちのゲームをプレイしていただきありがとうございます！";
			}
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
		if (_done_quests.Count == _possible_sidequests.Length) return;
		if (_active_sidequest == 100 && GetTutorialStep() >= 9) //sq id 100 = no sidequest active
		{
			_sidequestgivetimer.WaitTime = 120;
			_sidequestgivetimer.Start();
			_active_sidequest = GD.RandRange(0, _possible_sidequests.Length - 1);
			while (_done_quests.Contains(_active_sidequest))
				_active_sidequest = GD.RandRange(0, _possible_sidequests.Length - 1);
			Sidequest quest = _possible_sidequests[_active_sidequest];

			if (!_jp_lang_enable) _textbox.Call("PopUp", quest.starting_dialogue_en, quest.caller_id);
			else _textbox.Call("PopUp", quest.starting_dialogue_jp, quest.caller_id);

			if (!_firstsidequestgiven) QuestTutorial();

			_sidequestspawner.Call("_SpawnAt", _active_sidequest);
		}
		else
		{
			_sidequestgivetimer.WaitTime = 10;
			_sidequestgivetimer.Start();
		}
	}

	public void SidequestComplete()
	{
		if (_active_sidequest > 4) return;
		_done_quests.Add(_active_sidequest);
		Sidequest quest = _possible_sidequests[_active_sidequest];
		_active_sidequest = 100;
		if (!_jp_lang_enable) _textbox.Call("PopUp", quest.ending_dialogue_en, quest.caller_id);
		else _textbox.Call("PopUp", quest.ending_dialogue_jp, quest.caller_id);
		IncreaseGameCompletion(8);
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
		_firstsidequestgiven = true;
		_textbox.Call("PopUp", (string[])["It looks like you just got your first [b]request[/b]. Help the citizen in need by finding where their lost item is!"], 1);
	}
}
