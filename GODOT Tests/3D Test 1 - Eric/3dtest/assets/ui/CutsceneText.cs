using Godot;
using System;

public partial class CutsceneText : RichTextLabel
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("preSequenceText ready: ", Name, " | In tree: ", IsInsideTree());
        // Set the text to be displayed
        Text = "Welcome to Crisis Cleanup Crew! This is our test playground where we’ll teach you everything we know before assigning you to your first job!";
        // Show the text
        Show();
    }

    // Text to be displayed in the visual novel.
    private string[] text = {
        "Cleaning up after a natural disaster is very important! It helps create room for the city to rebuild and prevents further issues in plumbing and roadways; And it keeps the citizens safe.",
        "Your job is the reason that the city continues to remain safe and operable after a typhoon, so do your best!",
    };

    //Have space change text
    public override void _Process(double delta)
    {
        // Check if the player pressed the space key or left mouse button
        if (Input.IsActionJustPressed("jump") || Input.IsActionJustPressed("m1"))
        {
            if (text.Length > 0)
            {
                Text = text[0];
                text = text[1..];
            }
            else GetTree().ChangeSceneToFile("res://assets/level/testscene.tscn");
        }
    }
}