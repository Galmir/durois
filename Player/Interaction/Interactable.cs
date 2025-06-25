using Godot;

public partial class Interactable : StaticBody3D {
    [Export]
    public string promptMessage { get; private set; }
}
