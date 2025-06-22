using Godot;

[GlobalClass]
public partial class LootDropItem : Resource {
    [Export] public InventoryItem ItemDropped;
    [Export] public int LootWeight;
    [Export] public int MinAmountDropped;
    [Export] public int MaxAmountDropped;

    public int RollAmount() {
        return GD.RandRange(MinAmountDropped, MaxAmountDropped);
    }
}
