using Godot;

[GlobalClass]
public partial class DamageData : Resource {
    [Export] public Rules.DamageTypes DamageType { get; private set; }
    [Export] public float Amount { get; set; }
}
