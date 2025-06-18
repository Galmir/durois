using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CamRoot : Node3D {


    [Export] private float LookAroundSpeed = 0.01f;



    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }


    private float _rotationX = 0f;
    private float _rotationY = 0f;

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMotion) {
            // modify accumulated mouse rotation
            _rotationX += -mouseMotion.Relative.X * LookAroundSpeed;
            _rotationY += -mouseMotion.Relative.Y * LookAroundSpeed;

            // reset rotation
            Transform3D transform = Transform;
            transform.Basis = Basis.Identity;
            Transform = transform;

            RotateObjectLocal(Vector3.Up, _rotationX); // first rotate about Y
            RotateObjectLocal(Vector3.Right, _rotationY); // then rotate about X
        }
    }
}
