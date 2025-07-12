using Godot;
using System;

public partial class RaccoonAgent : CharacterBody3D
{

    [Export] public float AvoidPlayerDistance = 5f;
    [Export] public CharacterBody3D Player;

    private RigidBody3D _lastDroppedTrash;
    private bool _isFleeing = false;

    [Export] public float Speed = 5f;
    [Export] public float DropDistance = 5f;
    private RigidBody3D _heldTrash;
    private RigidBody3D _targetTrash;
    private Vector3 _dropTarget;
    private Random _rand = new();
    private Timer _searchTimer;

    public override void _Ready()
    {
        _searchTimer = GetNode<Timer>("SearchTimer");
        _searchTimer.Timeout += OnSearchTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        float distToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

        if (distToPlayer < AvoidPlayerDistance)
        {
            FleeFromPlayer(delta);
            return;
        }

        _isFleeing = false;

        if (_heldTrash == null && _targetTrash != null)
        {
            if (!IsInstanceValid(_targetTrash))
            {
                _targetTrash = null;
                return;
            }

            MoveTo(_targetTrash.GlobalPosition, delta);
        }
        else if (_heldTrash != null)
        {
            MoveTo(_dropTarget, delta);

            if (GlobalPosition.DistanceTo(_dropTarget) < 1f)
                DropTrash();
        }
    }


    private void MoveTo(Vector3 target, double delta)
    {
        Vector3 flatTarget = new Vector3(target.X, GlobalPosition.Y, target.Z);
        Vector3 direction = (flatTarget - GlobalPosition).Normalized();

        Velocity = direction * Speed;
        MoveAndSlide();
    }


    private void OnSearchTimeout()
    {
        if (_heldTrash != null) return;
        _targetTrash = FindNearbyTrash();
    }


    private RigidBody3D FindNearbyTrash()
    {
        RigidBody3D closest = null;
        float closestDist = float.MaxValue;

        foreach (var node in GetTree().GetNodesInGroup("cleanable_vacuum"))
        {
            if (node is RigidBody3D trash && trash.IsInsideTree() && trash != _lastDroppedTrash)
            {
                float dist = trash.GlobalPosition.DistanceTo(GlobalPosition);
                float distToPlayer = trash.GlobalPosition.DistanceTo(Player.GlobalPosition);

                if (dist < 15f && dist < closestDist && distToPlayer > 2.5f)
                {
                    closest = trash;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    private void PickupTrash(RigidBody3D trash)
    {
        _heldTrash = trash;
        _targetTrash = null;

        trash.Freeze = true;
        trash.Visible = false;
        trash.SetProcess(false);

        _dropTarget = GlobalPosition + new Vector3(
            (float)_rand.NextDouble() * 2f - 1f,
            0,
            (float)_rand.NextDouble() * 2f - 1f
        ).Normalized() * DropDistance;
    }

    private void DropTrash()
    {
        _heldTrash.GlobalPosition = _dropTarget;
        _heldTrash.Freeze = false;
        _heldTrash.Visible = true;
        _heldTrash.SetProcess(true);
        _heldTrash.ApplyImpulse(Vector3.Up, new Vector3(
            (float)_rand.NextDouble() * 4f - 2f,
            1,
            (float)_rand.NextDouble() * 4f - 2f
        ));

        _lastDroppedTrash = _heldTrash;
        _heldTrash = null;
    }


    public override void _Process(double delta)
    {
        if (_heldTrash == null && _targetTrash != null)
        {
            if (IsInstanceValid(_targetTrash))
            {
                if (GlobalPosition.DistanceTo(_targetTrash.GlobalPosition) < 1f)
                {
                    PickupTrash(_targetTrash);
                }
            }
            else
            {
                _targetTrash = null;
            }

        }
    }

    private void FleeFromPlayer(double delta)
    {
        _isFleeing = true;
        Vector3 playerFlat = new Vector3(Player.GlobalPosition.X, GlobalPosition.Y, Player.GlobalPosition.Z);
        Vector3 fleeDir = (GlobalPosition - playerFlat).Normalized();

        Velocity = fleeDir * Speed * 1.5f;
        MoveAndSlide();

        _targetTrash = null;
    }


}
