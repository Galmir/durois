using Godot;

public partial class EnemyController : EntityController {
    [Export] public EnemyData EnemyData;
    [Export] public Timer ChaseTimer;
    [Export] public FSMAI StateMachine;
    [Export] private Label3D healthLabel;


    public override void _Ready() {
        base._Ready();
        ChaseTimer.Timeout += ResetTarget;
        CombatHandler.HealthChanged += UpdateHealthLabel;
    }


    public void ResetTarget() {
        targetEntity = null;
        StateMachine.ChangeState("Idle");
        ChaseTimer.Stop();
    }


    private void UpdateHealthLabel(float current, float max) {
        healthLabel.Text = Mathf.RoundToInt(current).ToString() + "/" + Mathf.RoundToInt(max).ToString();
    }

    public override void OnDied() {
        QuestManager.Instance.OnEnemyDied?.Invoke(EnemyData);
        QueueFree();
    }
}
