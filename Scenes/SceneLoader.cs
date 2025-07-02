using Godot;

public partial class SceneLoader : Control {
    [Export] private ProgressBar loadingBar;
    private string sceneToLoadPath;
    private PackedScene loadingScreenScene = GD.Load<PackedScene>("res://Scenes/loading_screen.tscn");
    private Node loadingScreenInstance;
    private bool isLoading = false;

    private static SceneLoader _instance;
    public static SceneLoader Instance => _instance;

    public override void _EnterTree() {
        if (_instance != null) Instance.QueueFree();
        _instance = this;
    }


    public void LoadScene(string path) {
        Node currentScene = GetTree().CurrentScene;
        loadingScreenInstance = loadingScreenScene.Instantiate<Node>();
        GetTree().Root.CallDeferred("add_child", loadingScreenInstance);

        if (ResourceLoader.HasCached(path)) ResourceLoader.LoadThreadedGet(path);
        else ResourceLoader.LoadThreadedRequest(path);
        currentScene.QueueFree();

        sceneToLoadPath = path;
        loadingBar = loadingScreenInstance.GetNode<ProgressBar>("%ProgressBar");
        isLoading = true;
    }


    public override void _Process(double delta) {
        if (!isLoading || loadingScreenInstance is null) return;
        Godot.Collections.Array progress = [];
        ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(sceneToLoadPath, progress);
        if (status == ResourceLoader.ThreadLoadStatus.InProgress) loadingBar.Value = (float)progress[0] * 100;
        else if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
            loadingBar.Value = 100;
            Signals.Instance.FinishedLoadingScene?.Invoke();
            GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(sceneToLoadPath));
            isLoading = false;
            loadingScreenInstance = null;
            loadingBar = null;
        }
    }
}
