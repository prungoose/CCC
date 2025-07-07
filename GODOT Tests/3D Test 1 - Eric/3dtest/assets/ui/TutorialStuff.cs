using Godot;
using System.Collections.Generic;

public partial class TutorialStuff : MarginContainer
{
    [Export] public RichTextLabel TutorialText;
    [Export] public Control TutorialUI;
    private AudioStreamPlayer TextBubbleSFX;

    private int _step = 0;

    private List<string> _messages = new()
    {
        "Welcome! Move around using WASD. Try heading over to the blue beacon over there!",
        "By using left click, you can pick up trash. Try filling any of the four tanks to 50% by picking up trash!",
        "Great job! Now, let's learn how to toss trash. Hold right click and aim towards the red bin.",
        "Amazing! To progress, throw trash into each colored bin.",
        "Head to the flashing symbol on the map to continue your cleanup journey.",
        "This area represents a major hazard. Open your phone with 'F' to learn more about it.",
        "Navigate to the 'Hazards' tab in your phone to search all possible hazards.",
        "Now cick on the 'Powerline' hazard to learn more about it.",
        "Now click the 'Agencies' tab in your phone to learn about the agencies that can help you.",
        "Head to the 'Dispatch' tab in your phone and enter the correct agencies code.",
        "Great job! You now know how to deal with hazards! Thanks for playing our prototype!",
    };

    public override void _Ready()
    {
        TextBubbleSFX = GetParent().GetNode<AudioStreamPlayer>("TextBubbleSFX");
        ShowStep(_step);
    }

    public void ShowStep(int step)
    {
        if (step < _messages.Count)
        {
            TextBubbleSFX.Play();
            TutorialText.Text = _messages[step];
            TutorialUI.Visible = true;
        }
        else
        {
            TutorialUI.Visible = false;
        }
    }

    public void NextStep()
    {
        _step++;
        ShowStep(_step);
    }

    public int GetTutorialStep()
    {
        return _step;
    }
}
