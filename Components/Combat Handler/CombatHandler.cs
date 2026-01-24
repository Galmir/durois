using Godot;
using System;
using Godot.Collections;

public partial class CombatHandler : Node3D {
    public Action<Rules.EffectTypes> EffectStarted;
    public Action<Rules.EffectTypes> EffectEnded;
    public Action<float, float> HealthChanged;
    public Action<float> Damaged;
    public Action Died;

    [Export] public int MaxHealth { get; private set; } = 100;
    public int currentHealth { get; private set; } = 100;

    Vector3 velocityKnockBack = Vector3.Zero;
    float knockbackFriction = 800.0f;

    Array<Dictionary> activeDots = [];
    Dictionary<Rules.EffectTypes, Dictionary<string, Variant>> activeStatsModifiers = new();

    float speedMultiplier = 1.0f;
    float attackMultiplier = 1.0f;
    float defenseMultiplier = 1.0f;

    public Dictionary<Rules.EffectTypes, float> stateTimers = new Dictionary<Rules.EffectTypes, float>();
    public Dictionary<Rules.EffectTypes, bool> activeStates = new Dictionary<Rules.EffectTypes, bool>();

    Node3D parent;

    public override void _Ready() {
        parent = GetParent<Node3D>();
        currentHealth = MaxHealth;
    }


    public override void _PhysicsProcess(double delta) {
        ProcessDots((float)delta);
        ProcessStatModifiers((float)delta);
        ProcessStateTimers((float)delta);
        ProcessKnockback((float)delta);
    }


    public void ApplyEffect(Effect effect, Node3D source) {
        EffectStarted?.Invoke(effect.type);
        switch (effect.behavior) {
            case Rules.EffectBehaviors.Instant:
                ApplyInstantEffect(effect, source);
                break;
            case Rules.EffectBehaviors.DoT:
                AddDotEffect(effect);
                break;
            case Rules.EffectBehaviors.Buff:
            case Rules.EffectBehaviors.Debuff:
                AddStatModifier(effect);
                break;
        }

    }


    void TakeDamage(DamageData damage) {
        float finalDamage = damage.Amount;
        currentHealth = Mathf.Clamp(Mathf.RoundToInt(currentHealth - finalDamage), 0, MaxHealth);
        HealthChanged?.Invoke(currentHealth, MaxHealth);
        if (currentHealth <= 0) Died?.Invoke();
    }


    private void AddDotEffect(Effect effect) {
        Dictionary dotDict = new() {
            { "effect", effect },
            { "timer", 0.0f },
            { "elapsed", 0.0f }
        };

        activeDots.Add(dotDict);
    }


    private void ProcessDots(float delta) {
        for (int i = 0; i < activeDots.Count; i++) {
            Dictionary dot = activeDots[i];
            Effect e = (Effect)dot["effect"];
            dot["elapsed"] = (float)dot["elapsed"] + delta;
            dot["timer"] = (float)dot["timer"] + delta;

            if ((float)dot["timer"] >= e.tickInterval) {
                dot["timer"] = 0.0f;
                if (e.damage is not null) TakeDamage(e.damage);
            }

            if ((float)dot["elapsed"] >= e.duration) {
                EffectEnded?.Invoke(e.type);
                activeDots.RemoveAt(i);
                return;
            }
        }
    }


    private void ApplyStatModifier(StatModifier stat) {
        speedMultiplier *= stat.SpeedMultiplier;
        attackMultiplier *= stat.AttackMultiplier;
        defenseMultiplier *= stat.DefenseMultiplier;
    }


    private void RemoveStat(StatModifier stat) {
        speedMultiplier /= stat.SpeedMultiplier;
        attackMultiplier /= stat.AttackMultiplier;
        defenseMultiplier /= stat.DefenseMultiplier;
    }


    private void AddStatModifier(Effect effect) {
        if (effect.statModifier == null) return;

        Dictionary<string, Variant> effDict = new()  {
            { "modifier" , effect.statModifier},
            { "remainingTime", effect.duration}
        };

        activeStatsModifiers[effect.type] = effDict;

        ApplyStatModifier(effect.statModifier);
    }


    private void ProcessStatModifiers(float delta) {
        foreach (Rules.EffectTypes e in activeStatsModifiers.Keys) {
            Dictionary temp = (Dictionary)activeStatsModifiers[e];
            temp["remainingTime"] = (float)temp["remainingTime"] - delta;
            if ((float)temp["remainingTime"] <= 0) {
                StatModifier mod = (StatModifier)temp["modifier"];
                EffectEnded?.Invoke(e);
                activeStatsModifiers.Remove(e);
                break;
            }
        }
    }


    private void ApplyKnockbackFrom(Vector3 origin, float strength) {
        Vector3 fixedYdirection = new Vector3(parent.GlobalPosition.X, 0, parent.GlobalPosition.Z);
        Vector3 direction = (fixedYdirection - origin).Normalized();
        velocityKnockBack = direction * strength;
    }


    private void ProcessKnockback(float delta) {
        if (velocityKnockBack.LengthSquared() > 0.1) velocityKnockBack = velocityKnockBack.MoveToward(Vector3.Zero, knockbackFriction * delta);
    }


    private void ApplyState(Rules.EffectTypes effectType, float duration) {
        activeStates[effectType] = true;
        stateTimers[effectType] = duration;
    }


    private void ProcessStateTimers(float delta) {
        foreach (Rules.EffectTypes effectType in stateTimers.Keys) {
            stateTimers[effectType] = (float)stateTimers[effectType] - delta;
            if ((float)stateTimers[effectType] <= 0) {
                activeStates[effectType] = false;
                stateTimers.Remove(effectType);
                EffectEnded?.Invoke(effectType);
            }
        }
    }


    public bool IsUnder(Rules.EffectTypes effectType) {
        return activeStates[effectType];
    }


    private void ApplyInstantEffect(Effect effect, Node3D source = null) {
        if (effect.damage is not null) TakeDamage(effect.damage);
        if (effect.type == Rules.EffectTypes.Knockback && source is not null) ApplyKnockbackFrom(source.GlobalPosition, effect.knockbackStrength);
        if (
            effect.type == Rules.EffectTypes.Blind ||
            effect.type == Rules.EffectTypes.Stun) ApplyState(effect.type, effect.duration);
        if (effect.statModifier is not null) AddStatModifier(effect);
    }
}
