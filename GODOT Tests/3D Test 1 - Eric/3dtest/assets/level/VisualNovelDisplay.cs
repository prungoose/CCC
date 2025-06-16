using Godot;
using System.Collections.Generic;

public partial class VisualNovelDisplay : CanvasLayer
{
    [Export] public TextureRect PlayerSprite;
    [Export] public TextureRect NPCSprite;
    [Export] public RichTextLabel DialogueLabel;

    private Queue<string> dialogueQueue = new();
    private bool showingDialogue = false;

    public override void _Ready()
    {
        Hide();
    }

    public void StartDialogue(List<string> lines)
    {
        dialogueQueue.Clear();

        foreach (var line in lines)
            dialogueQueue.Enqueue(line);

        Show();
        ShowNextLine();
    }

    public override void _Input(InputEvent @event)
    {
        if (showingDialogue && Input.IsActionJustPressed("jump"))
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        if (dialogueQueue.Count > 0)
        {
            DialogueLabel.Text = dialogueQueue.Dequeue();
        }
        else
        {
            DialogueLabel.Text = "";
            Hide();
            showingDialogue = false;
        }
    }
}
