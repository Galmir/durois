using Godot;

public partial class PlayerController : CharacterBody3D {
    [Export] private Camera3D camera;
    [Export] public int Speed { get; set; } = 14;
    [Export] public int FallAcceleration { get; set; } = 75;
    [Export] private bool hasGravity = false;

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta) {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right")) {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left")) {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back")) {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward")) {
            direction.Z -= 1.0f;
        }

        if (direction != Vector3.Zero) {
            direction = direction.Normalized();
        }

        // Ground velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        Rotation.Lerp(camera.Rotation, 1f);

        // Vertical velocity
        if (!IsOnFloor() && hasGravity) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        // Moving the character
        Velocity = _targetVelocity;
        MoveAndSlide();
    }
}
