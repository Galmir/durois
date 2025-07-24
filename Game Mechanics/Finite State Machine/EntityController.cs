using Godot;
using Godot.Collections;

public partial class EntityController : CharacterBody3D {
    [ExportCategory("AssetDependencies")]
    [Export] public AnimationPlayer animator;
    [Export] public Node3D visualAnchor;
    [Export] public EffectReceiver effectReceiver;

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
        effectReceiver.Died += OnDied;
        effectReceiver.Damaged += DamageAmount;
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (!IsOnFloor() && hasGravity) _targetVelocity.Y -= FallAcceleration * (float)delta;

        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    protected void OnDied() {
        QueueFree();
    }


    protected void DamageAmount(float amount) {
        GD.Print(effectReceiver.currentHealth + "/" + effectReceiver.MaxHealth);
    }


    public void ReceiveHit(Array<Effect> effects, Node3D source) {
        foreach (Effect e in effects) {
            effectReceiver.ApplyEffect(e, source);
        }
    }
}
