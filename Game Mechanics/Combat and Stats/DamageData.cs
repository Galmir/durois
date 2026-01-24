using System;
using Godot;

[GlobalClass]
public partial class DamageData : Resource {
    [Export] public Rules.DamageTypes DamageType { get; private set; }
    [Export] public float Amount { get; set; }
    [Export(PropertyHint.Range, "-0,100,1")] public int DamageRange { get; private set; }

    public float RollDamage() {
        return (float)GD.RandRange(Amount - (Amount * DamageRange), Amount + (Amount * DamageRange));
    }
}
