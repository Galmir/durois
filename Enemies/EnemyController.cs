using Godot;

public partial class EnemyController : EntityController {
    [Export] public Timer ChaseTimer;
    [Export] public FSMAI StateMachine;

    public override void _Ready() {
        base._Ready();
        ChaseTimer.Timeout += ResetTarget;
    }

    public void ResetTarget() {
        targetEntity = null;
        StateMachine.ChangeState("Idle");
        ChaseTimer.Stop();
    }
}
