using Godot;

public partial class EntityController : CharacterBody3D {
    [ExportCategory("AssetDependencies")]
    [Export] public AnimationPlayer animator;
    [Export] public Node3D visualAnchor;
    [Export] public Node FSM;

    [ExportCategory("Movement Settings")]
    [Export] public float walkingSpeed = 2;
    [Export] public float runningSpeed = 5;
    [Export] public int FallAcceleration { get; set; } = 75;
    [Export] public bool hasGravity = false;

    public CharacterBody3D targetEntity;


    public Vector3 _targetVelocity = Vector3.Zero;
    public float speed;
    public string moveAnimation;
    public bool isRunning = false;
    public bool isLocked = false;
    public Tween tween;

    public override void _Ready() {

    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (!IsOnFloor() && hasGravity) _targetVelocity.Y -= FallAcceleration * (float)delta;

        Velocity = _targetVelocity;
        MoveAndSlide();
    }
}
