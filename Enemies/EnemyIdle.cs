using Godot;

public partial class EnemyIdle : StateAI {
    private Area3D detectionArea;
    private bool _enemyEntered = false;


    public override void _Ready() {
        base._Ready();
        detectionArea = GetNode<Area3D>("%DetectionArea");
        detectionArea.BodyEntered += SetTarget;
    }


    public override void Enter() {
        base.Enter();
        animPlayer.Play("idle");
        parentEntity._targetVelocity = Vector3.Zero;
        parentEntity.speed = 0;
        foreach (Node3D body in detectionArea.GetOverlappingBodies()) SetTarget(body);
    }


    public override void Exit() {
        base.Exit();
    }


    public void SetTarget(Node3D target) {
        if (target.IsInGroup("Player")) {
            parentEntity.targetEntity = (CharacterBody3D)target;
            parentFSM.ChangeState("Follow");
        }
    }
}
