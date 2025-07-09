using Godot;
using System;
using System.Numerics;

public partial class MajorObstacle1 : StaticBody3D
{
	private bool _dealt_with = false;
	[Export] private int _hazard_id;
	bool phone = false;
	bool popupExists = false;
	bool is_sucking = false;
	private Area3D _vacuum;
	[Export] private float _movespeed = 10;
	[Export] public Control _ui;
	[Export] public CharacterBody3D _player;
	private Label popUp;
	private AnimatedSprite3D _anim;
	private Node3D _tape;
	private MeshInstance3D _proxmesh;
	private ShaderMaterial _shader;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_tape = GetNode<Node3D>("tape");
		_proxmesh = GetNode<MeshInstance3D>("ProximityWarning");
		_shader = (ShaderMaterial)_proxmesh.GetActiveMaterial(0);
	}

	public override void _Process(double delta)
	{
		_shader.SetShaderParameter("character_position", _player.GlobalPosition);

		if (Position.DistanceTo(_player.GlobalPosition) < 5)
		{
			if ((int)_ui.Call("GetTutorialStep") == 4)
			{
				_ui.Call("NextTutorialStep");
			}
		}
	}

	private void DealWith(int id)
	{
		if (id == _hazard_id && _dealt_with == false)
		{
			_dealt_with = true;
			GD.Print(Name, " dealt with");
			var tween = GetTree().CreateTween();
			_tape.Show();
			tween.TweenProperty(_tape, "position", _tape.Position + Godot.Vector3.Down * 50, 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		}
	}

	public void StartAnimation()
	{
		_anim.Visible = true;
		_anim.Play("Warning_Sign");

	}

	public void StopAnimation()
	{
		_anim.Stop();
		_anim.Visible = false;
	}

	public bool GetDealtWithStatus()
	{
		return _dealt_with;
	}

}
