using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransition : Control
{
	private ColorRect black;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		black = GetNode<ColorRect>("Black");
		black.Modulate = new Color(0, 0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void ChangeScene(string scenePath)
	{
		await transitionIn();
		GetTree().ChangeSceneToFile(scenePath);
		transitionOut();
	}

	private async Task transitionIn()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(black, "modulate:a", 1.0f, 0.5f);
		await ToSignal(tween, Tween.SignalName.Finished);
	}
	private void transitionOut()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(black, "modulate:a", 0.0f, 0.5f);
	}
}
