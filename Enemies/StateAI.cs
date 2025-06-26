using Godot;

public abstract partial class StateAI : Node {
    protected AnimationPlayer animPlayer;
    protected FSMAI parentFSM;
    protected EnemyController parentEntity;

    public override void _Ready() {
        SetPhysicsProcess(false);
        parentFSM = GetParent<FSMAI>();
        parentEntity = GetParent().GetParent<EnemyController>();
        animPlayer = parentEntity.animator;
    }


    public virtual void Enter() {
        SetPhysicsProcess(true);
    }


    public virtual void Exit() {
        SetPhysicsProcess(false);
    }


    public override void _PhysicsProcess(double delta) {
    }
}
