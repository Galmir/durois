using System;
using Godot;
using Godot.Collections;

public partial class Quest : Node {
    [Export] public string QuestName { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; private set; }
    [Export] public QuestManager.QuestStates questState = QuestManager.QuestStates.Undiscovered;
    [Export] public Inventory Rewards { get; private set; }
    [Export] public bool IsSequential { get; private set; }
    [Export] public bool isMainQuest;
    [Export] public string questGiver; // the source of the quest. NPC name or something
    public Action<string> QuestCompleted;

    private int questSteps;
    private int completedSteps = 0;


    public override void _Ready() {
        questSteps = GetChildCount();
        StartQuest();
    }

    public void StartQuest() {
        GD.Print("quest started: " + QuestName + "\n Description: " + Description);
        questState = QuestManager.QuestStates.Started;
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
            GD.Print("quest completed: " + QuestName);
            PlayerData.Instance.PlayerInventory.Loot(Rewards);
            foreach (BaseQuestStep step in GetChildren()) {
                step.QueueFree();
            }
            QuestCompleted?.Invoke(Name);
            return;
        }
        if (IsSequential) GetChild<BaseQuestStep>(completedSteps).ActivateQuestStep(); // activate the next step
    }

    public Array<string> GetProgress() {
        Array<string> stepsProgress = [];
        Array<Node> children = GetChildren();
        foreach (Node child in children) {
            if (HasMethod("GetProgress")) stepsProgress.Add((string)child.Call("GetProgress"));

        }
        return stepsProgress;
    }
}
