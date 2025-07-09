using Godot;
using System;

public partial class CutsceneText : RichTextLabel
{
    private Tween _tween;
    private int textPos = 0;
    private bool typeStart = false;
    private AudioStreamPlayer CutsceneSFX;
    private RichTextLabel BottomTip;
    
    public ConfigFile CF = new ConfigFile();
    private float val;
	private int sfx_index;
    [Export] SceneTransition _transitionscene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _transitionscene = GetNode<SceneTransition>("/root/SceneTransition");

        sfx_index = AudioServer.GetBusIndex("SFX");

        BottomTip = GetParent().GetNode<RichTextLabel>("Bottom Tip");
        CutsceneSFX = GetParent().GetParent().GetNode<AudioStreamPlayer>("CutsceneSFX");
        GD.Print("preSequenceText ready: ", Name, " | In tree: ", IsInsideTree());
        // Set the text to be displayed
        // Show the text
        Show();
        BottomTip.Modulate = new Color(1, 1, 1, 0);

        if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
        {
            AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(1f));
        }
        else
        {
            val = (float)CF.GetValue("playersettings", "sfx");
            AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(val));
            GD.Print("eles block");
        }

    }

    // Text to be displayed in the visual novel.
    private string[] text = {
        "Welcome to Crisis Cleanup Crew! This is our test playground where we’ll teach you everything we know before assigning you to your first job!",
        "Cleaning up after a natural disaster is very important! It helps create room for the city to rebuild and prevents further issues in plumbing and roadways; And it keeps the citizens safe.",
        "Your job is the reason that the city continues to remain safe and operable after a typhoon, so do your best!",
    };

    //Have space change text
    public override void _Process(double delta)
    {
        // Check if the player pressed the space key or left mouse button
        if (Input.IsActionJustPressed("jump") || Input.IsActionJustPressed("m1"))
        {
            if (text.Length > 1)
            {
                //Text = text[0];
                //text = text[1..];
                text = text[1..];
                this.Text = "";
                textPos = 0;
                CutsceneSFX.Play();
                BottomTip.Modulate = new Color(1, 1, 1, 0);
            }
            else if ((Input.IsActionJustPressed("jump") || Input.IsActionJustPressed("m1")) && text.Length == 1)
                _transitionscene.Call("ChangeScene", "res://assets/level/testscene.tscn");
                //GetTree().ChangeSceneToFile("res://assets/level/testscene.tscn");
        }
        TypeText(text[0]);
    }

    public void TypeText(string text)
    {
        if (textPos >= 0 && textPos < text.Length) // checks if texting position is correct and so it doesn't go over the amount that needs to be typed
        {

            this.Text += text[textPos];
            textPos++; // moves to the next char in the str
        }
        else if (textPos >= text.Length) // go here once it goes over the amount typed (when it's done typing)
        {
            textPos = -1; // blocks typing until the person presses m1 or jump
            FadeIn();
        }
    }

    public void FadeIn()
    {
		_tween?.Kill();
		_tween = GetTree().CreateTween();
        _tween.TweenProperty(BottomTip, "modulate:a", 1, 1f);
        _tween.Play();
    }
}