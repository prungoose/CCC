using Godot;
using System;
using System.Collections.Generic;

public partial class TrashSpawner : Node3D
{


	[Export] private PackedScene _trash_scene;
	[Export] bool _enabled = true;
	private Timer _timer;
	private CharacterBody3D _player;

	private float _TimeSinceLastSpawn;
	private bool _initial_spawn = false;

	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody3D>("SubViewportContainer/SubViewport/Player");
	}

	public override void _Process(double delta)
	{
		if (_initial_spawn && GlobalPosition.DistanceTo(_player.GlobalPosition) < 50) _TimeSinceLastSpawn += (float)delta;

		//spawn if player > x distance away, more than a minute since last spawn, spawner has <5 remaining trash
		if (!_initial_spawn)
		{
			if (GlobalPosition.DistanceTo(_player.GlobalPosition) < 40)
			{
				_SpawnSomeTrashYayHoorayILoveThisFunctionItsMyFavoriteOfAllTimeWoooILoveGambling();
				_initial_spawn = true;
			}
		}
		else if (_TimeSinceLastSpawn > 100 && GlobalPosition.DistanceTo(_player.GlobalPosition) > 40 && GetChildCount() < 5 && _enabled)
		{
			var r = GD.Randf();
			if (r > .75) _SpawnSomeTrashYayHoorayILoveThisFunctionItsMyFavoriteOfAllTimeWoooILoveGambling();
			else _TimeSinceLastSpawn = 120f;
			_TimeSinceLastSpawn -= 100;
		}

	}

	void _SpawnSomeTrashYayHoorayILoveThisFunctionItsMyFavoriteOfAllTimeWoooILoveGambling()
	{
		//gamble for which types of trash to spawn, how many, and in what ratio (all random). trash types are 1,2,3,4
		int total_amnt_to_spawn = GD.RandRange(5, 25);

		int total_types_to_spawn;
		var r = GD.Randf();
		if (r < .7) total_types_to_spawn = 4;
		else if (r < .85) total_types_to_spawn = 3;
		else if (r < .95) total_types_to_spawn = 2;
		else total_types_to_spawn = 1;

		List<int> types = new List<int>();
		while (types.Count < total_types_to_spawn)
		{
			int rt = GD.RandRange(1, 4);
			if (!types.Contains(rt))
			{
				types.Add(rt);
			}
		}

		List<float> weights = new List<float>();
		for (int i = 0; i < total_types_to_spawn - 1; i++)
		{
			weights.Add(GD.Randf());
		}
		weights.Sort();

		for (int i = 0; i < total_amnt_to_spawn; i++)
		{
			float rand_float = GD.Randf();
			int id_to_spawn = 0;
			for (int j = 0; j < total_types_to_spawn; j++)
			{
				if (j == total_types_to_spawn - 1) id_to_spawn = types[j];
				else if (rand_float < weights[j]) { id_to_spawn = types[j]; break; }
			}
			_SpawnATrash(id_to_spawn);
		}
	}

	private void _SpawnATrash(int id)
	{
		float radius = 5f;
		float randomTheta = (float)GD.RandRange(0.0, Mathf.Tau);
		float randomPhi = Mathf.Acos((float)GD.RandRange(-1.0, 1.0));
		float randomRadius = (float)GD.RandRange(0.0, radius);

		float x = randomRadius * Mathf.Sin(randomPhi) * Mathf.Cos(randomTheta);
		float y = randomRadius * Mathf.Sin(randomPhi) * Mathf.Sin(randomTheta);
		float z = randomRadius * Mathf.Cos(randomPhi);

		RigidBody3D trash = _trash_scene.Instantiate<RigidBody3D>();
		AddChild(trash);
		trash.GlobalPosition = new Vector3(x, 0, z) + GlobalPosition;
		trash.LinearVelocity = new Vector3(x, y, z).Normalized() * (float)GD.RandRange(4f, 8f);
		trash.Call("_ChangeToType", id);
	}
	
}
