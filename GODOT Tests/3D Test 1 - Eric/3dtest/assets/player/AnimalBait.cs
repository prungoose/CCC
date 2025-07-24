using Godot;
using System;

public partial class AnimalBait : RigidBody3D
{
    [Export] private Area3D _attractionArea;
    [Export] private CollisionShape3D _areaShape;
    private bool _isLanded = false;
    private float _landedTimer = 0f;
    private const float TimeUntilActive = 0.25f;

    public override void _Ready()
    {
        _areaShape.Disabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isLanded) return;

        if (LinearVelocity.Length() < 0.1f)
        {
            _landedTimer += (float)delta;
            if (_landedTimer > TimeUntilActive)
            {
                _isLanded = true;
                _areaShape.Disabled = false;
                GD.Print("Bait is now active");
            }
        }
        else
        {
            _landedTimer = 0;
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is RaccoonAgent raccoon)
        {
            GD.Print("Bait has lured raccoon");
            raccoon.Call("LureBait", this.GlobalPosition);

            _areaShape.Disabled = true;
            QueueFree();
        }
    }
}
