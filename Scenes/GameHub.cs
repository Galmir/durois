using Godot;
using System;

public partial class GameHub : PanelContainer {
    [Export] private Button embark;


    public override void _Ready() {
        embark.Pressed += Embark;
    }


    public void Embark() {
        LoadingScreen.Instance.LoadScene("res://Scenes/game_level.tscn");
    }
}
