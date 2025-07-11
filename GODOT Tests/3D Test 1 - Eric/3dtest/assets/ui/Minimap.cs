using Godot;
using System;

public partial class Minimap : Control
{

	private Node3D _worldpivot;
	private Node3D _worldtarget;
	[Export] private Camera3D _minimapcam;

	public override void _Ready()
	{
		_worldpivot = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/Camera");
		_worldtarget = GetTree().CurrentScene.GetNode<Node3D>("SubViewportContainer/SubViewport/Camera/CameraTarget");
		//_minimapcam = GetNode<Camera3D>("PanelContainer/Minimap/SubViewport/Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		_minimapcam.Rotation = new Godot.Vector3(Mathf.DegToRad(-90), _worldpivot.Rotation.Y + Mathf.DegToRad(-45), 0);
		var pos = _worldtarget.GlobalPosition;
		_minimapcam.GlobalPosition = new Godot.Vector3(pos.X, 50, pos.Z);
    }

}
