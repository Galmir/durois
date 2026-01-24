using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WeaponItem : EquippableItem {
    [Export] public Array<Effect> WeaponDamage;

    protected override void UpdateDescription() {
        base.UpdateDescription();
    }
}
