using Godot;
using System;

public partial class QuestDetailsUi : VBoxContainer {
    [Export] private Label questTitleLabel;
    [Export] private VBoxContainer questStepsContainer;
    [Export] private PackedScene QuestStepUiScene;


    public void UpdateQuestDetails() {
        foreach (Node i in questStepsContainer.GetChildren()) i.QueueFree();

    }
}
