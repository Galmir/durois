using Godot;

public partial class MainMenu : PanelContainer {
    [Export] private Button newGame;
    [Export] private Button loadGame;
    [Export] private Button Settings;
    [Export] private Button Quit;


    public override void _Ready() {
        newGame.Pressed += StartNewGame;
    }


    public void StartNewGame() {
        LoadingScreen.Instance.LoadScene("res://Scenes/game_hub.tscn");
    }
}
