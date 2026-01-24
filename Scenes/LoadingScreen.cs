using Godot;

public partial class LoadingScreen : Control {

    [Export] private ProgressBar loadingBar;
    [Export] private Label continueLabel;
    [Export] private RichTextLabel gameHintLabel;

    private string sceneToLoadPath;
    private bool isLoading = false;
    private bool canProceed = false;

    private static LoadingScreen _instance;
    public static LoadingScreen Instance => _instance;


    public override void _EnterTree() {
        if (_instance != null) Instance.QueueFree();
        _instance = this;
    }


    public override void _Ready() {

        continueLabel.Visible = false;
        Visible = false;
    }


    public void LoadScene(string path) {
        Node currentScene = GetTree().CurrentScene;
        if (ResourceLoader.HasCached(path)) ResourceLoader.LoadThreadedGet(path);
        else ResourceLoader.LoadThreadedRequest(path);
        currentScene.QueueFree();
        sceneToLoadPath = path;
        isLoading = true;
        Visible = true;
        gameHintLabel.Text = GameConstants.loadingHints.PickRandom();
    }


    public override void _Process(double delta) {
        if (Input.IsAnythingPressed() && canProceed && !isLoading) {
            GetTree().Paused = false;
            Signals.Instance.EnteringNewScene?.Invoke();
            Visible = false;
        }
        else {
            Godot.Collections.Array progress = [];
            ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(sceneToLoadPath, progress);
            if (status == ResourceLoader.ThreadLoadStatus.InProgress) loadingBar.Value = (float)progress[0] * 100;

            else if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                loadingBar.Value = 100;
                canProceed = true;
                continueLabel.Visible = true;
                GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(sceneToLoadPath));
                isLoading = false;
                GetTree().Paused = true;
            }
        }
    }
}
