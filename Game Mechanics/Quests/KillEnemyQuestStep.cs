using Godot;

public partial class KillEnemyQuestStep : BaseQuestStep {
    [Export] public EnemyData targetEnemy;


    public override void ActivateQuestStep() {
        base.ActivateQuestStep();
        QuestManager.Instance.OnEnemyDied += CheckProgress;
    }


    public void CheckProgress(EnemyData enemy) {
        if (enemy == targetEnemy) ProgressQuest();
    }

}
