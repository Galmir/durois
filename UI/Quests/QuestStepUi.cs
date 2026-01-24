using Godot;

public partial class QuestStepUi : HBoxContainer {
    [Export] private TextureRect questIcon;
    [Export] private Label stepDescriptionLabel;
    [Export] private Label stepProgressLabel;

    public override void _Ready() {
        base._Ready();
    }


    public void UpdateStep(string description, string progress) {
        stepDescriptionLabel.Text = description;
        stepProgressLabel.Text = progress;
    }
}
