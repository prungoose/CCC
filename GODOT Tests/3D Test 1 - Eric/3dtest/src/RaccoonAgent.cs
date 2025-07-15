using Godot;
using System;

public partial class RaccoonAgent : CharacterBody3D
{
    [Export] public float Speed = 5f;
    [Export] public float AvoidPlayerDistance = 5f;
    [Export] public float DropDistance = 5f;
    [Export] public float DetectionRadius = 15f;
    [Export] public float SabotageCrashRange = 1f;
    [Export] public float StunDuration = 20f;
    [Export] public CharacterBody3D Player;
    [Export] public NavigationAgent3D NavAgent;

    private Vector3 _currentTarget;
    private RigidBody3D _targetTrash;
    private RigidBody3D _heldTrash;
    private RigidBody3D _lastDroppedTrash;
    private bool _isFleeing = false;
    private bool _isStunned = false;
    private float _stunTimer = 0f;
    private Random _rand = new();

    public override void _Ready()
    {
        NavAgent.PathDesiredDistance = 0.5f;
        NavAgent.TargetDesiredDistance = 0.5f;
        SetRandomWanderTarget();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isStunned)
        {
            _stunTimer -= (float)delta;
            if (_stunTimer <= 0f) _isStunned = false;
            return;
        }

        float distToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

        if (distToPlayer < AvoidPlayerDistance)
        {
            if (_heldTrash != null)
                DropTrash();

            FleeFromPlayer();
            return;
        }

        if (_isFleeing)
        {
            MoveTowards(NavAgent.GetNextPathPosition(), delta);
            CheckCrashIntoBins();
            return;
        }

        // Trash interaction logic
        if (_heldTrash != null)
        {
            MoveTowards(_currentTarget, delta);

            if (GlobalPosition.DistanceTo(_currentTarget) < 1f)
                DropTrash();
        }
        else
        {
            if (_targetTrash == null)
                _targetTrash = FindNearbyTrash();

            if (_targetTrash != null)
            {
                NavAgent.TargetPosition = _targetTrash.GlobalPosition;
                MoveTowards(NavAgent.GetNextPathPosition(), delta);

                if (GlobalPosition.DistanceTo(_targetTrash.GlobalPosition) < 1f)
                    PickupTrash(_targetTrash);
            }
            else if (!NavAgent.IsNavigationFinished())
            {
                MoveTowards(NavAgent.GetNextPathPosition(), delta);
            }
            else
            {
                SetRandomWanderTarget();
            }
        }
    }

    private void MoveTowards(Vector3 target, double delta)
    {
        Vector3 direction = (target - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    private void SetRandomWanderTarget()
    {
        Vector3 randomOffset = new Vector3(
            (float)(_rand.NextDouble() * 40 - 20),
            0,
            (float)(_rand.NextDouble() * 40 - 20)
        );
        NavAgent.TargetPosition = GlobalPosition + randomOffset;
    }

    private void FleeFromPlayer()
    {
        _isFleeing = true;
        Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();
        Vector3 target = GlobalPosition + fleeDir * 10f;
        NavAgent.TargetPosition = target;
    }

    private void CheckCrashIntoBins()
    {
        foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
        {
            if (node is Node3D bin && GlobalPosition.DistanceTo(bin.GlobalPosition) < SabotageCrashRange)
            {
                GD.Print("Raccoon crashed into bin!");
                // Trigger bin sabotage logic here
                _isFleeing = false;
                SetRandomWanderTarget();
                break;
            }
        }
    }

    private RigidBody3D FindNearbyTrash()
    {
        RigidBody3D closest = null;
        float closestDist = float.MaxValue;

        foreach (var node in GetTree().GetNodesInGroup("cleanable_vacuum"))
        {
            if (node is RigidBody3D trash && trash.IsInsideTree() && trash != _lastDroppedTrash)
            {
                float dist = GlobalPosition.DistanceTo(trash.GlobalPosition);
                float distToPlayer = Player.GlobalPosition.DistanceTo(trash.GlobalPosition);

                if (dist < DetectionRadius && distToPlayer > 2.5f && dist < closestDist)
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

        _currentTarget = GetTrashDropLocation();
        NavAgent.TargetPosition = _currentTarget;
    }

    private void DropTrash()
    {
        _heldTrash.GlobalPosition = _currentTarget;
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

        SetRandomWanderTarget();
    }

    private Vector3 GetTrashDropLocation()
    {
        return GlobalPosition + new Vector3(
            (float)_rand.NextDouble() * 2f - 1f,
            0,
            (float)_rand.NextDouble() * 2f - 1f
        ).Normalized() * DropDistance;
    }

    public void AttractToBait(Vector3 baitPosition)
    {
        NavAgent.TargetPosition = baitPosition;
        _isFleeing = false;
        _targetTrash = null;
    }

    public void Stun()
    {
        _isStunned = true;
        _stunTimer = StunDuration;
        Velocity = Vector3.Zero;
    }
}
