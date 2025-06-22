using System;
using Godot;

public partial class Quest : Node {
    [Export] public string QuestName { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public bool IsStarted { get; set; }
    [Export] public Inventory Rewards { get; private set; }
    [Export] public bool IsSequential { get; private set; }
    [Export] public bool isMainQuest;
    [Export] public bool isActiveQuest;

    public string questGiver; // the source of the quest. NPC name or something
    public Action<string> QuestCompleted;

    private int questSteps;
    private int completedSteps = 0;


    public override void _Ready() {
        questSteps = GetChildCount();
    }

    public void StartQuest() {
        if (IsSequential) {
            GetChild<BaseQuestStep>(0).ActivateQuestStep();
        }
        else {
            foreach (BaseQuestStep step in GetChildren()) {
                if (step is not BaseQuestStep) return;
                step.StepCompleted += ProgressQuest;
                step.ActivateQuestStep();
            }
        }

    }


    public void ProgressQuest() {
        completedSteps += 1;

        if (completedSteps == questSteps) {
            PlayerData.Instance.PlayerInventory.Loot(Rewards);
            foreach (BaseQuestStep step in GetChildren()) {
                step.QueueFree();
            }
            QuestCompleted.Invoke(Name);
            return;
        }
        if (IsSequential) GetChild<BaseQuestStep>(completedSteps).ActivateQuestStep(); // activate the next step
    }
}
