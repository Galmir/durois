using Godot;
using System;

public partial class MiniMapCamera : Camera3D {
    private Node3D playerScene;


    public override void _Ready() {
        playerScene = GetNode<Node3D>("%ProtoController");
    }

    public override void _Process(double delta) {
        Position = new Vector3(playerScene.Position.X, Position.Y, playerScene.Position.Z);
    }
}
