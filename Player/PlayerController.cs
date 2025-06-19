using Godot;

public partial class PlayerController : CharacterBody3D {
    [ExportCategory("AssetDependencies")]
    [Export] private Node3D cameraMount;
    [Export] private Node3D UIMount;
    [Export] private AnimationPlayer animator;
    [Export] private Node3D visualAnchor;

    [ExportCategory("Movement Settings")]
    [Export] private float walkingSpeed = 2;
    [Export] private float runningSpeed = 5;
    [Export] private int FallAcceleration { get; set; } = 75;
    [Export] private bool hasGravity = false;

    [ExportCategory("Camera Settings")]
    [Export] private float LookAroundSpeed = 0.01f;
    [Export] private float mouseSensitivityH = 0.1f;
    [Export] private float mouseSensitivityV = 0.1f;

    private Vector3 _targetVelocity = Vector3.Zero;
    private float speed;
    private string moveAnimation;
    private bool isRunning = false;
    private bool isLocked = false;
    private Tween tween;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        UIMount.Visible = false;
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

        if (Input.IsActionJustPressed("toggle_character_sheet")) {

            if (UIMount.Visible == true) {
                visualAnchor.Visible = true;

                tween = CreateTween();
                tween.TweenProperty(UIMount, "rotation", new Vector3(Mathf.DegToRad(90), 0, 0), 0.2).SetEase(Tween.EaseType.Out);
                tween.TweenProperty(UIMount, "visible", false, 0.2);
            }
            else {
                UIMount.Visible = true;

                Tween tween = CreateTween();
                tween.TweenProperty(UIMount, "rotation", new Vector3(0, 0, 0), 0.2).SetEase(Tween.EaseType.Out);
                tween.TweenProperty(visualAnchor, "visible", false, 0.2);
            }
        }

        if (!IsOnFloor() && hasGravity) _targetVelocity.Y -= FallAcceleration * (float)delta;

        if (Input.IsActionJustPressed("toggle_run")) isRunning = true;
        if (Input.IsActionJustReleased("toggle_run")) isRunning = false;


        Vector2 inputVector = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputVector.X, 0, inputVector.Y)).Normalized();
        if (!isLocked) {
            if (direction != Vector3.Zero) {
                if (isRunning) {
                    speed = runningSpeed;
                    moveAnimation = "running";
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
