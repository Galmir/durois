using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LootTable : Resource {
    [Export] private Array<LootDropItem> guaranteedLoot;
    [Export] private Array<LootDropItem> lootTable;
    [Export] private int MinAmountOfRewards;
    [Export] private int MaxAmountOfRewards;
    [Export] private int MinAmountOfGold;
    [Export] private int MaxAmountOfGold;


    private int totalLootWeight;


    private Inventory LootDrop() {
        int rewardsAmount = (int)GD.RandRange(MinAmountOfRewards, MaxAmountOfRewards);
        Inventory rewardInventory = new Inventory();

        totalLootWeight = CalcTotalLootWeight();

        // give the guaranteed rewards:
        foreach (LootDropItem item in guaranteedLoot) rewardInventory.AddItem(item.ItemDropped, item.RollAmount());

        for (int i = 0; i < rewardsAmount; i++) {
            int roll = (int)GD.RandRange(1, totalLootWeight);
            foreach (LootDropItem item in lootTable) {
                if (roll <= item.LootWeight) {
                    rewardInventory.AddItem(item.ItemDropped, item.RollAmount());
                }
                else roll -= item.LootWeight;
            }
        }

        rewardInventory.Gold = GD.RandRange(MinAmountOfGold, MaxAmountOfGold);
        return rewardInventory;
    }

    private int CalcTotalLootWeight() {
        int weight = 0;
        foreach (LootDropItem item in lootTable) {
            weight += item.LootWeight;
        }
        return weight;
    }

    public Array<LootDropItem> GetPercentageDrop() {
        Array<LootDropItem> TotalData = [];
        int totalWeight = CalcTotalLootWeight();

        foreach (LootDropItem item in guaranteedLoot) {
            LootDropItem itemData = item;
            itemData.LootWeight = 100;
            TotalData.Add(itemData);
        }

        foreach (LootDropItem item in lootTable) {
            LootDropItem itemData = item;
            itemData.LootWeight = item.LootWeight / totalLootWeight * 10000;
            TotalData.Add(itemData);
        }

        return TotalData;
    }
}
