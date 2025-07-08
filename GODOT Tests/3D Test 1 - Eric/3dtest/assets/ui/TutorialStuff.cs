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
        ["By using [b][color=misty_rose]Left Click[/color][/b], you can vacuum trash. Try filling any of the four tanks to 50% by vacuuming trash!"],
        ["Great job! Now, let's learn how to toss trash.", "Use [b][color=misty_rose]1-4[/color][/b] to select the type of trash you want to toss.", "Then, hold [b][color=misty_rose]Right Click[/color][/b] to charge up a throw.", "Finally, release the button to toss a bag of the selected trash!", "Your tank holds [b][color=#ff4e4a]Combustibles[/color][/b], [b][color=#57e792]Plastics[/color][/b], [b][color=#e9c261]PET Bottles[/color][/b], and [b][color=#5a98fa]Cans & Glass Bottles[/color][/b].", "Try throwing a bag of [b][color=#ff4e4a]Combustibles[/color][/b] into the correct trash bin!"],
        ["Amazing! To progress, toss trash into each colored bin."],
        ["Head to the flashing symbol on the map to continue your cleanup journey."],
        ["This area represents a major hazard.", "Open your phone with [b]'F'[/b] to learn more about it."],
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
            _textbox.Call("PopUp", _messages[step], 1);
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
