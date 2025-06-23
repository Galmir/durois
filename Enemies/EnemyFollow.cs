using Godot;

public partial class EnemyFollow : StateAI {
    public Area3D detectionArea;
    public Area3D attackRangeArea;

    public override void _Ready() {
        base._Ready();
        detectionArea = GetNode<Area3D>("%DetectionArea");
        attackRangeArea = GetNode<Area3D>("%AttackRangeArea");
        detectionArea.BodyExited += StopFollowing;
        attackRangeArea.BodyEntered += AttackTarget;
    }

    public override void Enter() {
        base.Enter();
        animPlayer.Play("walking");
        parentEntity.speed = parentEntity.walkingSpeed;
    }


    public override void Exit() {
        base.Exit();
    }


    public override void _PhysicsProcess(double delta) {

        parentEntity.LookAt(parentEntity.targetEntity.Position, Vector3.Up);
        Vector3 direction = (parentEntity.targetEntity.Position - parentEntity.Position).Normalized();

        parentEntity._targetVelocity.X = direction.X * parentEntity.speed;
        parentEntity._targetVelocity.Z = direction.Z * parentEntity.speed;
    }


    public void StopFollowing(Node3D target) {
        if (parentEntity.ChaseTimer.TimeLeft == 0) parentEntity.ChaseTimer.Start();
    }

    public void AttackTarget(Node3D target) {
        if (target == parentEntity.targetEntity && target is CharacterBody3D) parentFSM.ChangeState("Attack");
    }
}
