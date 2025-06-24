using Godot;
using System;
using System.Collections.Specialized;

public partial class Phone : Control
{

    private Button _agenciesButton;
    private Button _hazardsButton;
    private Button _dispatchButton;

    private Control _agenciesScreen;
    private Control _hazardsScreen;
    private Control _dispatchScreen;

    private Label _phoneText;

    private Control _UI;

    // Hazards
    private Button _PowerlineButton;
    private Button _ChemicalSpillButton;

    // Agencies
    private Button _PowerCompanyButton;
    private Button _FireDepartmentButton;



    public override void _Ready()
    {
        _agenciesButton = GetNode<Button>("PhoneSprite/Agencies Button");
        _hazardsButton = GetNode<Button>("PhoneSprite/Hazards Button");
        _dispatchButton = GetNode<Button>("PhoneSprite/Dial Button");

        _agenciesScreen = GetNode<Control>("PhoneSprite/Agencies Screen");
        _hazardsScreen = GetNode<Control>("PhoneSprite/Hazards Screen");
        _dispatchScreen = GetNode<Control>("PhoneSprite/Dial Screen");

        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = false;

        _phoneText = GetNode<Label>("PhoneSprite/Dial Screen/Label");

        _PowerlineButton = GetNode<Button>("PhoneSprite/Hazards Screen/Power");
        GD.Print("Powerline button: " + _PowerlineButton);
        _ChemicalSpillButton = GetNode<Button>("PhoneSprite/Hazards Screen/Chemical Spill");

        _PowerCompanyButton = GetNode<Button>("PhoneSprite/Agencies Screen/Power Company");
        _FireDepartmentButton = GetNode<Button>("PhoneSprite/Agencies Screen/Fire Department");


        _UI = GetParent().GetParent().GetNode<Control>("UI");

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
    }

    private void DispatchTabPressed()
    {

        _agenciesScreen.Visible = false;
        _hazardsScreen.Visible = false;
        _dispatchScreen.Visible = true;
    }

    private void InfoButtonPressed()
    {
        GD.Print("Button Pressed");
        if (_ChemicalSpillButton.IsPressed())
        {
            _UI.Call("ShowInfoSection", "Chemical Spill", "Chemical spills can be extremely dangerous. They can cause fires, explosions, and environmental damage. Always call the appropriate agency to handle them.");
        }
        if (_PowerlineButton.IsPressed())
        {
            _UI.Call("ShowInfoSection", "Powerline", "Powerlines are a major hazard that can cause serious damage if not handled properly. They can electrocute you and cause fires. Always call the appropriate agency to handle them.");

            if ((int)_UI.Call("GetTutorialStep") == 7)
            {

                _UI.Call("NextTutorialStep");
            }
        }
        if (_PowerCompanyButton.IsPressed())
        {
            _UI.Call("ShowInfoSection", "Power Company", "The power company is responsible for maintaining the power grid and handling powerline hazards. They can help you with any power-related issues. The code is ↑→↓←");
        }
        if (_FireDepartmentButton.IsPressed())
        {
            _UI.Call("ShowInfoSection", "Fire Department", "The fire department is responsible for handling fires and other emergencies. They can help you with any fire-related issues.");
        }


    }
}
