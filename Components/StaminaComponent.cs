using Godot;

public partial class StaminaComponent : Node {
    [Export] public int maxStamina { get; private set; }
    [Export] private float staminaRegen = 10;
    [Export] private PhysicsBody3D parent;

    private float _stamina;
    public bool isFatigued = false;
    public bool isSpendingStamina = false;


    public float currentStamina {
        get { return _stamina; }
        set {
            _stamina = Mathf.Clamp(value, 0, maxStamina);
            Signals.Instance.StaminaChanged.Invoke(parent, this);
            if (_stamina == 0) {
                isFatigued = true;
                Signals.Instance.HasDied?.Invoke(parent);
            }
        }
    }


    public override void _Ready() {
        currentStamina = maxStamina;
    }


    public void Heal(float amount) {
        currentStamina += amount;
    }


    public void Damage(float amount) {
        if (isFatigued) return;
        currentStamina -= amount;
    }


    public override void _PhysicsProcess(double delta) {
        if (!isSpendingStamina) Heal(staminaRegen * (float)delta);
    }
}
