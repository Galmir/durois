using Godot;
using System;

public partial class DoorWall : Node3D {
    [Export] private MeshInstance3D doorLeft;
    [Export] private MeshInstance3D doorRight;
    [Export] private CollisionShape3D doorCollision;
    [Export] private float animationDuration = 1.0f;

    public void OpenDoors() {
        Tween tweenL = GetTree().CreateTween();
        Tween tweenR = GetTree().CreateTween();
        Tween tweenC = GetTree().CreateTween();

        tweenR.TweenProperty(doorRight, "rotation:z", Mathf.DegToRad(100), animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);
        tweenL.TweenProperty(doorLeft, "rotation:z", Mathf.DegToRad(-100), animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);
        tweenC.TweenProperty(doorCollision, "disabled", true, animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);

    }
}
