using Godot;
using System;

public partial class MiniMapCamera : Camera3D {
    [Export] private Node3D playerScene;

    public override void _Process(double delta) {
        Position = new Vector3(playerScene.Position.X, Position.Y, playerScene.Position.Z);
    }
}
