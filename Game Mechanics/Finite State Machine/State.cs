using Godot;

public abstract partial class State : Node {
    protected AnimationPlayer animPlayer;
    protected FSM parentFSM;
    protected EntityController parentEntity;

    public override void _Ready() {
        SetPhysicsProcess(false);
        parentFSM = GetParent<FSM>();
        parentEntity = GetParent().GetParent<EntityController>();
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


    public abstract void HandleInput(InputEvent @event);
}
