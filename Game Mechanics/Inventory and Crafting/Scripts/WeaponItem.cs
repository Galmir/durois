using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WeaponItem : EquippableItem {
    [Export] public Array<DamageData> WeaponDamage;
    [Export] public float AttackSpeedMultiplier; /// <summary>
                                                 /// Multiply Attack's CD to this. Greatswords should be slower, daggers faster...
                                                 /// </summary>
    protected override void UpdateDescription() {
        base.UpdateDescription();
        description += "\n[p]Damage \n";
        foreach (DamageData dmg in WeaponDamage) {
            int minDamage = (int)(dmg.Amount - (dmg.Amount / 100 * dmg.Variance));
            int maxDamage = (int)(dmg.Amount + (dmg.Amount / 100 * dmg.Variance));
            description += minDamage + " - " + maxDamage + " [img]uid://cj7mo21edk2pl[/img] \n";
        }
        description += "[/p]";

    }
}
