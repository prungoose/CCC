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
        GlobalPosition = NavigationServer3D.MapGetClosestPoint(NavAgent.GetNavigationMap(), GlobalPosition);
        GD.Print("Nav Finished: ", NavAgent.IsNavigationFinished(), " | Next: ", NavAgent.GetNextPathPosition());



        NavAgent.PathDesiredDistance = 0.5f;
        NavAgent.TargetDesiredDistance = 0.5f;

        // Just walk 10 units forward from wherever the raccoon starts
        NavAgent.TargetPosition = GlobalPosition + new Vector3(10, 0, 0);
    }


    // public override void _PhysicsProcess(double delta)
    // {
    //     if (_isStunned)
    //     {
    //         _stunTimer -= (float)delta;
    //         if (_stunTimer <= 0f) _isStunned = false;
    //         return;
    //     }

    //     float distToPlayer = GlobalPosition.DistanceTo(Player.GlobalPosition);

    //     if (distToPlayer < AvoidPlayerDistance)
    //     {
    //         if (_heldTrash != null)
    //             DropTrash();

    //         FleeFromPlayer();
    //         return;
    //     }

    //     if (_isFleeing)
    //     {
    //         MoveTowards(NavAgent.GetNextPathPosition(), delta);
    //         CheckCrashIntoBins();

    //         // Stop fleeing when far enough from player
    //         if (distToPlayer > AvoidPlayerDistance * 2f)
    //         {
    //             _isFleeing = false;
    //             SetRandomWanderTarget();
    //         }
    //         return;
    //     }

    //     // Trash interaction logic
    //     if (_heldTrash != null)
    //     {
    //         MoveTowards(_currentTarget, delta);

    //         if (GlobalPosition.DistanceTo(_currentTarget) < 1f)
    //             DropTrash();
    //     }
    //     else
    //     {
    //         if (_targetTrash == null)
    //             _targetTrash = FindNearbyTrash();

    //         if (_targetTrash != null)
    //         {
    //             NavAgent.TargetPosition = _targetTrash.GlobalPosition;
    //             MoveTowards(NavAgent.GetNextPathPosition(), delta);

    //             if (GlobalPosition.DistanceTo(_targetTrash.GlobalPosition) < 1f)
    //                 PickupTrash(_targetTrash);
    //         }
    //         else if (!NavAgent.IsNavigationFinished())
    //         {
    //             MoveTowards(NavAgent.GetNextPathPosition(), delta);
    //         }
    //         else
    //         {
    //             SetRandomWanderTarget();
    //         }
    //     }
    // }

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
    public override void _PhysicsProcess(double delta)
    {
        if (NavAgent.IsNavigationFinished())
            return;

        Vector3 nextPosition = NavAgent.GetNextPathPosition();
        Vector3 direction = (nextPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;

        MoveAndSlide();
    }


    //     private void FleeFromPlayer()
    //     {
    //         _isFleeing = true;

    //         // Find nearest trash bin to sabotage while fleeing
    //         Node3D nearestBin = null;
    //         float nearestBinDist = float.MaxValue;

    //         foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
    //         {
    //             if (node is Node3D bin)
    //             {
    //                 float dist = GlobalPosition.DistanceTo(bin.GlobalPosition);
    //                 if (dist < nearestBinDist)
    //                 {
    //                     nearestBin = bin;
    //                     nearestBinDist = dist;
    //                 }
    //             }
    //         }

    //         Vector3 target;
    //         if (nearestBin != null && nearestBinDist < 20f) // If bin is reasonably close
    //         {
    //             // Flee towards the bin for sabotage opportunity
    //             Vector3 dirToBin = (nearestBin.GlobalPosition - GlobalPosition).Normalized();
    //             Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();

    //             // Blend flee direction with bin direction (60% flee, 40% towards bin)
    //             Vector3 blendedDir = (fleeDir * 0.6f + dirToBin * 0.4f).Normalized();
    //             target = GlobalPosition + blendedDir * 15f;
    //         }
    //         else
    //         {
    //             // Standard flee behavior
    //             Vector3 fleeDir = (GlobalPosition - Player.GlobalPosition).Normalized();
    //             target = GlobalPosition + fleeDir * 10f;
    //         }

    //         NavAgent.TargetPosition = target;
    //     }

    //     private void CheckCrashIntoBins()
    //     {
    //         foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
    //         {
    //             if (node is Node3D bin && GlobalPosition.DistanceTo(bin.GlobalPosition) < SabotageCrashRange)
    //             {
    //                 GD.Print("Raccoon crashed into bin!");

    //                 // // Trigger bin sabotage - call method on the bin to spill trash
    //                 // if (bin.HasMethod("_RaccoonSabotage"))
    //                 // {
    //                 //     bin.Call("_RaccoonSabotage");
    //                 // }

    //                 // // Create some visual/audio feedback
    //                 // if (bin.HasMethod("_PlaySabotageEffect"))
    //                 // {
    //                 //     bin.Call("_PlaySabotageEffect");
    //                 // }

    //                 // Reduce game completion progress
    //                 var ui = GetTree().CurrentScene.GetNode<Control>("SubViewportContainer/UI");
    //                 if (ui != null && ui.HasMethod("IncreaseGameCompletion"))
    //                 {
    //                     ui.Call("IncreaseGameCompletion", -10); // Negative progress
    //                 }

    //                 // Stun the raccoon briefly from the impact
    //                 _isStunned = true;
    //                 _stunTimer = 2f; // Brief stun from crash

    //                 _isFleeing = false;
    //                 SetRandomWanderTarget();
    //                 break;
    //             }
    //         }
    //     }

    //     private RigidBody3D FindNearbyTrash()
    //     {
    //         RigidBody3D closest = null;
    //         float bestScore = float.MinValue;

    //         foreach (var node in GetTree().GetNodesInGroup("cleanable_vacuum"))
    //         {
    //             if (node is RigidBody3D trash && trash.IsInsideTree() && trash != _lastDroppedTrash)
    //             {
    //                 float dist = GlobalPosition.DistanceTo(trash.GlobalPosition);
    //                 float distToPlayer = Player.GlobalPosition.DistanceTo(trash.GlobalPosition);

    //                 if (dist < DetectionRadius && distToPlayer > 2.5f)
    //                 {
    //                     // Calculate concentration score - count nearby trash of same type
    //                     int trashType = (int)trash.Call("GetThrownTrashID");
    //                     int nearbyCount = 0;

    //                     foreach (var otherNode in GetTree().GetNodesInGroup("cleanable_vacuum"))
    //                     {
    //                         if (otherNode is RigidBody3D otherTrash && otherTrash != trash && otherTrash.IsInsideTree())
    //                         {
    //                             float otherDist = trash.GlobalPosition.DistanceTo(otherTrash.GlobalPosition);
    //                             int otherType = (int)otherTrash.Call("GetThrownTrashID");

    //                             if (otherDist < 8f && otherType == trashType) // Same type within 8 units
    //                                 nearbyCount++;
    //                         }
    //                     }

    //                     // Score based on concentration (more nearby = better) and distance (closer = better)
    //                     float concentrationBonus = nearbyCount * 2f;
    //                     float distancePenalty = dist * 0.1f;
    //                     float score = concentrationBonus - distancePenalty;

    //                     if (score > bestScore)
    //                     {
    //                         closest = trash;
    //                         bestScore = score;
    //                     }
    //                 }
    //             }
    //         }

    //         return closest;
    //     }

    //     private void PickupTrash(RigidBody3D trash)
    //     {
    //         _heldTrash = trash;
    //         _targetTrash = null;

    //         trash.Freeze = true;
    //         trash.Visible = false;
    //         trash.SetProcess(false);

    //         _currentTarget = GetTrashDropLocation();
    //         NavAgent.TargetPosition = _currentTarget;
    //     }

    //     private void DropTrash()
    //     {
    //         _heldTrash.GlobalPosition = _currentTarget;
    //         _heldTrash.Freeze = false;
    //         _heldTrash.Visible = true;
    //         _heldTrash.SetProcess(true);
    //         _heldTrash.ApplyImpulse(Vector3.Up, new Vector3(
    //             (float)_rand.NextDouble() * 4f - 2f,
    //             1,
    //             (float)_rand.NextDouble() * 4f - 2f
    //         ));

    //         _lastDroppedTrash = _heldTrash;
    //         _heldTrash = null;

    //         SetRandomWanderTarget();
    //     }

    //     private Vector3 GetTrashDropLocation()
    //     {
    //         if (_heldTrash == null)
    //             return GlobalPosition;

    //         // Get trash type to find correct bin
    //         int trashType = (int)_heldTrash.Call("GetThrownTrashID");

    //         // Find the correct bin for this trash type
    //         Node3D correctBin = null;
    //         foreach (var node in GetTree().GetNodesInGroup("trash_bins"))
    //         {
    //             if (node is Node3D bin)
    //             {
    //                 int binType = (int)bin.Call("GetBinType"); // Assuming bins have this method
    //                 if (binType == trashType)
    //                 {
    //                     correctBin = bin;
    //                     break;
    //                 }
    //             }
    //         }

    //         // If we found the correct bin, drop trash away from it
    //         if (correctBin != null)
    //         {
    //             Vector3 dirAwayFromBin = (GlobalPosition - correctBin.GlobalPosition).Normalized();
    //             return correctBin.GlobalPosition + dirAwayFromBin * (DropDistance + 5f); // Extra distance from correct bin
    //         }

    //         // Fallback to random nearby location
    //         return GlobalPosition + new Vector3(
    //             (float)_rand.NextDouble() * 2f - 1f,
    //             0,
    //             (float)_rand.NextDouble() * 2f - 1f
    //         ).Normalized() * DropDistance;
    //     }

    //     public void AttractToBait(Vector3 baitPosition)
    //     {
    //         NavAgent.TargetPosition = baitPosition;
    //         _isFleeing = false;
    //         _targetTrash = null;
    //     }

    //     public void Stun()
    //     {
    //         _isStunned = true;
    //         _stunTimer = StunDuration;
    //         Velocity = Vector3.Zero;
    //     }
    // }
}