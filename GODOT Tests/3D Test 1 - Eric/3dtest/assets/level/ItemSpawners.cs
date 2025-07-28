using Godot;
using System;

public partial class ItemSpawners : Node3D
{


	[Export] public PackedScene _itemscene;
	public Node3D _bigpark;
	public Node3D _smallpark;
	public Node3D _lot;
	public Node3D _polizia;


	public override void _Ready()
	{
		_bigpark = GetNode<Node3D>("BigPark");
		_smallpark = GetNode<Node3D>("SmallPark");
		_lot = GetNode<Node3D>("Lot");
		_polizia = GetNode<Node3D>("Polizia");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//0: big park, 1: small park, 2: lot, 3: by the police car
	public void _SpawnAt(int id)
	{
		RigidBody3D item = _itemscene.Instantiate<RigidBody3D>();
		switch (id)
		{
			case 0: _lot.AddChild(item); item.Call("_ChangeSprite", 0); item.GlobalPosition = _lot.GlobalPosition;  break;
			case 1: _smallpark.AddChild(item); item.Call("_ChangeSprite", 1); item.GlobalPosition = _smallpark.GlobalPosition;break;
			case 2: _polizia.AddChild(item); item.Call("_ChangeSprite", 2); item.GlobalPosition = _polizia.GlobalPosition;break;
			case 3: _bigpark.AddChild(item); item.Call("_ChangeSprite", 3);  item.GlobalPosition = _bigpark.GlobalPosition;break;
		}
	}
}
