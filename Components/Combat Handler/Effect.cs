using Godot;

[GlobalClass]
public partial class Effect : Resource {
    [Export] public string name;
    [Export] public Rules.EffectTypes type;
    [Export] public Rules.EffectBehaviors behavior;
    [Export] public DamageData damage;
    [Export] public StatModifier statModifier;
    [Export] public float duration;
    [Export] public float tickInterval;
    [Export] public float knockbackStrength;
}
