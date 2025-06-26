using System;
using Godot;

public abstract partial class Interactable : StaticBody3D {
    [Export]
    public string promptMessage { get; private set; }

    public Action<Node3D> Interacted;

    public void Interact(Node3D interacter) {
        Interacted?.Invoke(interacter);
    }

    public abstract void _OnInteract(Node3D interacter);
}
