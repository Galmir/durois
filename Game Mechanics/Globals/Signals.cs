using Godot;
using System;

public partial class Signals : Node {
    private static Signals _instance;
    public static Signals Instance => _instance;

    public override void _Ready() {
        if (_instance != null) {
            _instance.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }

    //Health
    public Action<PhysicsBody3D, HealthComponent> HealthChanged;
    public Action<PhysicsBody3D> HasDied;

    //Stamina
    public Action<PhysicsBody3D, StaminaComponent> StaminaChanged;
    public Action<PhysicsBody3D> IsFatigued;
}
