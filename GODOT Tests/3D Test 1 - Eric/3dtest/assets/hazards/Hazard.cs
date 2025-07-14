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


	public override void _Ready()
	{
		_tape = GetNode<Node3D>("tape");
		var proxmesh = GetNode<MeshInstance3D>("ProximityWarning");
		_shader = (ShaderMaterial)proxmesh.GetActiveMaterial(0);
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
	}

	public override void _Process(double delta)
	{
		if (_dealt_with) _time_after_dealt_with += (float)delta;
		_shader.SetShaderParameter("character_position", _player.GlobalPosition);
		if (_time_after_dealt_with >= 20 && GlobalPosition.DistanceTo(_player.GlobalPosition) > 30)
		{
			GetParent().Call("_HazardIsDealtWith");
			QueueFree();
		}
	}

	void _DealWith()
	{
		_dealt_with = true;
		GD.Print(Name, " dealt with");
		var tween = GetTree().CreateTween();
		_tape.Show();
		tween.TweenProperty(_tape, "position", _tape.Position + Godot.Vector3.Down * 50, 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
	}

	void _UpdateID(int id)
	{
		if (id % 2 == 1) id -= 1;	//bc 0,1 = fire, 2,3 = water, etc
		_id = id;
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
