using Godot;


public partial class WallDoorTemple : Node3D {
    [Export] private DoorInteract door;
    [Export] private CollisionShape3D doorCollision;
    [Export] private float animationDuration = 1.0f;


    public override void _Ready() {
        door.Interacted += OpenDoors;
    }
    public void OpenDoors(Node3D Interacter) {
        Tween tweenDoor = GetTree().CreateTween();
        Tween tweenCollision = GetTree().CreateTween();

        tweenDoor.TweenProperty(door, "position:y", 4.8, animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);
        tweenCollision.TweenProperty(doorCollision, "disabled", true, animationDuration).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);

    }
}
