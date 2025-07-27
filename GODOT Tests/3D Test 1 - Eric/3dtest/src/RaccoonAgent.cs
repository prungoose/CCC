using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;

public partial class RaccoonAgent : CharacterBody3D
{
    [Export] public float Speed = 5f;
    [Export] public float FleeSpeedMultiplier = 1.6f;
    [Export] public float AvoidPlayerDistance = 5f;
    [Export] public float StopFleeingDistanceMultiplier = 2f;
    [Export] public float AvoidBinDistance = 10f;
    [Export] public float DropDistance = 5f;
    [Export] public float DetectionRadius = 15f;
    [Export] public float SabotageCrashRange = 1f;
    [Export] public float StunDuration = 20f;
    [Export] public CharacterBody3D Player;
    [Export] public NavigationAgent3D NavAgent;
    [Export] public Timer WanderCooldownTimer;
    [Export] private AnimatedSprite3D _animations;
    [Export] private Node3D _camera;

    // Stuck Detection Params
    [Export] private float StuckCheckCooldown = 0.5f;
    [Export] public float StuckMovementThreshold = 0.1f;
    [Export] public float StuckDurationThreshold = 1f;


    private Vector3 _currentTarget;
    private RigidBody3D _targetTrash;
    private RigidBody3D _heldTrash;
    private RigidBody3D _lastDroppedTrash;
    private bool _isFleeing = false;
    private bool _isStunned = false;
    private float _stunTimer = 0f;
    private Random _rand = new();
    private Vector3 _lastPositionSnapshot;
    private float _totalStuckTime = 0f;
    private float _stuckCheckTimer;
    private bool _isLured = false;
    private Vector3 _baitTarget;
    [Export] private Area3D _sabatageArea;
    [Export] private CollisionShape3D _areaShape;

    private Dictionary<int, List<Node3D>> _binsByType = [];
    public override void _Ready()
    {
        foreach (Node node in GetTree().GetNodesInGroup("trash_bins"))
        {
            if (node is Node3D bin)
            {
                int binType = (int)bin.Call("GetBinType");

                if (!_binsByType.ContainsKey(binType))
                {
                    _binsByType[binType] = [];
                }
                _binsByType[binType].Add(bin);
            }
        }
        NavAgent.PathDesiredDistance = 1f;
        NavAgent.TargetDesiredDistance = 1f;

        SetRandomWanderTarget();

        WanderCooldownTimer.Timeout += OnWanderTimerTimeout;

        _stuckCheckTimer = StuckCheckCooldown;
        _lastPositionSnapshot = GlobalPosition;

        // Print Bins dictionary for debugging
        GD.Print("Bins by type:");
        foreach (var kvp in _binsByType)
        {
            GD.Print($"Type {kvp.Key}: {kvp.Value.Count} bins");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isLured)
        {
            HandleLuredState((float)delta);
            return;
        }
        if (_isStunned)
        {
            HandleStunnedState((float)delta);
            return;
        }

        // Must run before any movement logic
        CheckStuck((float)delta);

        float distToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

        if (distToPlayer < AvoidPlayerDistance)
        {
            HandleFleeingState(distToPlayer, delta);
        }
        else if (_isFleeing)
        {
            HandleFleeRecoveryState(distToPlayer, delta);
        }
        else if (_heldTrash != null)
        {
            HandleCarryingTrashState(delta);
        }
        else if (!IsTrashValid(_targetTrash))
        {
            _targetTrash = FindNearbyTrash();
            HandleWanderState(delta);
        }
        else if (_targetTrash != null)
        {
            HandleTargetingTrashState(delta);
        }
        else
        {
            HandleWanderState(delta);
        }
    }

    public void LureBait(Vector3 baitPosition)
    {
        if (_isStunned) return;

        _isLured = true;
        _baitTarget = baitPosition;

        _isFleeing = false;
        if (_heldTrash != null) DropTrash();
        _targetTrash = null;
        Speed = 5f;

        NavAgent.TargetPosition = NavigationServer3D.MapGetClosestPoint(
            NavAgent.GetNavigationMap(), _baitTarget
        );
    }

    private void HandleLuredState(float delta)
    {
        if (NavAgent.TargetPosition != _baitTarget)
        {
            NavAgent.TargetPosition = _baitTarget;
        }

        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());
        _animations.Play("run");

        if (GlobalPosition.DistanceTo(_baitTarget) < 1.5f)
        {
            _isLured = false;
            _isStunned = true;
            _stunTimer = StunDuration;
            Velocity = Vector3.Zero;
            MoveAndSlide();
            _animations.Play("idle");
        }
    }

    private void HandleStunnedState(float delta)
    {
        _stunTimer -= delta;
        if (_stunTimer <= 0f)
        {
            _isStunned = false;
            SetRandomWanderTarget();
        }
        Velocity = Vector3.Zero;
        MoveAndSlide();
        _animations.Play("idle");
    }

    private void HandleFleeingState(float distToPlayer, double delta)
    {
        if (_heldTrash != null && (!IsInstanceValid(_heldTrash) || !_heldTrash.IsInsideTree())) DropTrash();

        _targetTrash = null;
        FleeFromPlayer();
        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());
        _animations.Play(_heldTrash != null ? "trash_run" : "run");
        CheckCrashIntoBins();
    }

    private void HandleFleeRecoveryState(float distToPlayer, double delta)
    {
        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());
        CheckCrashIntoBins();

        if (distToPlayer > AvoidPlayerDistance * StopFleeingDistanceMultiplier)
        {
            _isFleeing = false;
            _targetTrash = null;
            SetRandomWanderTarget();
            Speed = 5f;
        }
        _animations.Play(_heldTrash != null ? "trash_run" : "run");
    }

    private void HandleCarryingTrashState(double delta)
    {
        NavAgent.TargetPosition = _currentTarget;
        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());

        _animations.Play("trash_run");

        if (GlobalPosition.DistanceTo(_currentTarget) < 1f)
        {
            DropTrash();
            SetRandomWanderTarget();
        }
    }

    private void HandleTargetingTrashState(double delta)
    {
        NavAgent.TargetPosition = _targetTrash.GlobalPosition;
        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());

        _animations.Play("run");

        if (_targetTrash != null && _targetTrash.IsInsideTree() && GlobalPosition.DistanceTo(_targetTrash.GlobalPosition) < 2f)
        {
            PickupTrash(_targetTrash);
        }
    }

    private void HandleWanderState(double delta)
    {
        if (NavAgent.IsNavigationFinished())
            SetRandomWanderTarget();

        MoveTowards(NavAgent.GetNextPathPosition(), delta);
        HandleRunAnimationDirection(NavAgent.GetNextPathPosition());

        _animations.Play("run");
    }

    private void OnWanderTimerTimeout()
    {
        SetRandomWanderTarget();
    }

    private void SetRandomWanderTarget()
    {
        Vector3 randomPoint = GlobalPosition + new Vector3(
            (float)(_rand.NextDouble() * 20 - 10), 0, (float)(_rand.NextDouble() * 20 - 10));
        NavAgent.TargetPosition = NavigationServer3D.MapGetClosestPoint(
             NavAgent.GetNavigationMap(), randomPoint
         );
    }

    private void MoveTowards(Vector3 targetPosition, double delta)
    {
        if (NavAgent.IsNavigationFinished())
        {
            Velocity = Vector3.Zero;
            MoveAndSlide();
            return;
        }

        Vector3 nextPathPosition = NavAgent.GetNextPathPosition();
        Vector3 direction = (nextPathPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }


    private void FleeFromPlayer()
    {
        if (!_isFleeing)
        {
            Speed *= FleeSpeedMultiplier;
        }
        _isFleeing = true;

        Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();
        Node3D bestBin = null;
        float bestScore = -1f;

        // Find the most suitable bin to run towards
        foreach (var binList in _binsByType.Values)
        {
            foreach (var bin in binList)
            {
                if (!bin.IsInsideTree()) continue;

                Vector3 dirToBin = (bin.GlobalPosition - GlobalPosition).Normalized();
                float distToBin = GlobalPosition.DistanceTo(bin.GlobalPosition);

                float dot = fleeDir.Dot(dirToBin);

                if (dot > 0.3f)
                {
                    float score = dot / distToBin;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestBin = bin;
                    }
                }
            }
        }

        GD.Print("Best bin found: ", bestBin?.Name ?? "None", " with score: ", bestScore);

        Vector3 target;
        if (bestBin != null)
        {
            target = bestBin.GlobalPosition;
        }
        else
        {
            target = GlobalPosition + fleeDir * 15f;
        }

        NavAgent.TargetPosition = NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), target);
    }
    bool IsTrashValid(Node trash) => trash != null && IsInstanceValid(trash) && trash.IsInsideTree();
    private RigidBody3D FindNearbyTrash()
    {
        RigidBody3D closest = null;
        float bestScore = float.MinValue;

        Vector3 toPlayerDir = (Player.GlobalPosition - GlobalPosition).Normalized();

        foreach (var node in GetTree().GetNodesInGroup("cleanable_vacuum"))
        {
            if (node is RigidBody3D trash && trash.IsInsideTree() && trash != _lastDroppedTrash)
            {
                float dist = GlobalPosition.DistanceTo(trash.GlobalPosition);
                float distToPlayer = Player.GlobalPosition.DistanceTo(trash.GlobalPosition);

                if (dist < DetectionRadius && distToPlayer > 2.5f)
                {
                    Vector3 toTrash = (trash.GlobalPosition - GlobalPosition).Normalized();
                    float angleToPlayer = Mathf.RadToDeg(Mathf.Acos(toTrash.Dot(toPlayerDir)));

                    if (angleToPlayer < 60f) continue;

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

                    float concentrationBonus = nearbyCount * 2f;
                    float distancePenalty = dist * 1.5f;
                    float score = concentrationBonus - distancePenalty;

                    if (score > bestScore && IsReachable(trash.GlobalPosition))
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
        if (_heldTrash != null && IsInstanceValid(_heldTrash) && _heldTrash.IsInsideTree())
        {
            _heldTrash.GlobalPosition = _currentTarget;
            _heldTrash.Freeze = false;
            _heldTrash.Visible = true;
            _heldTrash.SetProcess(true);

            _heldTrash.ApplyImpulse(Vector3.Up, new Vector3(
                (float)(_rand.NextDouble() * 4f - 2f),
                1,
                (float)(_rand.NextDouble() * 4f - 2f)
            ));

            _lastDroppedTrash = _heldTrash;
            _heldTrash = null;
        }

        _animations.Play("idle");
        SetRandomWanderTarget();
    }

    private void HandleRunAnimationDirection(Vector3 targetLocation)
    {
        if (_camera == null)
            return;

        Vector3 localVelocity = ToLocal(targetLocation).Normalized();
        _animations.FlipH = localVelocity.X < 0;
    }
    private Vector3 GetTrashDropLocation()
    {
        if (_heldTrash == null || !IsInstanceValid(_heldTrash) || !_heldTrash.IsInsideTree()) return GlobalPosition;

        int trashType = (int)_heldTrash.Call("_GetTrashID");

        if (!_binsByType.TryGetValue(trashType, out var binList) || binList.Count == 0)
        {
            return DropRandomAwayFromPlayer();
        }

        Node3D closeBinToAvoid = null;
        float closestBinDistance = float.MaxValue;

        foreach (var bin in binList)
        {
            float distToBin = GlobalPosition.DistanceTo(bin.GlobalPosition);
            if (distToBin < AvoidBinDistance)
            {
                if (distToBin < closestBinDistance)
                {
                    closestBinDistance = distToBin;
                    closeBinToAvoid = bin;
                }
            }
        }

        if (closeBinToAvoid != null)
        {
            Vector3 directionAwayFromBin = (GlobalPosition - closeBinToAvoid.GlobalPosition).Normalized();
            Vector3 desired = GlobalPosition + directionAwayFromBin * (DropDistance + 2f);

            return NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), desired);
        }
        else
        {
            return DropRandomAwayFromPlayer();
        }
    }
    private Vector3 DropRandomAwayFromPlayer()
    {
        Vector3 awayFromPlayer = (GlobalPosition - Player.GlobalPosition).Normalized();
        Vector3 randomOffset = new Vector3(
            (float)(_rand.NextDouble() * 2f - 1f),
            0,
            (float)(_rand.NextDouble() * 2f - 1f)
        ).Normalized();

        Vector3 blended = (awayFromPlayer * 0.7f + randomOffset * 0.3f).Normalized();
        Vector3 desired = GlobalPosition + blended * DropDistance;

        return NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), desired);
    }

    private void CheckStuck(float delta)
    {
        _stuckCheckTimer -= delta;
        if (_stuckCheckTimer > 0) return;
        _stuckCheckTimer = StuckCheckCooldown;

        if (NavAgent.IsNavigationFinished())
        {
            _totalStuckTime = 0f;
            return;
        }

        if (GlobalPosition.DistanceTo(_lastPositionSnapshot) < StuckMovementThreshold)
        {
            _totalStuckTime += StuckCheckCooldown;
        }
        else
        {
            _totalStuckTime = 0f;
        }

        _lastPositionSnapshot = GlobalPosition;

        if (_totalStuckTime > StuckDurationThreshold)
        {
            _totalStuckTime = 0f;

            if (_targetTrash != null)
            {
                _targetTrash = null;
            }

            if (_heldTrash != null)
            {
                _currentTarget = GetTrashDropLocation();
                if (!IsReachable(_currentTarget))
                {
                    DropTrash();
                }
                else
                {
                    NavAgent.TargetPosition = _currentTarget;
                }
            }
            else
            {
                SetRandomWanderTarget();
            }
        }
    }

    private bool IsReachable(Vector3 target)
    {
        var path = NavigationServer3D.MapGetPath(
            NavAgent.GetNavigationMap(),
            GlobalPosition,
            NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), target),
            false
        );
        return path != null && path.Length > 1;
    }

    private void CheckCrashIntoBins()
    {
        if (_sabatageArea == null || _areaShape == null) return;

        _areaShape.Disabled = false;

        // Check for collisions with bins
        foreach (Node3D body in _sabatageArea.GetOverlappingBodies())
        {
            OnSabotageAreaBodyEntered(body);
        }
    }
    private void OnSabotageAreaBodyEntered(Node3D body)
    {
        GD.Print("Checking body in sabotage area: ", body.Name);
        if (body is Node3D bin && GlobalPosition.DistanceTo(bin.GlobalPosition) <
            SabotageCrashRange && bin.IsInGroup("trash_bins"))
        {
            GD.Print("Raccoon crashed into bin: ", bin.Name);
            if (bin.HasMethod("_RaccoonSabotage"))
            {
                bin.Call("_RaccoonSabotage");
            }

            // Stun the raccoon briefly from the impact
            _isStunned = true;
            _stunTimer = 2f;

            _isFleeing = false;
            Speed = 5f;
            SetRandomWanderTarget();
        }
    }
}
