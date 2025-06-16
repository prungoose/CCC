using Godot;
using System;

public partial class NPC : Area3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("NPC ready: ", Name, " | In tree: ", IsInsideTree());

    }
    bool playerEntry = false;
    public override void _Process(double delta)
    {
        //GD.Print(playerEntry);
        if (playerEntry && Input.IsActionPressed("jump"))
        {

            GD.Print("NPC interaction triggered.");
            GetTree().ChangeSceneToFile("res://assets/level/visualNovelDisplay.tscn");
        }
    }

    private void _OnPlayerEntry(Node3D body)
    {
        GD.Print($"{body.Name} entered the npc area.");
        if (body.Name == "Player")
        {
            playerEntry = true;
        }
    }
    private void _OnPlayerExit(Node3D body)
    {
        GD.Print($"{body.Name} exited the npc area.");
        if (body.Name == "Player")
        {
            playerEntry = false;
        }
    }
}

