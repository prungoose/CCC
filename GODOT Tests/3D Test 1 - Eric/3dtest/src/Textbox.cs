using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class Textbox : Control
{

	private RichTextLabel _text;
	private Timer _timer;
	private AnimatedSprite2D _sprite;
	private string[] _text_bin;
	private int _bin_progress = 0;
	private int _text_pos = 0;
	private Queue<KeyValuePair<string[], int>> _popup_queue = new();

	private float tps = 50f;    //text per second
	private float _acc = 0f;
	private bool _bbcode = false;
	private bool _active = false;
	private Godot.Vector2 _initalpos;

	public override void _Ready()
	{
		tps = 1 / tps;
		_text = GetNode<RichTextLabel>("PanelContainer/MarginContainer/MarginContainer/RichTextLabel/");
		_timer = GetNode<Timer>("Timer");
		_sprite = GetNode<AnimatedSprite2D>("PanelContainer/AnimatedSprite2D");
		_sprite.Pause();
		_timer.Timeout += Continue;
		_initalpos = Position;
		Scale = new Godot.Vector2(0, 0);
	}

	public override void _Process(double delta)
	{
		if (_text_pos != -1) _acc += (float)delta;
		if (_popup_queue.Count > 0 && !_active)
		{
			KeyValuePair<string[], int> t = _popup_queue.Dequeue();
			PopUp(t.Key, t.Value);
		}
		if (_text_bin != null)
		{
			if (_text_bin.Length > 0 && _acc > tps && _bin_progress < _text_bin.Length)
			{
				TypeText();
			}
		}
	}

	private void PopUp(string[] text, int caller_id)
	{
		if (_active)
		{
			KeyValuePair<string[], int> t = new KeyValuePair<string[], int>(text, caller_id);
			_popup_queue.Enqueue(t);
			return;
		}
		var x = GD.RandRange(-50, 50);
		var y = GD.RandRange(-120, 50);
		Position = _initalpos + new Godot.Vector2(x, y);
		_text.Text = "";
		_text_pos = 0;
		_active = true;
		_text_bin = text;
		_bin_progress = 0;
		if (caller_id == 0) _sprite.Hide();
		else { _sprite.Show(); _sprite.Frame = caller_id; }
		_timer.WaitTime = 3 + text[0].Length * .04f;
		_timer.Start();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "scale", new Godot.Vector2(1, 1), .35f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}

	private void Continue()
	{
		if (!_active) return;
		_bin_progress++;
		if (_bin_progress >= _text_bin.Length) PopAway();
		else
		{
			_timer.WaitTime = 2 + _text_bin[_bin_progress].Length * .04f;
			_timer.Start();
			_text_pos = 0;
			_text.Text = "";
		}
	}

	private void PopAway()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "scale", new Godot.Vector2(0, 0), .15f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.Finished += () => _active = false;
	}

	private void TypeText()
	{
		if (_text_pos >= 0 && _text_pos < _text_bin[_bin_progress].Length)
		{
			char c = _text_bin[_bin_progress][_text_pos];
			if (c == '[')
			{
				string temp = "[";
				while (true)
				{
					_text_pos++;
					temp += _text_bin[_bin_progress][_text_pos];
					if (_text_bin[_bin_progress][_text_pos] == ']') break;
				}
				_text.Text += temp;
				_text_pos++;
				TypeText();
				return;
			}
			_text.Text += c;
			_text_pos++;
		}
		else _text_pos = -1;
		_acc = 0;
	}


}
