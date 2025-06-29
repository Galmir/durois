using Godot;
using System;

public partial class HealthComponent : Node {
    [Export] public int maxHealth { get; private set; }
    [Export] private float healthRegen;
    [Export] private PhysicsBody3D parent;
    [Export] private int ticksPerSecond;
    private float _health;
    bool isDead = false;
    private Timer HealthRegenTimer;

    public float currenthealth {
        get { return _health; }
        set {
            if (isDead) return;
            _health = Mathf.Clamp(value, 0, maxHealth);
            Signals.Instance.HealthChanged.Invoke(parent, this);
            if (_health == 0) {
                isDead = true;
                Signals.Instance.HasDied?.Invoke(parent);
            }
        }
    }

    public override void _Ready() {
        HealthRegenTimer = new();
        AddChild(HealthRegenTimer);
        HealthRegenTimer.Start(1 / ticksPerSecond);
        HealthRegenTimer.Timeout += RegenerateHealth;
        currenthealth = maxHealth;
    }
    public override void _PhysicsProcess(double delta) {

    }
    public void Heal(float amount) {
        currenthealth += amount;
    }
    public void Damage(float amount) {
        currenthealth -= amount;
    }


    public void RegenerateHealth() {
        if (isDead) return;
        Heal(healthRegen / ticksPerSecond);
        HealthRegenTimer.Start(1 / ticksPerSecond);
    }
}
