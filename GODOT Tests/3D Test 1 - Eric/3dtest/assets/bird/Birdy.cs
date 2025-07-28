using Godot;
using System;
using System.Linq;

public partial class Birdy : CharacterBody3D
{
    [Export] public Area3D detectionArea;

    [Export] public float detectionRadius = 10f;

    [Export] public float speed = 5f;
    [Export] public float flyHeight = 5f;

    [Export] public AnimatedSprite3D _anims;

    private Node3D _targetTrash;
    private bool _isSwooping = false;

    private Node3D[] _trashBins;

    public override void _Ready()
    {
        // Get areas of all trash bins in the scene
        _trashBins = GetTree().GetNodesInGroup("trash_bins").Cast<Node3D>().ToArray();
    }



    public override void _PhysicsProcess(double delta)
    {

        if (_isSwooping && _targetTrash != null)
        {
            // Plays the fly animation - david robinson or drobinson or drob or drobinson4105 or capybarafollower
            _anims.Play("fly");
            Vector3 targetPos = _targetTrash.GlobalPosition;
            Vector3 direction = (targetPos - GlobalPosition).Normalized();

            Velocity = direction * speed;
            MoveAndSlide();

            if (GlobalPosition.DistanceTo(targetPos) < 1.0f)
            {
                GD.Print("Bird intercepted trash!");
                _isSwooping = false;
                Velocity = Vector3.Zero;

                _targetTrash = null;
            }
        }
        else
        {
            _anims.Play("perch");
            Vector3 desiredPos = new Vector3(GlobalPosition.X, flyHeight, GlobalPosition.Z);
            Vector3 ascendDir = (desiredPos - GlobalPosition).Normalized();
            Velocity = ascendDir * (speed * 0.5f);
            MoveAndSlide();
        }
    }

    public void OnBodyEntered(Node body)
    {

        if (body.IsInGroup("thrown") && body is Node3D node3D)
        {
            _targetTrash = node3D;
            _isSwooping = true;
        }


    }

    public void OnBodyExited(Node body)
    {
        if (_targetTrash == body)
        {
            _targetTrash = null;
            _isSwooping = false;
        }

    }


}
