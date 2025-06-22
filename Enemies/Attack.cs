using Godot;

public partial class Attack : State {

    public override void Enter() {
        base.Enter();
        animPlayer.Play("kick");
        parentEntity.speed = 0;
    }


    public override void Exit() {
    }


    public override void _PhysicsProcess(double delta) {
        if (!animPlayer.IsPlaying()) parentFSM.ChangeState("Idle");
    }


    public override void HandleInput(InputEvent @event) {
    }

}
