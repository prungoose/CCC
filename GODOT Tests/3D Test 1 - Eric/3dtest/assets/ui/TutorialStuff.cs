using Godot;
using System.Collections.Generic;

public partial class TutorialStuff : MarginContainer
{
    [Export] public RichTextLabel TutorialText;
    [Export] public Control TutorialUI;

    private int _step = 0;

    private List<string> _messages = new()
    {
        "Welcome! Move around using WASD. Try heading over to the blue beacon over there!",
        "By using left click, you can pick up trash. Try filling your tank to 50% by picking up trash!",
        "Great job! Now, let's learn how to toss trash. Hold right click and aim towards the red bin.",
        "Amazing! To progress, throw trash into each colored bin.",
        "Head to the flashing symbol on the map to continue your cleanup journey.",
    };

    public override void _Ready()
    {
        ShowStep(_step);
    }

    public void ShowStep(int step)
    {
        if (step < _messages.Count)
        {
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
