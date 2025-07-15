using Godot;
using System;
using static Godot.ResourceLoader;

public partial class SceneLoader : Control
{
	string loadDir;
	Node currentLoadedNode;
	private Tween tween;
	bool isLoading = false;
	public static SceneLoader Instance;
	private ProgressBar ProgressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		ProgressBar = GetNode<ProgressBar>("ProgressBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isLoading || loadDir == "")
		{
			return;
		}

		Godot.Collections.Array loadPercent = new();
		ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(loadDir, loadPercent);
		double percentage = (double)loadPercent[0];

		if (status == ResourceLoader.ThreadLoadStatus.Loaded)
		{
			if (tween.IsRunning())
			{
				return;
			}
			isLoading = false;
			tween = GetTree().CreateTween();
			tween.TweenProperty(this, "value", percentage, 0.5);
			tween.TweenCallback(Callable.From(InstantiateScene));
		}

		if (ProgressBar.Value == percentage || tween != null)
		{
			return;
		}

		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "value", percentage, 0.5);
	}

	private void InstantiateScene()
	{
		var resource = ResourceLoader.LoadThreadedGet(loadDir) as PackedScene;
		currentLoadedNode = resource.Instantiate();
		GetTree().Root.AddChild(currentLoadedNode);
		this.Hide();
	}

	public void LoadScene(string Path)
	{
		if (isLoading)
		{
			return;
		}
		loadDir = Path;
		ResourceLoader.LoadThreadedRequest(loadDir);
		Visible = true;
		ProgressBar.Value = 0;
		if (currentLoadedNode == null)
		{
			currentLoadedNode.QueueFree();
		}
	}
}
