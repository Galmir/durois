using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Inventory : Resource {
    [Export] public Array<InventorySlot> InventorySlots { get; set; }
    [Export] public int Gold { get; set; } = 0;
    [Export] public Array<CraftingRecipe> KnownRecipes;

    public Action<InventorySlot> ItemObtained;


    /// <summary>
    /// add a specific Item inside the inventory. If the Item is stackable it increased the amount, otherwise it will make a new slot
    /// </summary>
    /// <param name="item">which item to put</param>
    /// <param name="amount"> how many of the item. Irrelevant with non stackable items liek weapons</param>
    public void AddItem(InventoryItem item, int amount = 1) {
        InventorySlot newSlot = new InventorySlot();
        newSlot.Item = item;
        newSlot.Amount = !item.IsStackable ? amount : 1;
        int alreadyOwned = GetAmountOfItem(item);
        if (alreadyOwned == 0 || !item.IsStackable) {
            InventorySlots.Add(newSlot);
        }
        else {
            foreach (InventorySlot slot in InventorySlots) {
                if (slot.Item.Name == item.Name) slot.Amount += amount;
                break;
            }
        }
        ItemObtained.Invoke(newSlot);
    }

    public void ClearInventory() {
        Gold = 0;
        InventorySlots.Clear();
    }

    public void RemoveItem(InventoryItem item, int amount = 1) {
        if (GetAmountOfItem(item) >= amount) {
            foreach (InventorySlot slot in InventorySlots) {
                if (slot.Item.Name == item.Name) {
                    slot.Amount -= amount;
                    if (slot.Amount == 0 || !item.IsStackable) InventorySlots.Remove(slot);
                    return;
                }
            }
        }
    }

    public int GetAmountOfItem(InventoryItem item) {
        foreach (InventorySlot slot in InventorySlots) {
            if (slot.Item.Name == item.Name) return slot.Amount;
        }

        return 0;
    }


    public void Loot(Inventory fromIventory) {
        foreach (InventorySlot slot in fromIventory.InventorySlots) {
            AddItem(slot.Item, slot.Amount);
        }

        Gold += fromIventory.Gold;
    }

    public bool CraftItemFromRecipe(CraftingRecipe recipe) {
        //double check if all items are available
        foreach (InventorySlot ingredient in recipe.RequiredIngredients) {
            if (GetAmountOfItem(ingredient.Item) < ingredient.Amount) return false;
        }

        //actually remove them and provide the resulting item
        foreach (InventorySlot ingredient in recipe.RequiredIngredients) {
            RemoveItem(ingredient.Item, ingredient.Amount);
        }
        AddItem(recipe.ResultingItem, recipe.ResultingItemAmount);
        return true;
    }


    public Array<InventorySlot> FilterBy(Rules.ItemTags filterTag) {
        Array<InventorySlot> filteredInventory = [];
        if (filterTag != Rules.ItemTags.ALL) {
            foreach (InventorySlot slot in InventorySlots) {
                if (slot.Item.FilterTags.Contains(filterTag)) filteredInventory.Add(slot);
            }

            return filteredInventory;
        }
        else {
            return InventorySlots;
        }
    }
}
