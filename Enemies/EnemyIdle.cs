using Godot;

public partial class EnemyIdle : State {
    private Area3D detectionArea;
    private bool _enemyEntered = false;


    private bool enemyEntered {
        get {
            return _enemyEntered;
        }
        set {
            _enemyEntered = value;
        }
    }

    public override void _Ready() {
        enemyEntered = false;
        base._Ready();
        detectionArea = GetNode<Area3D>("%DetectionArea");
        detectionArea.BodyEntered += SetTarget;
    }


    public override void Enter() {
        base.Enter();
        animPlayer.Play("idle");
    }


    public override void Exit() {
        base.Exit();
    }


    public override void HandleInput(InputEvent @event) {
    }

    public void SetTarget(Node3D target) {
        if (target is CharacterBody3D) {
            parentEntity.targetEntity = (CharacterBody3D)target;
            parentFSM.ChangeState("Follow");
        }
    }
}
