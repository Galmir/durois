using Godot;
using System;

public partial class QuestManager : Node {
    private static QuestManager _instance;
    public static QuestManager Instance => _instance;

    // signals that advance quests
    public Action<EnemyData> OnEnemyDied;
    public Action<InventoryItem, int> OnItemObtained;
    public Action<Quest> OnQuestStepProgress;

    public enum QuestStates {
        Undiscovered,
        Discovered,
        Started,
        Succeeded,
        Failed
    }


    public override void _EnterTree() {

        if (_instance != null) {
            this.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }

    public override void _Ready() {
    }

}

