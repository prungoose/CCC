using Godot;
using System;

public partial class Phone : Control
{
    // Application Buttons
    [Export] private Button _agenciesButton;
    [Export] private Button _hazardsButton;
    [Export] private Button _dispatchButton;

    // Application Screens
    [Export] private Control _agenciesScreen;
    [Export] private Control _hazardsScreen;
    [Export] private Control _dispatchScreen;
    [Export] private Control _homeScreen;

    // Hazard Info
    [Export] private Control _hazardOptions;
    [Export] private Button _powerlineButton;
    [Export] private Button _chemicalSpillButton;
    [Export] private Button _fallenTreeButton;

    // Agency Info
    [Export] private Control _agencyOptions;
    [Export] private Button _powerCompanyButton;
    [Export] private Button _fireDepartmentButton;
    [Export] private Button _emsButton;

    // Misc
    [Export] private Label _dialLabel;
    [Export] private Button _backButton;
    [Export] private RichTextLabel _titleText;
    [Export] private RichTextLabel _descriptionText;

    // Internal State
    private bool _agencyInfoOpen = false;
    private bool _hazardInfoOpen = false;

    // External
    private Control _ui;
    private AudioStreamPlayer _phoneSfx;

    public override void _Ready()
    {
        // Initial screen visibility
        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = false;
        _homeScreen.Visible = true;
        _backButton.Visible = false;

        // Fetch references
        _ui = GetNodeOrNull<Control>("../../UI");
        _phoneSfx = GetNodeOrNull<AudioStreamPlayer>("PhoneSFX");

        if (_ui == null)
            GD.PushWarning("UI node not found.");
        if (_phoneSfx == null)
            GD.PushWarning("PhoneSFX node not found.");
    }

    private void PlaySfx() => _phoneSfx?.Play();

    private void AgenciesTabPressed()
    {
        if ((int)_ui?.Call("GetTutorialStep") == 8)
            _ui.Call("NextTutorialStep");


        ShowScreen(_agenciesScreen);
    }

    private void HazardsTabPressed()
    {
        if ((int)_ui?.Call("GetTutorialStep") == 6)
            _ui.Call("NextTutorialStep");


        ShowScreen(_hazardsScreen);
    }

    private void DispatchTabPressed()
    {

        ShowScreen(_dispatchScreen);
    }

    private void ShowScreen(Control screenToShow)
    {
        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = false;
        _homeScreen.Visible = false;

        screenToShow.Visible = true;
        _backButton.Visible = true;

        if (screenToShow == _agenciesScreen)
        {
            _agencyOptions.Visible = true;
            _hazardOptions.Visible = false;
        }
        else if (screenToShow == _hazardsScreen)
        {
            _hazardOptions.Visible = true;
            _agencyOptions.Visible = false;
        }
        else
        {
            _agencyOptions.Visible = false;
            _hazardOptions.Visible = false;
        }

        PlaySfx();
    }

    private void InfoButtonPressed()
    {
        if (_chemicalSpillButton.IsPressed())
        {
            SetInfo(
                "[b][color=red]Chemical Spill[/color][/b]",
                "Chemical spills can be [b]extremely dangerous[/b].\n\nThey may cause [i]fires[/i], [i]explosions[/i], or long-term [color=orange]environmental damage[/color].\n\nIf you see a chemical spill:\n• [b]Avoid the area immediately[/b]\n• Call the proper agency for cleanup\n• Do not touch or inhale fumes"
            );
            ToggleHazardInfo();
        }

        if (_powerlineButton.IsPressed())
        {
            SetInfo(
                "[b][color=yellow]Downed Powerline[/color][/b]",
                "Downed powerlines are [b]high voltage hazards[/b] that can cause electrocution or start fires.\n\nIf you see one:\n• [b]Stay far away[/b]\n• Do [i]not[/i] touch water or metal near it\n• Report it to the [color=cyan]Power Company[/color]"
            );
            ToggleHazardInfo();

            if ((int)_ui?.Call("GetTutorialStep") == 7)
                _ui.Call("NextTutorialStep");
        }

        if (_fallenTreeButton.IsPressed())
        {
            SetInfo(
                "[b][color=green]Fallen Tree[/color][/b]",
                "Fallen trees can block roads and damage property or power lines.\n\nWhen you see one:\n• [b]Keep a safe distance[/b] from hanging wires\n• Call the appropriate agency\n• Do not attempt to move it yourself"
            );
            ToggleHazardInfo();
        }

        if (_powerCompanyButton.IsPressed())
        {
            SetInfo(
                "[b][color=cyan]Power Company[/color][/b]",
                "The Power Company maintains the [i]electric grid[/i] and responds to issues like [b]downed lines[/b], [b]blackouts[/b], or [b]transformer damage[/b].\n\n• Contact them for electrical hazards\n• Do not handle power lines yourself\n\n[b]Agency Access Code:[/b] ↑ → ↓ ←"
            );
            ToggleAgencyInfo();
        }

        if (_fireDepartmentButton.IsPressed())
        {
            SetInfo(
                "[b][color=orange]Fire Department[/color][/b]",
                "The Fire Department responds to:\n• [b]Fires[/b]\n• [b]Rescue operations[/b]\n• [b]Hazardous spills[/b]\n\nThey have the training and equipment to handle life-threatening hazards quickly.\n\n[b]Agency Access Code:[/b] ↓ ← ↑ →"
            );
            ToggleAgencyInfo();
        }

        if (_emsButton.IsPressed())
        {
            SetInfo(
                "[b][color=lightblue]Emergency Medical Services (EMS)[/color][/b]",
                "EMS provides [b]life-saving care[/b] in emergencies.\n\nThey respond to:\n• [i]Injuries[/i]\n• [i]Exposure to toxins[/i]\n• [i]Fainting or collapse[/i]\n\nCall EMS if someone is hurt or unconscious.\n\n[b]Agency Access Code:[/b] ↓ ↓ ↑ ↑"
            );
            ToggleAgencyInfo();
        }

        PlaySfx();
    }

    private void SetInfo(string title, string description)
    {
        _titleText.Text = title;
        _descriptionText.Text = description;
    }

    private void ToggleAgencyInfo()
    {
        _agencyOptions.Visible = false;
    }

    private void ToggleHazardInfo()
    {
        _hazardOptions.Visible = false;
    }

    private void BackButtonPressed()
    {
        if (_agenciesScreen.Visible || _hazardsScreen.Visible || _dispatchScreen.Visible)
        {
            _agenciesScreen.Visible = false;
            _hazardsScreen.Visible = false;
            _dispatchScreen.Visible = false;
            _homeScreen.Visible = true;
            _backButton.Visible = false;
            _titleText.Text = "";
            _descriptionText.Text = "";
        }

        PlaySfx();
    }
}

