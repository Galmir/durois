using Godot;

[GlobalClass]
public partial class DamageData : Resource {
    [Export] public Rules.DamageTypes DamageType { get; private set; }
    [Export] public float Amount { get; set; }

    [Export(PropertyHint.Range, "0,100,1,suffix:%")]
    public float Variance { get; private set; }

    /// roll the damage instance. Returns a random number between the minimum damage and maximum given the coefficient
    public int RollDamage() {
        float minDamage = Amount - (Amount / 100 * Variance);
        float maxDamage = Amount + (Amount / 100 * Variance);
        double finalDamage = GD.RandRange(minDamage, maxDamage);
        return (int)finalDamage;
    }
}
