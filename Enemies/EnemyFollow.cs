using Godot;
using System;

public partial class EnemyFollow : State {
    public Area3D detectionArea;
    public Area3D attackRangeArea;

    public override void Enter() {
        base.Enter();
        animPlayer.Play("walking");
        parentEntity.speed = parentEntity.walkingSpeed;
        detectionArea = GetNode<Area3D>("%DetectionArea");
        detectionArea.BodyExited += StopFollowing;
        attackRangeArea = GetNode<Area3D>("%AttackRangeArea");
        attackRangeArea.BodyEntered += AttackTarget;
    }


    public override void Exit() {
    }


    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        parentEntity.LookAt(parentEntity.targetEntity.Position, Vector3.Up);
        Vector3 direction = (parentEntity.targetEntity.Position - parentEntity.Position).Normalized();

        parentEntity._targetVelocity.X = direction.X * parentEntity.speed;
        parentEntity._targetVelocity.Z = direction.Z * parentEntity.speed;

        parentEntity.Velocity = parentEntity._targetVelocity;
        parentEntity.MoveAndSlide();
    }


    public override void HandleInput(InputEvent @event) {
    }


    public void StopFollowing(Node3D target) {
        if (target == parentEntity.targetEntity) parentFSM.ChangeState("Idle");
    }

    public void AttackTarget(Node3D target) {
        if (target == parentEntity.targetEntity && target is CharacterBody3D) parentFSM.ChangeState("Attack");
    }
}
