using Godot;
using System;

public partial class Torch : Node3D {
    [Export] AudioStreamPlayer3D positionalAudio;

    public override void _Ready() {
        base._Ready();
        GetTree().CreateTimer(2).Timeout += () => positionalAudio.Play();
    }

}
