using Godot;
using System;

public abstract partial class BaseQuestStep : Node {
    [Export] public string description { get; private set; }
    [Export] public int TargetAmount { get; private set; }

    public bool IsCompleted { get; set; } = false;
    public bool IsActive { get; private set; } = false;
    public int Counter { get; private set; } = 0;

    public Action StepCompleted;

    public virtual void ActivateQuestStep() {
        IsActive = true;
    }
    public void ProgressQuest() {
        if (!IsActive) return;
        Counter += 1;
        GD.Print(GetProgress());
        if (Counter >= TargetAmount) {
            IsActive = false;
            IsCompleted = true;
            StepCompleted.Invoke();
            GD.Print("killed all required enemies!");
        }
    }

    public string GetProgress() {
        return "enemies killed: " + Counter + "/" + TargetAmount;
    }
}
