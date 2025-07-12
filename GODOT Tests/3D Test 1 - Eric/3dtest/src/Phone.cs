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

    // Agency Info
    [Export] private Control _agencyOptions;
    [Export] private Button _powerCompanyButton;
    [Export] private Button _fireDepartmentButton;

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
                "Chemical Spill",
                "Chemical spills can be extremely dangerous. They can cause fires, explosions, and environmental damage. Always call the appropriate agency to handle them."
            );
            ToggleHazardInfo();
        }

        if (_powerlineButton.IsPressed())
        {

            SetInfo(
                "Powerline",
                "Powerlines are a major hazard that can cause serious damage if not handled properly. They can electrocute you and cause fires. Always call the appropriate agency to handle them."
            );
            ToggleHazardInfo();

            if ((int)_ui?.Call("GetTutorialStep") == 7)
                _ui.Call("NextTutorialStep");
        }

        if (_powerCompanyButton.IsPressed())
        {

            SetInfo(
                "Power Company",
                "The power company is responsible for maintaining the power grid and handling powerline hazards. They can help you with any power-related issues. The code is ↑→↓←"
            );
            ToggleAgencyInfo();
        }

        if (_fireDepartmentButton.IsPressed())
        {

            SetInfo(
                "Fire Department",
                "The fire department is responsible for handling fires and other emergencies. They can help you with any fire-related issues."
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
