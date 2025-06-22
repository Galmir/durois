using Godot;

public partial class Minimap : SubViewportContainer {
    [Export] private Camera3D camera;
    private Node3D targetToFollow;

    public override void _Process(double delta) {
        if (targetToFollow is not null) camera.Position = new Vector3(targetToFollow.Position.X, camera.Position.Y, targetToFollow.Position.Z);
    }

    public void AssignCameraTarget(Node3D target) {
        targetToFollow = target;
    }
}
