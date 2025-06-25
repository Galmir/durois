using Godot;
using System;

public partial class InteractRay : RayCast3D {
    [Export] private Label prompt;


    public override void _PhysicsProcess(double delta) {
        prompt.Text = "";
        if (IsColliding()) {
            var collider = GetCollider();
            if (collider is Interactable) {
                Interactable iCollider = (Interactable)collider;
                prompt.Text = "" + iCollider.promptMessage;
            }
        }
    }
}
