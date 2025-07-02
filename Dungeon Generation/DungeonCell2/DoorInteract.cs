using Godot;

public partial class DoorInteract : Interactable {
    public override void _Ready() {
        Interacted += _OnInteract;
    }

    public override void _OnInteract(Node3D interacter) {
        GetParent().GetParent<DoorWall>().OpenDoors();
    }
}
