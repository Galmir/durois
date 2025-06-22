using Godot;
using System;

public partial class MiniMapCamera : Camera3D {
    public Node3D targetToFollow;

    public override void _Process(double delta) {
        if (targetToFollow is not null) Position = new Vector3(targetToFollow.Position.X, Position.Y, targetToFollow.Position.Z);
    }
}
