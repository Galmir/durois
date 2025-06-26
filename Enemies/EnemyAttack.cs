using Godot;

public partial class EnemyAttack : StateAI {

    public override void Enter() {
        base.Enter();
        animPlayer.Play("kick");
        parentEntity._targetVelocity = Vector3.Zero;
        parentEntity.speed = 0;
    }


    public override void Exit() {
        base.Exit();
    }


    public override void _PhysicsProcess(double delta) {
        if (!animPlayer.IsPlaying()) parentFSM.ChangeState("Follow");
    }
}
