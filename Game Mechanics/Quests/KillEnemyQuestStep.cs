using Godot;

public partial class KillEnemyQuestStep : BaseQuestStep {
    [Export] public CombatData targetEnemy;

    private int counter = 0;

    public override void ActivateQuestStep() {
        base.ActivateQuestStep();
        QuestManager.Instance.OnEnemyKilled += CheckProgress;
    }

    public void CheckProgress(CombatData enemy) {
        if (enemy == targetEnemy) ProgressQuest();
    }

}
