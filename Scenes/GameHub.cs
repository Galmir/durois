using Godot;
using System;

public partial class GameHub : PanelContainer {
    [ExportCategory("Buttons")]
    [Export] private Button embark;
    [Export] private Button shop;
    [Export] private Button saveAndExit;

    [ExportCategory("Dependencies")]
    [Export] private Control shopUIscene;
    [Export] AudioStream BGM;


    public override void _Ready() {
        embark.Pressed += Embark;
        shop.Pressed += ToggleShop;
        saveAndExit.Pressed += SaveandExit;

        GetTree().CreateTimer(1).Timeout += () => AudioManager.Instance.Play(AudioType.Music, BGM);
    }


    public void Embark() {
        AudioManager.Instance.Stop();
        LoadingScreen.Instance.LoadScene("res://Scenes/game_level.tscn");
    }


    public void ToggleShop() {
        shopUIscene.Visible = !shopUIscene.Visible;
    }

    public void SaveandExit() {
        SaveLoad.Instance.SaveGame();
        LoadingScreen.Instance.LoadScene("res://Scenes/main_menu.tscn");
    }
}
