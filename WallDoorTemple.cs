using Godot;


public partial class WallDoorTemple : Node3D {
    [Export] private MeshInstance3D door;
    [Export] private CollisionShape3D doorCollision;
    [Export] private float animationDuration = 1.0f;

    public void OpenDoors() {
        Tween tweenDoor = GetTree().CreateTween();
        Tween tweenCollision = GetTree().CreateTween();

        tweenDoor.TweenProperty(door, "position:y", 2.3, animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);

        tweenCollision.TweenProperty(doorCollision, "disabled", true, animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);

    }
}
