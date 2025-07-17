using Godot;
using System;

public partial class Hazard : Area3D
{
	private bool _dealt_with = false;
	private int _id;
	private Node3D _tape;
	private ShaderMaterial _shader;
	private CharacterBody3D _player;
	private float _time_after_dealt_with = 0;

	private AudioStreamPlayer3D thumpSFX;


	public override void _Ready()
	{
		thumpSFX = GetNode<AudioStreamPlayer3D>("thumpSFX");
		_tape = GetNode<Node3D>("tape");
		var proxmesh = GetNode<MeshInstance3D>("ProximityWarning");
		_shader = (ShaderMaterial)proxmesh.GetActiveMaterial(0);
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
		Rotation = new Godot.Vector3(0, Mathf.DegToRad(GD.Randf() * 360), 0);
	}

	public override void _Process(double delta)
	{
		if (_dealt_with) _time_after_dealt_with += (float)delta;
		_shader.SetShaderParameter("character_position", _player.GlobalPosition);
		if (_time_after_dealt_with >= 20 && GlobalPosition.DistanceTo(_player.GlobalPosition) > 30)
		{
			QueueFree();
		}
	}

	void _DealWith()
	{
		_dealt_with = true;
		var tween = GetTree().CreateTween();
		_tape.Show();
		tween.TweenProperty(_tape, "position", _tape.Position + Godot.Vector3.Down * 50, 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		thumpSFX.Play();
		GetParent().Call("_HazardIsDealtWith");
		
	}

	void _UpdateID(int id)
	{
		switch (id)
		{
			case 0: _id = 0; break;
			case 1: _id = 0; break;
			case 2: _id = 1; break;
			case 3: _id = 1; break;
			case 4: _id = 2; break;
			case 5: _id = 2; break;
			case 6: _id = 3; break;
			case 7: _id = 3; break;
			case 8: _id = 4; break;
			case 9: _id = 4; break;
		}
	}

	void _BeaconEntered(Node3D body)
	{
		if (_dealt_with) return;
		if (body.IsInGroup("beacon"))
		{
			if ((int)body.Call("_GetBeaconID") == _id)
			{
				_DealWith();
				body.Call("_DealtWithSomething");
			}
		}
	}


}
