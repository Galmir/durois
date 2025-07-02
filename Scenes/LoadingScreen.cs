using Godot;


public partial class LoadingScreen : Control {
    [Export] private Label continueLabel;

    private bool canProceed = false;


    public override void _Ready() {
        Signals.Instance.FinishedLoadingScene += ContinueToScene;
        continueLabel.Visible = false;
    }


    public override void _Process(double delta) {
        if (Input.IsAnythingPressed() && canProceed) {
            GetTree().Paused = false;
            Signals.Instance.EnteringNewScene?.Invoke();
            QueueFree();
        }
    }

    public void ContinueToScene() {
        GetTree().Paused = true;
        canProceed = true;
        continueLabel.Visible = true;
    }
}
