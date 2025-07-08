using Godot;
using System.Collections.Generic;

public partial class TutorialStuff : MarginContainer
{
    private AudioStreamPlayer TextBubbleSFX;
    private Control _textbox;

    private int _step = 0;

    private string[][] _messages =
    [
        ["Welcome! Move around using [b]WASD[/b].", "Try heading over to the blue beacon over there!"],
        ["By using [b][color=cyan]Left Click[/color][/b], you can pick up trash. Try filling any of the four tanks to 50% by picking up trash!"],
        ["Great job! Now, let's learn how to toss trash. Hold [b]Right Click[/b] and aim towards the red bin."],
        ["Amazing! To progress, throw trash into each colored bin."],
        ["Head to the flashing symbol on the map to continue your cleanup journey."],
        ["This area represents a major hazard. Open your phone with [b]'F'[/b] to learn more about it."],
        ["Navigate to the 'Hazards' tab in your phone to search all possible hazards."],
        ["Now cick on the 'Powerline' hazard to learn more about it."],
        ["Now click the 'Agencies' tab in your phone to learn about the agencies that can help you."],
        ["Head to the 'Dispatch' tab in your phone and enter the correct agencies code."],
        ["Great job! You now know how to deal with hazards! Thanks for playing our prototype!"],
    ];

    public override void _Ready()
    {
        TextBubbleSFX = GetParent().GetNode<AudioStreamPlayer>("TextBubbleSFX");
        _textbox = GetParent().GetNode<Control>("Textbox");
        ShowStep(_step);
    }

    public void ShowStep(int step)
    {
        if (step < _messages.Length)
        {
            _textbox.Call("PopUp", _messages[step], 0);
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
