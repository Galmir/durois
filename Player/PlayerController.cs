using System;
using Godot;
using Godot.Collections;

public partial class PlayerController : EntityController {
    [ExportCategory("AssetDependencies")]
    [Export] private Node3D cameraMountY;
    [Export] private Node3D cameraMountX;
    [Export] private StaminaComponent stamina;
    [Export] private int runningCost = 5;
    [Export] private Area3D hitBox;
    [Export] private Array<Effect> effects;


    [ExportCategory("Camera Settings")]
    [Export] private float LookAroundSpeed = 0.01f;
    [Export] private float cameraSensitivity = 1.5f;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        hitBox.BodyEntered += OnBodyEnteredHitBox;
    }

    public override void _PhysicsProcess(double delta) {
        //camera handling
        Vector2 cameraVector = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down");
        if (cameraVector.Length() >= 0.2) {
            cameraMountX.RotateX(Mathf.DegToRad(-cameraVector.Y * cameraSensitivity));
            cameraMountY.RotateY(
                Mathf.Clamp(
                    Mathf.DegToRad(-cameraVector.X * cameraSensitivity),
                    Mathf.DegToRad(-80),
                    Mathf.DegToRad(30)
                )
            );
        }


        if (!animator.IsPlaying()) isLocked = false;

        if (Input.IsActionJustPressed("attack")) {
            if (animator.CurrentAnimation != "kick") animator.Play("kick");
            isLocked = true;
        }

        if (!IsOnFloor() && hasGravity) _targetVelocity.Y -= FallAcceleration * (float)delta;

        if (Input.IsActionJustPressed("toggle_run")) {
            isRunning = !isRunning;
            stamina.isSpendingStamina = !stamina.isSpendingStamina;
        }

        Vector2 inputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (cameraMountY.Transform.Basis * new Vector3(inputVector.X, 0, inputVector.Y)).Normalized();
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
                isRunning = false;
                stamina.isSpendingStamina = false;
            }
            Velocity = _targetVelocity;
            MoveAndSlide();
        }
    }

    private void OnBodyEnteredHitBox(Node3D body) {
        if (body.HasMethod("ReceiveHit")) body.Call("ReceiveHit", effects, (Node3D)Owner);
    }
}
