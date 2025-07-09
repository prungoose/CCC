using Godot;
using System;
using System.Collections.Specialized;
using System.Security.Cryptography;

public partial class Phone : Control
{


    // Application Buttons
    [Export] private Button _agenciesButton;
    [Export] private Button _hazardsButton;
    [Export] private Button _dispatchButton;

    // Application Screens (For toggling visibility)
    [Export] private Control _agenciesScreen;
    [Export] private Control _hazardsScreen;
    [Export] private Control _dispatchScreen;
    [Export] private Control _homeScreen;

    // Hazards
    [Export] private Control _hazardOptions;
    [Export] private Button _PowerlineButton;
    [Export] private Button _ChemicalSpillButton;

    // Agencies
    [Export] private Control _agencyOptions;
    [Export] private Button _PowerCompanyButton;
    [Export] private Button _FireDepartmentButton;

    // Misc
    [Export] private Label _dialLabel;
    [Export] private Button _backButton;
    [Export] private RichTextLabel _titleText;
    [Export] private RichTextLabel _descriptionText;

    private Boolean _agencieInfoOpen = false;
    private Boolean _hazardInfoOpen = false;

    // External
    private Control _UI;
    private AudioStreamPlayer PhoneSFX;



    public override void _Ready()
    {
        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = false;
        _homeScreen.Visible = true;
        _backButton.Visible = false;

        _UI = GetParent().GetParent().GetNode<Control>("UI");
        PhoneSFX = GetNode<AudioStreamPlayer>("PhoneSFX");
    }

    private void AgenciesTabPressed()
    {
        if ((int)_UI.Call("GetTutorialStep") == 8)
        {
            _UI.Call("NextTutorialStep");
        }
        _agenciesScreen.Visible = true;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = false;
        _homeScreen.Visible = false;
        _backButton.Visible = true;

        PhoneSFX.Play();
    }

    private void HazardsTabPressed()
    {
        if ((int)_UI.Call("GetTutorialStep") == 6)
        {
            _UI.Call("NextTutorialStep");
        }
        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = true;
        _dispatchScreen.Visible = false;
        _homeScreen.Visible = false;
        _backButton.Visible = true;


        PhoneSFX.Play();
    }

    private void DispatchTabPressed()
    {

        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = true;
        _homeScreen.Visible = false;
        _backButton.Visible = true;


        PhoneSFX.Play();
    }


    // Changing this to internal text on the phone directly

    private void InfoButtonPressed()
    {
        GD.Print("Button Pressed");
        if (_ChemicalSpillButton.IsPressed())
        {
            _titleText.Text = "Chemical Spill";
            _descriptionText.Text = "Chemical spills can be extremely dangerous. They can cause fires, explosions, and environmental damage. Always call the appropriate agency to handle them.";
            showInfoSection();
        }
        if (_PowerlineButton.IsPressed())
        {
            _titleText.Text = "Powerline";
            _descriptionText.Text = "Powerlines are a major hazard that can cause serious damage if not handled properly. They can electrocute you and cause fires. Always call the appropriate agency to handle them.";
            showInfoSection();

            if ((int)_UI.Call("GetTutorialStep") == 7)
            {

                _UI.Call("NextTutorialStep");
            }
        }
        if (_PowerCompanyButton.IsPressed())
        {
            _titleText.Text = "Power Company";
            _descriptionText.Text = "The power company is responsible for maintaining the power grid and handling powerline hazards. They can help you with any power-related issues. The code is ↑→↓←";
            showInfoSection();
        }
        if (_FireDepartmentButton.IsPressed())
        {
            _titleText.Text = "Fire Department";
            _descriptionText.Text = "The fire department is responsible for handling fires and other emergencies. They can help you with any fire-related issues.";
            showInfoSection();
        }

        PhoneSFX.Play();
    }

    private void showInfoSection()
    {
        if (_agencieInfoOpen)
        {
            _agencyOptions.Visible = false;
            _agencieInfoOpen = false;
        }
        else
        {
            _agencyOptions.Visible = true;
            _agencieInfoOpen = true;
        }

        if (_hazardInfoOpen)
        {
            _hazardOptions.Visible = false;
            _hazardInfoOpen = false;
        }
        else
        {
            _hazardOptions.Visible = true;
            _hazardInfoOpen = true;
        }
    }

    private void BackButtonPressed()
    {
        if (_agenciesScreen.Visible)
        {
            _agenciesScreen.Visible = false;
            _homeScreen.Visible = true;
        }
        else if (_hazardsScreen.Visible)
        {
            _hazardsScreen.Visible = false;
            _homeScreen.Visible = true;
        }
        else if (_dispatchScreen.Visible)
        {
            _dispatchScreen.Visible = false;
            _homeScreen.Visible = true;
        }
    }
}
