using Godot;
using System;

public partial class GameHub : PanelContainer {
    [Export] private Button embark;
    [Export] private Button shop;
    [Export] private Control shopUIscene;


    public override void _Ready() {
        embark.Pressed += Embark;
        shop.Pressed += ToggleShop;
    }


    public void Embark() {
        LoadingScreen.Instance.LoadScene("res://Scenes/game_level.tscn");
    }


    public void ToggleShop() {
        shopUIscene.Visible = !shopUIscene.Visible;
    }
}
