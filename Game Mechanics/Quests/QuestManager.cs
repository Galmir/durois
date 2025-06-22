using Godot;
using System;

public partial class QuestManager : Node {
    private static QuestManager _instance;
    public static QuestManager Instance => _instance;

    private Node UndiscoveredQuests;
    private Node DiscoveredQuests;
    private Node CompletedQuests;

    // signals that advance quests
    public Action<CombatData> OnEnemyKilled;
    public Action<InventoryItem, int> OnItemObtained;


    public override void _EnterTree() {

        if (_instance != null) {
            this.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }

    public override void _Ready() {
        UndiscoveredQuests = GetNode<Node>("%UndiscoveredQuests");
        DiscoveredQuests = GetNode<Node>("%DiscoveredQuests");
        CompletedQuests = GetNode<Node>("%CompletedQuests");
    }

    public void DiscoverQuest(string questName) {
        foreach (Quest quest in UndiscoveredQuests.GetChildren()) {
            if (quest.Name == questName) {
                quest.Reparent(DiscoveredQuests);
                quest.StartQuest();
                quest.QuestCompleted += CompleteQuest;
                break;
            }
        }
    }

    public void CompleteQuest(string questName) {
        foreach (Quest quest in DiscoveredQuests.GetChildren()) {
            if (quest.Name == questName) {
                quest.Reparent(CompletedQuests);
                quest.QuestCompleted += CompleteQuest;
                break;
            }
        }
    }

}

