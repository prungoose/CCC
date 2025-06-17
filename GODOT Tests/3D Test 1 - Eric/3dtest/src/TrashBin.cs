using Godot;
using System;

public partial class TrashBin : Area3D
{

	[Export] PackedScene _minigamepacked;
	private CharacterBody3D _player;
	private Node3D _minigame;
	private bool playerEntry = false;
	private Camera3D oldcam;


	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("Player");

	}


	public override void _Process(double delta)
	{
		if (playerEntry && Input.IsActionJustPressed("jump") && _minigame == null)
		{
			oldcam = GetViewport().GetCamera3D();
			_minigame = _minigamepacked.Instantiate<Node3D>();
			AddChild(_minigame);
			_minigame.GlobalPosition = new Godot.Vector3(0, 200, 0);
			_minigame.GlobalRotation = new Godot.Vector3(0, Mathf.DegToRad(-45), 0);
			var cam = _minigame.GetNode<Camera3D>("TrashCam");
			cam.MakeCurrent();
			_player.Call("_changestatus", 1);
		}
	}

	private void _minigame_done()
	{
		oldcam.MakeCurrent();
		_player.Call("_changestatus", 0);
	}

	private void _OnPlayerEntry(Node3D body)
	{
		if (body.IsInGroup("player"))
		{
			playerEntry = true;
		}
	}
	private void _OnPlayerExit(Node3D body)
	{
		if (body.IsInGroup("player"))
		{
			playerEntry = false;
		}
	}
	
}
