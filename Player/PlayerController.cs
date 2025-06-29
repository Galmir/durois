using Godot;

public partial class PlayerController : EntityController {
    [ExportCategory("AssetDependencies")]
    [Export] private Node3D cameraMount;
    [Export] private StaminaComponent stamina;
    [Export] private int runningCost = 5;


    [ExportCategory("Camera Settings")]
    [Export] private float LookAroundSpeed = 0.01f;
    [Export] private float mouseSensitivityH = 0.1f;
    [Export] private float mouseSensitivityV = 0.1f;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        // look around
        if (@event is InputEventMouseMotion ev) {
            float rotateY = Mathf.DegToRad(-ev.Relative.X * mouseSensitivityH);
            RotateY(rotateY);
            visualAnchor.RotateY(-rotateY);
            cameraMount.RotateX(Mathf.DegToRad(-ev.Relative.Y * mouseSensitivityV));
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (!animator.IsPlaying()) isLocked = false;

        if (Input.IsActionJustPressed("attack")) {
            if (animator.CurrentAnimation != "kick") animator.Play("kick");
            isLocked = true;
        }


        if (!IsOnFloor() && hasGravity) _targetVelocity.Y -= FallAcceleration * (float)delta;

        if (Input.IsActionJustPressed("toggle_run")) {
            isRunning = true;
            stamina.isSpendingStamina = true;
        }
        if (Input.IsActionJustReleased("toggle_run")) {
            isRunning = false;
            stamina.isSpendingStamina = false;
        }


        Vector2 inputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputVector.X, 0, inputVector.Y)).Normalized();
        if (!isLocked) {
            if (direction != Vector3.Zero) {
                if (isRunning && !stamina.isFatigued) {
                    speed = runningSpeed;
                    moveAnimation = "running";
                    stamina.Damage(runningCost * (float)delta);
                }
                else {
                    speed = walkingSpeed;
                    moveAnimation = "walking";
                }

                _targetVelocity.X = direction.X * speed;
                _targetVelocity.Z = direction.Z * speed;
                if (animator.CurrentAnimation != moveAnimation) animator.Play(moveAnimation);

                if (!isLocked) visualAnchor.LookAt(Position + direction);
            }
            else {
                if (animator.CurrentAnimation != "idle") animator.Play("idle");
                _targetVelocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
                _targetVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
            }
            Velocity = _targetVelocity;
            MoveAndSlide();
        }
    }


}
