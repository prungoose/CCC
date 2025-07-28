using Godot;
using System;
using System.Runtime.ExceptionServices;
using static Godot.ResourceLoader;

public partial class SceneLoader : Control
{
	private string path;
	private bool loading;
	private AnimatedSprite2D sprite;

	[Export] public bool waitforInput = true;
	[Export] SceneTransition _transitionscene;

	private ProgressBar progressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play();
		_transitionscene = GetNode<SceneTransition>("/root/SceneTransition");
		progressBar = GetNode<Panel>("Panel").GetNode<ProgressBar>("ProgressBar");
		LoadLevel("res://assets/level/testscene.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(loading, progressBar.Value);
		if (loading)
		{
			var progress = new Godot.Collections.Array();
			var status = ResourceLoader.LoadThreadedGetStatus(path, progress);
			if (status == ResourceLoader.ThreadLoadStatus.InProgress)
			{
				progressBar.Value = (double)progress[0] * 100;
			}
			else if (status == ResourceLoader.ThreadLoadStatus.Loaded)
			{
				SetProcess(false);
				progressBar.Value = 100;
				if (progressBar.Value == 100)
				{
					_transitionscene.Call("ChangeScene", "res://assets/level/testscene.tscn");
					//ChangeScene(ResourceLoader.LoadThreadedGet(path) as PackedScene);
				}
			}
		}
	}

	// public void ChangeScene(PackedScene resource)
	// {

	// 	var rootNode = GetTree().Root;
	// 	// foreach (var item in GetTree().Root.GetChildren())
	// 	// {
	// 	// 	if (item is Node3D || item is Node2D || item is Control)
	// 	// 	{
	// 	// 		GetTree().Root.RemoveChild(item);
	// 	// 		item.QueueFree();
	// 	// 	}

	// 	// }

	// 	Node currentNode = resource.Instantiate();
	// 	rootNode.AddChild(currentNode);
	// 	//QueueFree();
	// }

	public void LoadLevel(string path)
	{
		this.path = path;
		if (ResourceLoader.HasCached(path))
		{
			//ResourceLoader.LoadThreadedGet(path);
			ResourceLoader.LoadThreadedRequest(path, null, false, ResourceLoader.CacheMode.Reuse);
			//ResourceLoader.LoadThreadedRequest(path);
			loading = true;
		}
		else
		{
			ResourceLoader.LoadThreadedRequest(path);
			loading = true;
		}
	}
}
