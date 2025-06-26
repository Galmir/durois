using Godot;
using System;

public partial class HealthComponent : Node {
    [Export] private int maxHealth;
    [Export] private float healthRegen;
    private float _health;
    bool isDead = false;
    private Timer regenTimer;

    private float currenthealth {
        get { return _health; }
        set {
            if (isDead) return;
            _health = Mathf.Clamp(value, 0, maxHealth);
            HealthChanged?.Invoke(_health);
            if (_health == 0) {
                isDead = true;
                HasDied.Invoke();
            }
        }
    }
    public Action HasDied;
    public Action<float> HealthChanged;

    public override void _Ready() {
        regenTimer = new();
        regenTimer.Autostart = true;
        regenTimer.Start(1);
        regenTimer.Timeout += RegenerateHealth;
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
        Heal(healthRegen);
        regenTimer.Start(1);
    }
}
