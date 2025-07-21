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

    [Export] public Timer WanderCooldownTimer;

    [Export] private AnimatedSprite3D _animations;


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
        NavAgent.PathDesiredDistance = 1f;
        NavAgent.TargetDesiredDistance = 1f;
        NavAgent.Radius = 0.5f;


        SetRandomWanderTarget();

        WanderCooldownTimer.Timeout += OnWanderTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isStunned)
        {
            _stunTimer -= (float)delta;
            if (_stunTimer <= 0f)
            {
                _isStunned = false;
                SetRandomWanderTarget();
            }
            Velocity = Vector3.Zero;
            MoveAndSlide();
            _animations.Play("idle");
            return;
        }

        float distToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

        // Player is too close - flee and drop trash if holding any
        if (distToPlayer < AvoidPlayerDistance)
        {
            if (_heldTrash != null)
                DropTrash();

            // Reset target trash to avoid chasing while fleeing
            _targetTrash = null;
            FleeFromPlayer();
            MoveTowards(NavAgent.GetNextPathPosition(), delta);
            _animations.Play("run");
            return;
        }

        // Currently fleeing - move to flee target and check for stopping flee
        if (_isFleeing)
        {
            // Make it flee towards trash bins
            MoveTowards(NavAgent.GetNextPathPosition(), delta);
            // CheckCrashIntoBins();

            // Stop fleeing when far enough and reset target trash
            if (distToPlayer > AvoidPlayerDistance * 2f)
            {
                _isFleeing = false;
                _targetTrash = null;
                SetRandomWanderTarget();
            }
            _animations.Play(_heldTrash != null ? "trash_run" : "run");
            return;
        }

        // Carrying trash - move to drop location
        if (_heldTrash != null)
        {
            NavAgent.TargetPosition = _currentTarget;
            MoveTowards(NavAgent.GetNextPathPosition(), delta);

            _animations.Play("trash_run");

            if (GlobalPosition.DistanceTo(_currentTarget) < 1f)
            {
                DropTrash();
                SetRandomWanderTarget();
            }
            return;
        }

        if (!IsTrashValid(_targetTrash))
        {
            _targetTrash = FindNearbyTrash();

        }


        if (_targetTrash != null)
        {
            NavAgent.TargetPosition = _targetTrash.GlobalPosition;
            MoveTowards(NavAgent.GetNextPathPosition(), delta);

            _animations.Play("run");

            // Check if close enough to pick up - double check trash is still valid
            if (_targetTrash != null && _targetTrash.IsInsideTree() && GlobalPosition.DistanceTo(_targetTrash.GlobalPosition) < 2f)
            {
                PickupTrash(_targetTrash);
            }
        }
        else
        {
            // No trash found - wander around
            if (NavAgent.IsNavigationFinished())
                SetRandomWanderTarget();

            MoveTowards(NavAgent.GetNextPathPosition(), delta);
            _animations.Play("idle");
        }
    }
    private void OnWanderTimerTimeout()
    {
        SetRandomWanderTarget();
    }

    private void SetRandomWanderTarget()
    {
        Vector3 randomOffset = new Vector3(
            (float)(_rand.NextDouble() * 20 - 10), 0,
            (float)(_rand.NextDouble() * 20 - 10)
        );
        Vector3 target = GlobalPosition + randomOffset;
        NavAgent.TargetPosition = target;
    }

    // private void MoveTowards(Vector3 targetPosition, double delta)
    // {
    //     if (NavAgent.IsNavigationFinished())
    //     {
    //         _animations.Play("idle");
    //         Velocity = Vector3.Zero;
    //         MoveAndSlide();
    //         return;
    //     }

    //     Vector3 nextPathPosition = NavAgent.GetNextPathPosition();

    //     Vector3 direction = (nextPathPosition - GlobalPosition).Normalized();
    //     Velocity = direction * Speed;
    //     MoveAndSlide();
    // }
    private void MoveTowards(Vector3 targetPosition, double delta)
    {
        if (NavAgent.IsNavigationFinished())
        {
            Velocity = Vector3.Zero;
            MoveAndSlide();
            return;
        }

        Vector3 navTarget = NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), targetPosition);
        NavAgent.TargetPosition = navTarget;
        Vector3 nextPathPosition = NavAgent.GetNextPathPosition();

        Vector3 direction = (nextPathPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }


    private void FleeFromPlayer()
    {
        Speed = 8f;
        _isFleeing = true;

        // Find nearest trash bin to sabotage while fleeing
        Node3D nearestBin = null;
        float nearestBinDist = float.MaxValue;

        foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
        {
            if (node is Node3D bin)
            {
                float dist = GlobalPosition.DistanceTo(bin.GlobalPosition);
                if (dist < nearestBinDist)
                {
                    nearestBin = bin;
                    nearestBinDist = dist;
                }
            }
        }

        Vector3 target;
        if (nearestBin != null)
        {
            // Flee towards the bin for sabotage opportunity
            Vector3 dirToBin = (nearestBin.GlobalPosition - GlobalPosition).Normalized();
            Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();

            // Blend flee direction with bin direction (60% flee, 40% towards bin)
            Vector3 blendedDir = (fleeDir * 0.6f + dirToBin * 0.4f).Normalized();
            target = GlobalPosition + blendedDir * 15f;
        }
        else
        {
            // Standard flee behavior
            Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();
            target = GlobalPosition + fleeDir * 10f;
        }

        // Snap to nearest navigable position on the NavMesh
        target = NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), target);
        NavAgent.TargetPosition = target;
    }

    bool IsTrashValid(Node trash) => trash != null && IsInstanceValid(trash);


    private RigidBody3D FindNearbyTrash()
    {
        RigidBody3D closest = null;
        float bestScore = float.MinValue;

        foreach (var node in GetTree().GetNodesInGroup("cleanable_vacuum"))
        {
            if (node is RigidBody3D trash && trash.IsInsideTree() && trash != _lastDroppedTrash)
            {
                float dist = GlobalPosition.DistanceTo(trash.GlobalPosition);
                float distToPlayer = Player.GlobalPosition.DistanceTo(trash.GlobalPosition);

                if (dist < DetectionRadius && distToPlayer > 2.5f)
                {
                    // Calculate concentration score - count nearby trash of same type
                    int trashType = (int)trash.Call("_GetTrashID");
                    int nearbyCount = 0;

                    foreach (var otherNode in GetTree().GetNodesInGroup("cleanable_vacuum"))
                    {
                        if (otherNode is RigidBody3D otherTrash && otherTrash != trash && otherTrash.IsInsideTree())
                        {
                            float otherDist = trash.GlobalPosition.DistanceTo(otherTrash.GlobalPosition);
                            int otherType = (int)otherTrash.Call("_GetTrashID");

                            if (otherDist < 8f && otherType == trashType)
                                nearbyCount++;
                        }
                    }

                    // Score based on concentration (more nearby = better) and distance (closer = better)
                    float concentrationBonus = nearbyCount * 2f;
                    float distancePenalty = dist * 0.1f;
                    float score = concentrationBonus - distancePenalty;

                    if (score > bestScore)
                    {
                        closest = trash;
                        bestScore = score;
                    }
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
        _animations.Play("trash_idle");
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

        // appears as if the trash is tossed
        _heldTrash.ApplyImpulse(Vector3.Up, new Vector3(
            (float)_rand.NextDouble() * 4f - 2f,
            1,
            (float)_rand.NextDouble() * 4f - 2f
        ));

        _animations.Play("idle");

        _lastDroppedTrash = _heldTrash;
        _heldTrash = null;

        SetRandomWanderTarget();
    }

    private Vector3 GetTrashDropLocation()
    {
        if (_heldTrash == null)
            return GlobalPosition;

        int trashType = (int)_heldTrash.Call("_GetTrashID");


        // Finds the trash's corresponding bin type (change to array later)
        Node3D correctBin = null;
        foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
        {
            if (node is Node3D bin)
            {
                int binType = (int)bin.Call("GetBinType");
                if (binType == trashType)
                {
                    correctBin = bin;
                    break;
                }
            }
        }


        Vector3 desired;
        if (correctBin != null)
        {
            Vector3 dirAwayFromBin = (GlobalPosition - correctBin.GlobalPosition).Normalized();
            desired = correctBin.GlobalPosition + dirAwayFromBin * (DropDistance + 5f);
        }
        else
        {
            // No matching bin found, drop randomly
            desired = GlobalPosition + new Vector3(
                (float)_rand.NextDouble() * 2f - 1f,
                0,
                (float)_rand.NextDouble() * 2f - 1f
            ).Normalized() * DropDistance;
        }

        // Snap to nearest navigable position on the NavMesh
        Vector3 snapped = NavigationServer3D.MapGetClosestPoint(
            NavAgent.GetNavigationMap(), desired
        );

        // Ensure it's still on the ground (not up on a bush)
        if (Mathf.Abs(snapped.Y - GlobalPosition.Y) > 1.5f)
        {
            // Too much vertical difference — fallback to self
            return GlobalPosition;
        }

        return snapped;
    }

    // private void CheckCrashIntoBins()
    // {
    //     foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
    //     {
    //         if (node is Node3D bin && GlobalPosition.DistanceTo(bin.GlobalPosition) < SabotageCrashRange)
    //         {
    //             GD.Print("Raccoon crashed into bin!");

    //             // // Trigger bin sabotage - call method on the bin to spill trash
    //             // if (bin.HasMethod("_RaccoonSabotage"))
    //             // {
    //             //     bin.Call("_RaccoonSabotage");
    //             // }

    //             // // Create some visual/audio feedback
    //             // if (bin.HasMethod("_PlaySabotageEffect"))
    //             // {
    //             //     bin.Call("_PlaySabotageEffect");
    //             // }

    //             // Reduce game completion progress (optional)
    //             var ui = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI");
    //             if (ui != null && ui.HasMethod("IncreaseGameCompletion"))
    //             {
    //                 ui.Call("IncreaseGameCompletion", -10);
    //             }

    //             // Stun the raccoon briefly from the impact
    //             _isStunned = true;
    //             _stunTimer = 2f;

    //             _isFleeing = false;
    //             SetRandomWanderTarget();
    //             break;
    //         }
    //     }
    // }
}
